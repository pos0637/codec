using OpenCvSharp;
using System;
using System.Runtime.InteropServices;

namespace encoder_csharp {
    class Program {
        private const int width = 320;
        private const int height = 240;
        private static Random random = new Random();

        static void Main(string[] args) {
            Encoder encoder = new Encoder();
            encoder.Initialize(width, height, 25);
            encoder.Start("1.h264");

            byte[] data1 = new byte[width * height];
            byte[] data2 = new byte[width * height / 4];
            byte[] data3 = new byte[width * height / 4];

            for (int i = 0; i < 25; ++i) {
                Mat mat = new Mat(height, width, MatType.CV_8UC3, new Scalar(0, 0, 0));
                Cv2.PutText(mat, DateTime.Now.ToString("hh:mm:ss:fff"), new Point(random.Next(0, 220), random.Next(20, 220)), HersheyFonts.HersheySimplex, 0.5, new Scalar(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)));
                mat = mat.CvtColor(ColorConversionCodes.BGR2YUV_I420);

                Marshal.Copy(mat.Data, data1, 0, data1.Length);
                Marshal.Copy(mat.Data + data1.Length, data2, 0, data2.Length);
                Marshal.Copy(mat.Data + data1.Length + data2.Length, data3, 0, data3.Length);

                encoder.Encode(data1, data2, data3);

                /*
                Cv2.ImShow("mat", mat);
                if (Cv2.WaitKey(0) == 27) {
                    break;
                }
                */
            }

            encoder.Encode();
            encoder.Stop();
            encoder.Dispose();
        }
    }
}
