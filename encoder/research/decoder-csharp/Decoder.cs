using FFmpeg.AutoGen;
using System;
using System.Runtime.CompilerServices;

namespace decoder_csharp
{
    public unsafe class Decoder : IDisposable
    {
        public delegate void OnPacket(AVPacket* packet);
        public delegate void OnFrame(AVFrame* frame);

        private const int INBUF_SIZE = 4096; 
        private AVFormatContext* formatContext;
        private AVCodec* codec;
        private AVCodecContext* context;
        private AVFrame* frame;
        private AVPacket* packet;

        public void Dispose()
        {
            Reset();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize()
        {
            Reset();

            codec = ffmpeg.avcodec_find_decoder(AVCodecID.AV_CODEC_ID_H264);
            if (codec == null) {
                throw new Exception("codec not found!");
            }

            context = ffmpeg.avcodec_alloc_context3(codec);
            if (context == null) {
                throw new Exception("alloc context fail");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Reset()
        {
            Stop();

            if (context != null) {
                ffmpeg.avcodec_close(context);
                fixed (AVCodecContext** c = &context) {
                    ffmpeg.avcodec_free_context(c);
                }
                context = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start(string filename)
        {
            fixed (AVFormatContext** c = &formatContext) {
                if (ffmpeg.avformat_open_input(c, filename, null, null) < 0) {
                    throw new Exception("Could not allocate input format context!");
                }

                ffmpeg.avformat_find_stream_info(formatContext, null).ThrowExceptionIfError();
                ffmpeg.avcodec_parameters_to_context(context, formatContext->streams[0]->codecpar);
            }

            frame = ffmpeg.av_frame_alloc();
            if (frame == null) {
                throw new Exception("alloc frame fail");
            }

            ffmpeg.avcodec_open2(context, codec, null).ThrowExceptionIfError();

            packet = ffmpeg.av_packet_alloc();
            if (packet == null) {
                throw new Exception("alloc packet fail");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (formatContext != null) {
                ffmpeg.avformat_free_context(formatContext);
                formatContext = null;
            }

            if (packet != null) {
                fixed (AVPacket** p = &packet) {
                    ffmpeg.av_packet_free(p);
                }
                packet = null;
            }

            if (frame != null) {
                fixed (AVFrame** f = &frame) {
                    ffmpeg.av_frame_free(f);
                }
                frame = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Decode(OnPacket onPacket, OnFrame onFrame)
        {
            int ret = ffmpeg.av_read_frame(formatContext, packet);
            if (ret < 0) {
                return false;
            }

            onPacket?.Invoke(packet);

            ret = ffmpeg.avcodec_send_packet(context, packet);
            if (ret < 0) {
                return true;
            }

            while (ret >= 0) {
                ret = ffmpeg.avcodec_receive_frame(context, frame);
                if ((ret == ffmpeg.AVERROR(ffmpeg.EAGAIN)) || (ret == ffmpeg.AVERROR_EOF)) {
                    return true;
                }
                else if (ret < 0) {
                    throw new Exception("error during encoding");
                }

                Console.WriteLine($"id: {context->frame_number}");
                onFrame?.Invoke(frame);
            }

            return true;
        }
    }
}
