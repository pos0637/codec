using FFmpeg.AutoGen;
using System;
using System.Runtime.InteropServices;

namespace decoder_csharp
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

        public static int IndexOf(this byte[] arrayToSearchThrough, byte[] patternToFind, int startIndex = 0)
        {
            if (patternToFind.Length > (arrayToSearchThrough.Length - startIndex)) {
                return -1;
            }

            for (int i = startIndex; i < arrayToSearchThrough.Length - patternToFind.Length - startIndex; i++) {
                bool found = true;
                for (int j = 0; j < patternToFind.Length; j++) {
                    if (arrayToSearchThrough[i + j] != patternToFind[j]) {
                        found = false;
                        break;
                    }
                }

                if (found) {
                    return i;
                }
            }

            return -1;
        }
    }
}
