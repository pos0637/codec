#ifndef GVSP_H
#define GVSP_H

#include <stdint.h>

class Thread;
class Mutex;

namespace Gige
{
    class GigEVision;

    class Gvsp
    {
    public:

        Gvsp(GigEVision& gev);
        ~Gvsp();
        bool Start();
        void Stop();
        bool IsRunning();
        bool GetRawData(void* dataAddr, int32_t len);
        void SetParameter(uint32_t port, uint32_t height, uint32_t width);

    private:

        GigEVision& mGev;
        uint32_t mSize;
        uint32_t mPort;
        uint32_t mWidth;
        uint32_t mHeight;
        bool mRun;
        bool mInit;
        Thread* mThread;
        Mutex* mLock;
        uint8_t* mImg;
        uint8_t* mTempImg;
        static void* StartReceive(void* handle);
        void RecHandler(char* buf, int32_t nBytes);
    };
}

#endif // GVSP_H
