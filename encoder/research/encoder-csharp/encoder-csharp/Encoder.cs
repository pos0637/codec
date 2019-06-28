using FFmpeg.AutoGen;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace encoder_csharp
{
    /// <summary>
    /// 编码器
    /// refs: https://github.com/FFmpeg/FFmpeg/blob/master/doc/examples/encode_video.c
    /// </summary>
    public unsafe class Encoder : IDisposable
    {
        private AVCodec* codec;
        private AVCodecContext* context;
        private AVFrame* frame;
        private AVPacket* packet;
        private FileStream fs;
        private int pts;

        public void Dispose()
        {
            Reset();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize(int width, int height, int frames_per_second)
        {
            Reset();

            codec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_H264);
            if (codec == null) {
                throw new Exception("codec not found!");
            }

            context = ffmpeg.avcodec_alloc_context3(codec);
            if (context == null) {
                throw new Exception("alloc context fail");
            }

            context->bit_rate = 400000;
            context->width = width;
            context->height = height;
            context->time_base = new AVRational { num = 1, den = frames_per_second };
            context->framerate = new AVRational { num = frames_per_second, den = 1 };
            context->gop_size = 10;
            context->max_b_frames = 1;
            context->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;

            if (codec->id == AVCodecID.AV_CODEC_ID_H264) {
                ffmpeg.av_opt_set(context->priv_data, "preset", "slow", 0);
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

            frame->format = (int)context->pix_fmt;
            frame->width = context->width;
            frame->height = context->height;

            ffmpeg.av_frame_get_buffer(frame, 32).ThrowExceptionIfError();
            ffmpeg.av_frame_make_writable(frame).ThrowExceptionIfError();
            ffmpeg.avcodec_open2(context, codec, null).ThrowExceptionIfError();

            packet = ffmpeg.av_packet_alloc();
            if (packet == null) {
                throw new Exception("alloc packet fail");
            }

            fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            pts = 0;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (fs != null) {
                fs.Write(new byte[] { 0x0, 0x0, 0x1, 0xb7 }, 0, 4);
                fs.Flush();
                fs.Close();
                fs.Dispose();
                fs = null;
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
        public void Encode(IntPtr data1, IntPtr data2, IntPtr data3)
        {
            if ((data1 != IntPtr.Zero) && (data2 != IntPtr.Zero) && (data3 != IntPtr.Zero)) {
                ffmpeg.av_frame_make_writable(frame).ThrowExceptionIfError();

                int size = frame->width * frame->height;
                Buffer.MemoryCopy(data1.ToPointer(), frame->data[0], size, size);
                Buffer.MemoryCopy(data2.ToPointer(), frame->data[1], size / 4, size / 4);
                Buffer.MemoryCopy(data3.ToPointer(), frame->data[2], size / 4, size / 4);

                frame->pts = pts++;
                ffmpeg.avcodec_send_frame(context, frame).ThrowExceptionIfError();
            }
            else {
                ffmpeg.avcodec_send_frame(context, null).ThrowExceptionIfError();
            }

            do {
                int ret = ffmpeg.avcodec_receive_packet(context, packet);
                if ((ret == ffmpeg.AVERROR(ffmpeg.EAGAIN)) || (ret == ffmpeg.AVERROR_EOF)) {
                    return;
                }
                else if (ret < 0) {
                    throw new Exception("error during encoding");
                }

                byte[] data = new byte[packet->size];
                Marshal.Copy((IntPtr)packet->data, data, 0, packet->size);
                // Console.WriteLine("data: " + string.Concat(data.Select(b => string.Format("0x{0},", b.ToString("X2"))).ToArray()));
                fs.Write(data, 0, packet->size);

                ffmpeg.av_packet_unref(packet);
            } while (true);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void EncodeSEI(byte[] content)
        {
            int length = 16 + content.Length;
            int seiPayloadSizeLength = (length / 0xFF) + (((length % 0xFF) == 0) ? 0 : 1);
            // NAL header + SEI payload type + SEI payload size + SEI payload uuid + SEI payload content + rbsp trailing bits
            byte[] data = new byte[4 + 1 + 1 + seiPayloadSizeLength + 16 + length + 1];
            byte[] NALHeader = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x06 };
            NALHeader.CopyTo(data, 0);
            data[5] = 0x05; // user_data_unregistered
            for (int i = 1; i < seiPayloadSizeLength; ++i) {
                data[5 + i] = 0xFF;
            }
            data[5 + seiPayloadSizeLength] = (byte)(length % 0xFF);
            content.CopyTo(data, 6 + seiPayloadSizeLength + 16);
            data[6 + seiPayloadSizeLength + 16 + length] = 0x80;

            fs.Write(data, 0, data.Length);
        }
    }
}
