#pragma once

using namespace System;

namespace Renderer {
    public ref class Renderer
    {
    public:

        static void FillSurface(
            System::IntPtr^ src,
            System::IntPtr^ dst,
            int width,
            int stride,
            int height)
        {
            int length = stride * height;
            int padding = stride - width;
            unsigned char* p = (unsigned char*)dst->ToPointer();
            unsigned char* b = (unsigned char*)src->ToPointer();
            int w = width;
            int h = height;

            // Y
            while (h--) {
                while (w--) {
                    *p++ = *b++;
                }
                p += padding;
                w = width;
            }

            // U
            w = width;
            h = height;

            while ((h -= 2) >= 0) {
                while ((w -= 2) >= 0) {
                    *p++ = *b++;
                }
                p += padding / 2;
                w = width;
            }

            // V
            w = width;
            h = height;

            while ((h -= 2) >= 0) {
                while ((w -= 2) >= 0) {
                    *p++ = *b++;
                }
                p += padding / 2;
                w = width;
            }
        }
    };
}
