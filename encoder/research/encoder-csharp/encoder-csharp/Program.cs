using OpenCvSharp;
using System;

namespace encoder_csharp
{
    class Program
    {
        private const int width = 320;
        private const int height = 240;
        private static Random random = new Random();

        static void Main(string[] args)
        {
            EncoderRTMP encoder = new EncoderRTMP();
            encoder.Initialize(width, height, 25);
            encoder.Start("rtmp://localhost:1935/live/test");

            int size = width * height;
            for (int i = 0; i < 25; ++i) {
                Mat mat = new Mat(height, width, MatType.CV_8UC3, new Scalar(0, 0, 0));
                Cv2.PutText(mat, DateTime.Now.ToString("hh:mm:ss:fff"), new Point(random.Next(0, 220), random.Next(20, 220)), HersheyFonts.HersheySimplex, 0.5, new Scalar(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)));
                mat = mat.CvtColor(ColorConversionCodes.BGR2YUV_I420);
                encoder.Encode(mat.Data, mat.Data + size, mat.Data + size + size / 4);
                encoder.EncodeSEI(System.Text.Encoding.UTF8.GetBytes("Hello, world!"));

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
