using FFmpeg.AutoGen;
using System;
using System.Runtime.InteropServices;

namespace encoder_csharp
{
    internal static class Helper
    {
        public static unsafe string av_strerror(int error)
        {
            var bufferSize = 1024;
            var buffer = stackalloc byte[bufferSize];
            ffmpeg.av_strerror(error, buffer, (ulong)bufferSize);
            var message = Marshal.PtrToStringAnsi((IntPtr)buffer);
            return message;
        }

        public static int ThrowExceptionIfError(this int ret)
        {
            if (ret < 0) {
                throw new Exception(av_strerror(ret));
            }

            return ret;
        }
    }
}
