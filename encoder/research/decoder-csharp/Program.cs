﻿using OpenCvSharp;
using System;
using System.Runtime.InteropServices;
using static decoder_csharp.Decoder;

namespace decoder_csharp
{
    class Program
    {
        static unsafe void Main(string[] args)
        {
            Decoder decoder = new Decoder();
            decoder.Initialize();
            decoder.Start("1.h264");

            OnPacket onPacket = (packet) => {
                Console.WriteLine($"length: {packet->size}");
                byte[] data = new byte[packet->size];
                Marshal.Copy((IntPtr)packet->data, data, 0, packet->size);
                // Console.WriteLine("data: " + string.Concat(data.Select(b => string.Format("0x{0},", b.ToString("X2"))).ToArray()));

                byte[] NALHeader = new byte[] { 0x00, 0x00, 0x00, 0x01, 0x06 };
                int startIndex = 0;
                int pos;
                while ((pos = data.IndexOf(NALHeader, startIndex)) >= 0) {
                    int length = 0;
                    for (int i = 6; ; ++i) {
                        length += data[i];
                        if (data[i] != 0xFF) {
                            break;
                        }
                    }

                    int seiPayloadSizeLength = (length / 0xFF) + (((length % 0xFF) == 0) ? 0 : 1);
                    byte[] content = new byte[length];
                    Array.Copy(data, 4 + 1 + 1 + seiPayloadSizeLength + 16, content, 0, length);
                    string userData = System.Text.Encoding.UTF8.GetString(content);
                    Console.WriteLine($"SEI length: {length}, {userData}");

                    startIndex = pos + length;
                }
            };

            OnFrame onFrame = (frame) => {
                Mat mat = new Mat(frame->height, frame->width, MatType.CV_8UC3, new Scalar(0, 0, 0));
                mat = mat.CvtColor(ColorConversionCodes.BGR2YUV_I420);
                IntPtr ptr = mat.Data;

                int length1 = frame->linesize[0] * frame->height;
                byte[] data = new byte[length1];
                Marshal.Copy((IntPtr)frame->data[0], data, 0, length1);
                for (int i = 0; i < frame->height; ++i, ptr += frame->width) {
                    Marshal.Copy(data, frame->linesize[0] * i, ptr, frame->width);
                }

                int length2 = frame->linesize[1] * frame->height / 2;
                data = new byte[length2];
                Marshal.Copy((IntPtr)frame->data[1], data, 0, length2);
                for (int i = 0; i < frame->height / 2; ++i, ptr += frame->width / 2) {
                    Marshal.Copy(data, frame->linesize[1] * i, ptr, frame->width / 2);
                }

                int length3 = frame->linesize[2] * frame->height / 2;
                data = new byte[length3];
                Marshal.Copy((IntPtr)frame->data[2], data, 0, length3);
                for (int i = 0; i < frame->height / 2; ++i, ptr += frame->width / 2) {
                    Marshal.Copy(data, frame->linesize[2] * i, ptr, frame->width / 2);
                }

                mat = mat.CvtColor(ColorConversionCodes.YUV2BGR_I420);
                Cv2.ImShow($"mat", mat);
                Cv2.WaitKey(0);
            };

            while (decoder.Decode(onPacket, onFrame)) {
            }

            decoder.Stop();
            decoder.Dispose();
        }
    }
}
