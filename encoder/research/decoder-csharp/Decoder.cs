using FFmpeg.AutoGen;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace decoder_csharp
{
    public unsafe class Decoder : IDisposable
    {
        public delegate void OnPacket(AVPacket* packet);
        public delegate void OnFrame(AVFrame* frame);

        private const int INBUF_SIZE = 4096;
        private AVCodec* codec;
        private AVCodecParserContext* parser;
        private AVCodecContext* context;
        private AVFrame* frame;
        private AVPacket* packet;
        private FileStream fs;
        private byte[] buffer;

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

            parser = ffmpeg.av_parser_init((int)codec->id);
            if (parser == null) {
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
            frame = ffmpeg.av_frame_alloc();
            if (frame == null) {
                throw new Exception("alloc frame fail");
            }

            ffmpeg.avcodec_open2(context, codec, null).ThrowExceptionIfError();

            packet = ffmpeg.av_packet_alloc();
            if (packet == null) {
                throw new Exception("alloc packet fail");
            }

            fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            buffer = new byte[INBUF_SIZE + ffmpeg.AV_INPUT_BUFFER_PADDING_SIZE];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (fs != null) {
                fs.Close();
                fs.Dispose();
                fs = null;
            }

            if (parser != null) {
                ffmpeg.av_parser_close(parser);
                parser = null;
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
            int read = fs.Read(buffer, 0, INBUF_SIZE);
            if (read <= 0) {
                return false;
            }

            IntPtr ptr = Marshal.AllocHGlobal(read);
            IntPtr data = ptr;
            Marshal.Copy(buffer, 0, ptr, read);

            while (read > 0) {
                int ret = ffmpeg.av_parser_parse2(parser, context, &packet->data, &packet->size, (byte*)data, read, ffmpeg.AV_NOPTS_VALUE, ffmpeg.AV_NOPTS_VALUE, 0);
                if (ret < 0) {
                    Marshal.FreeHGlobal(data);
                    throw new Exception("Error while parsing");
                }

                data += ret;
                read -= ret;

                if (packet->size > 0) {
                    DecodePacket(onPacket, onFrame);
                }
            }

            Marshal.FreeHGlobal(ptr);

            return true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void DecodePacket(OnPacket onPacket, OnFrame onFrame)
        {
            onPacket?.Invoke(packet);

            int ret = ffmpeg.avcodec_send_packet(context, packet);
            if (ret < 0) {
                return;
            }

            while (ret >= 0) {
                ret = ffmpeg.avcodec_receive_frame(context, frame);
                if (ret == ffmpeg.AVERROR(ffmpeg.EAGAIN) || ret == ffmpeg.AVERROR_EOF) {
                    return;
                }
                else if (ret < 0) {
                    throw new Exception("Error during decoding");
                }

                onFrame?.Invoke(frame);
            }
        }
    }
}
