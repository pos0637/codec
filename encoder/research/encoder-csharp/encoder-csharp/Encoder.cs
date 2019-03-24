using FFmpeg.AutoGen;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace encoder_csharp {
    /// <summary>
    /// 编码器
    /// refs: https://github.com/FFmpeg/FFmpeg/blob/master/doc/examples/encode_video.c
    /// </summary>
    public unsafe class Encoder : IDisposable {
        private AVCodec* codec;
        private AVCodecContext* context;
        private AVFrame* frame;
        private AVPacket* packet;
        private FileStream fs;
        private int pts;

        public void Dispose() {
            Reset();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize(int width, int height, int frames_per_second) {
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
        public void Reset() {
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
        public void Start(string filename) {
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
        public void Stop() {
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
        public void Encode(byte[] data1 = null, byte[] data2 = null, byte[] data3 = null) {
            if ((data1 != null) && (data2 != null) && (data3 != null)) {
                ffmpeg.av_frame_make_writable(frame).ThrowExceptionIfError();
                /*
                Marshal.Copy(data1, 0, (IntPtr)frame->data[0], data1.Length);
                Marshal.Copy(data2, 0, (IntPtr)frame->data[1], data2.Length);
                Marshal.Copy(data3, 0, (IntPtr)frame->data[2], data3.Length);                                
                */

                /* prepare a dummy image */
                /* Y */
                for (int y = 0; y < context->height; y++) {
                    for (int x = 0; x < context->width; x++) {
                        frame->data[0][y * frame->linesize[0] + x] = (byte)(x + y + pts * 3);
                    }
                }

                /* Cb and Cr */
                for (int y = 0; y < context->height / 2; y++) {
                    for (int x = 0; x < context->width / 2; x++) {
                        frame->data[1][y * frame->linesize[1] + x] = (byte)(128 + y + pts * 2);
                        frame->data[2][y * frame->linesize[2] + x] = (byte)(64 + x + pts * 5);
                    }
                }

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
                fs.Write(data, 0, packet->size);

                ffmpeg.av_packet_unref(packet);
            } while (true);
        }
    }
}
