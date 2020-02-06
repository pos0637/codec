#ifndef GVCP_H
#define GVCP_H

#include <stdint.h>
#include "mutex.h"
#include "UdpSocket.h"

class Thread;
//class Mutex;

namespace Gige
{
    class MyGenICam;
    class UdpClient;
    class GigEVision;

    enum GvcpState
    {
        Idle = 0,
        None,
        ReadXML,
        GetControl,
        SetParameters,
        SendHeartbeat,
    };

    class Gvcp
    {
    public:

        Gvcp(GigEVision& gev);
        ~Gvcp();

        bool Start(const char* sAddr);
        void Stop();
        bool IsRunning();

        template<typename T>
        bool ReadRegister(const std::string& sKey, T* out)
        {
            uint32_t addr;
            if (!mGenicam->GetAddress(sKey, &addr))
                return false;

            return ReadRegister(addr, out);
        }

        template<typename T>
        bool ReadRegister(uint32_t addr, T* out)
        {
            Synchronized(mLock) {
                uint8_t sendbuf[12] = { 0x42, 0x01, 0x00, READREG_CMD, 0x00, 0x04 };
                ((uint16_t*)&sendbuf)[3] = htons(mMsgNr++);
                ((uint32_t*)&sendbuf)[2] = htonl(addr);

                mClient.Send((char*)sendbuf, sizeof(sendbuf));

                uint8_t recvbuf[256] = { 0 };
                if (mClient.Recv((char*)recvbuf, sizeof(recvbuf)) != 12)
                    return false;

                ((uint8_t*)out)[0] = recvbuf[11];
                ((uint8_t*)out)[1] = recvbuf[10];
                ((uint8_t*)out)[2] = recvbuf[9];
                ((uint8_t*)out)[3] = recvbuf[8];

                return true;
            }

            return false;
        }

        template<typename T>
        bool WriteRegister(const std::string& sKey, T nVal)
        {
            uint32_t addr;
            if (!mGenicam->GetAddress(sKey, &addr))
                return false;

            return WriteRegister(addr, nVal);
        }

        template<typename T>
        bool WriteRegister(uint32_t addr, T nVal)
        {
            Synchronized(mLock) {
                uint8_t sendbuf[16] = { 0x42, 0x01, 0x00, WRITEREG_CMD, 0x00, 0x08 };
                ((uint16_t*)&sendbuf)[3] = htons(mMsgNr++);
                ((uint32_t*)&sendbuf)[2] = htonl(addr);

                T temp = nVal;
                ((uint8_t*)&nVal)[0] = ((uint8_t*)&temp)[3];
                ((uint8_t*)&nVal)[1] = ((uint8_t*)&temp)[2];
                ((uint8_t*)&nVal)[2] = ((uint8_t*)&temp)[1];
                ((uint8_t*)&nVal)[3] = ((uint8_t*)&temp)[0];

                memcpy(&(((uint32_t*)&sendbuf)[3]), &nVal, sizeof(T));

                mClient.Send((char*)sendbuf, sizeof(sendbuf));
                uint8_t recvbuf[12] = { 0 };
                if (mClient.Recv((char*)recvbuf, sizeof(recvbuf)) != 12)
                    return false;
                else
                    return recvbuf[11] == 0x01;
            }

            return false;
        }

        std::vector<uint8_t> ReadMemory(uint32_t addr, uint32_t sSize, uint32_t nSize);

    private:

        static void* Run(void* handle);

    private:

        GvcpState mState;
        bool mIsRun;
        char* mIpAddr;
        GigEVision& mGev;
        MyGenICam* mGenicam;
        int32_t mMsgNr;
        Mutex* mLock;
        Thread* mThread;
        UdpClient mClient;
    };
}

#endif // GVCP_H
