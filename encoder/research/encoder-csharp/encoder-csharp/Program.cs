using OpenCvSharp;
using System;

namespace encoder_csharp
{
    class Program
    {
        private const int width = 320;
        private const int height = 240;
        private const int frames_per_second = 25;
        private static Random random = new Random();

        static void Main(string[] args)
        {
            Encoder encoder = new Encoder();
            encoder.Initialize(width, height, frames_per_second);
            // encoder.Start("rtmp://localhost:1935/live/test");
            encoder.Start("1.h264");

            int size = width * height;
            for (int i = 0; i < 10 * frames_per_second; ++i) {
                Mat mat = new Mat(height, width, MatType.CV_8UC3, new Scalar(0, 0, 0));
                Cv2.PutText(mat, i + ": " + DateTime.Now.ToString("hh:mm:ss:fff"), new Point(random.Next(0, 220), random.Next(20, 220)), HersheyFonts.HersheySimplex, 0.5, new Scalar(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)));
                mat = mat.CvtColor(ColorConversionCodes.BGR2YUV_I420);
                encoder.Encode(mat.Data, mat.Data + size, mat.Data + size + size / 4);
                encoder.EncodeSEI(System.Text.Encoding.UTF8.GetBytes($"{i / frames_per_second} Hello, world!"));

                /*
                Cv2.ImShow("mat", mat);
                if (Cv2.WaitKey(0) == 27) {
                    break;
                }
                */
            }

            encoder.Encode(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            encoder.Stop();
            encoder.Dispose();
        }
    }
}
