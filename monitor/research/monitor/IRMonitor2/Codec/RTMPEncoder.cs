﻿using FFmpeg.AutoGen;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Codec
{
    /// <summary>
    /// RTMP编码器
    /// </summary>
    public unsafe class RTMPEncoder : IDisposable
    {
        private AVFormatContext* formatContext;
        private AVCodec* codec;
        private AVCodecContext* context;
        private AVFrame* frame;
        private AVPacket* packet;
        private AVStream* stream;
        private long pts;
        private int frames_per_second;
        private bool ioOpened;
        private Hashtable userData = Hashtable.Synchronized(new Hashtable());

        public void Dispose()
        {
            Reset();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="frames_per_second">帧率</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Initialize(int width, int height, int frames_per_second)
        {
            Reset();

            ffmpeg.avformat_network_init();

            fixed (AVFormatContext** c = &formatContext) {
                if (ffmpeg.avformat_alloc_output_context2(c, null, "flv", null) < 0) {
                    throw new Exception("Could not allocate output format context!");
                }
            }

            codec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_H264);
            if (codec == null) {
                throw new Exception("codec not found!");
            }

            context = ffmpeg.avcodec_alloc_context3(codec);
            if (context == null) {
                throw new Exception("alloc context fail");
            }

            context->codec_id = codec->id;
            context->codec_type = AVMediaType.AVMEDIA_TYPE_VIDEO;
            context->pix_fmt = AVPixelFormat.AV_PIX_FMT_YUV420P;
            context->bit_rate = 400000;
            context->width = width;
            context->height = height;
            context->time_base = new AVRational { num = 1, den = frames_per_second };
            context->framerate = new AVRational { num = frames_per_second, den = 1 };
            context->gop_size = 50;
            context->max_b_frames = 1;
            context->qmin = 10;
            context->qmax = 50;
            context->level = 41;
            context->refs = 1;
            this.frames_per_second = frames_per_second;

            if (codec->id == AVCodecID.AV_CODEC_ID_H264) {
                ffmpeg.av_opt_set(context->priv_data, "preset", "slow", 0);
            }

            if ((formatContext->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER) != 0) {
                context->flags |= ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
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

            if (formatContext != null) {
                ffmpeg.avformat_free_context(formatContext);
                formatContext = null;
            }

            ffmpeg.avformat_network_deinit();
        }

        /// <summary>
        /// 开始推流
        /// </summary>
        /// <param name="uri">服务器资源地址</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start(string uri)
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

            stream = ffmpeg.avformat_new_stream(formatContext, codec);
            stream->time_base = new AVRational { num = 1, den = frames_per_second };
            stream->codecpar->codec_tag = 0;

            ffmpeg.avcodec_parameters_from_context(stream->codecpar, context);
            if ((formatContext->oformat->flags & ffmpeg.AVFMT_GLOBALHEADER) != 0) {
                stream->codec->flags |= ffmpeg.AV_CODEC_FLAG_GLOBAL_HEADER;
            }

            ffmpeg.avio_open2(&formatContext->pb, uri, ffmpeg.AVIO_FLAG_READ_WRITE, null, null).ThrowExceptionIfError();
            ffmpeg.avformat_write_header(formatContext, null).ThrowExceptionIfError();
            ioOpened = true;
            pts = 0;
        }

        /// <summary>
        /// 停止推流
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Stop()
        {
            if (ioOpened) {
                ffmpeg.av_write_trailer(formatContext);
                ffmpeg.avio_close(formatContext->pb);
                ioOpened = false;
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

        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="data1">Y通道数据</param>
        /// <param name="data2">U通道数据</param>
        /// <param name="data3">V通道数据</param>
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

                byte* oldData = null;
                int oldSize = 0;
                IntPtr newData = IntPtr.Zero;
                if (userData.ContainsKey(packet->pts)) {
                    byte[] content = userData[packet->pts] as byte[];
                    userData.Remove(packet->pts);

                    oldData = packet->data;
                    oldSize = packet->size;
                    newData = Marshal.AllocHGlobal(packet->size + content.Length);
                    Buffer.MemoryCopy(packet->data, (void*)newData, packet->size + content.Length, packet->size);
                    Marshal.Copy(content, 0, IntPtr.Add(newData, packet->size), content.Length);

                    packet->data = (byte*)newData.ToPointer();
                    packet->size = packet->size + content.Length;
                }

                if (packet->pts != ffmpeg.AV_NOPTS_VALUE)
                    packet->pts = ffmpeg.av_rescale_q(packet->pts, context->time_base, stream->time_base);
                if (packet->dts != ffmpeg.AV_NOPTS_VALUE)
                    packet->dts = ffmpeg.av_rescale_q(packet->dts, context->time_base, stream->time_base);
                double sec = packet->pts * ffmpeg.av_q2d(stream->time_base);
                Console.WriteLine($"frame: {frame->pts}, pts: {packet->pts}, dts: {packet->dts}, time: {sec}, length: {packet->size}");

                // byte[] data = new byte[packet->size];
                // Marshal.Copy((IntPtr)packet->data, data, 0, packet->size);
                // Console.WriteLine("data: " + string.Concat(data.Select(b => string.Format("0x{0},", b.ToString("X2"))).ToArray()));               

                ffmpeg.av_interleaved_write_frame(formatContext, packet);

                if (newData != IntPtr.Zero) {
                    packet->data = oldData;
                    packet->size = oldSize;
                    Marshal.FreeHGlobal(newData);
                }

                ffmpeg.av_packet_unref(packet);
            } while (true);
        }

        /// <summary>
        /// 自定义数据编码
        /// </summary>
        /// <param name="content">自定义数据</param>
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
            for (int i = 0; i < 16; ++i) {
                data[6 + seiPayloadSizeLength + i] = 0xAB;
            }
            content.CopyTo(data, 6 + seiPayloadSizeLength + 16);
            data[6 + seiPayloadSizeLength + 16 + length] = 0x80;

            if (userData.ContainsKey(pts)) {
                userData.Remove(pts);
            }

            userData.Add(pts, data);
        }
    }
}
