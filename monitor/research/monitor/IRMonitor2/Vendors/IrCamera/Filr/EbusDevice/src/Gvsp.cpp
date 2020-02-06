#include <stdio.h>
#include <iostream>
#include <string.h>
#include <malloc.h>
#include <limits.h>
#include "..\include\GigEVision.h"
#include "..\include\Gvsp.h"
#include "..\include\mutex.h"
#include "..\include\thread.h"
#include "..\include\Config.h"
#include "..\include\UdpSocket.h"
#include "..\include\Tracker.h"

namespace Gige
{
    Gvsp::Gvsp(GigEVision& gev) : mGev(gev)
    {
        mWidth = 0;
        mHeight = 0;
        mPort = 0;
        mThread = new Thread();
        mLock = new Mutex();
        mRun = false;
        mInit = false;
        mImg = NULL;
        mTempImg = NULL;
    }

    Gvsp::~Gvsp()
    {
        Stop();
        if (mThread)
            delete mThread;

        if (mLock)
            delete mLock;

        if (mImg)
            free(mImg);

        if (mTempImg)
            free(mTempImg);
    }

    void Gvsp::SetParameter(uint32_t port, uint32_t height, uint32_t width)
    {
        mPort = port;
        mWidth = width;
        mHeight = height;
        mSize = mWidth * mHeight * sizeof(uint16_t);
    }

    bool Gvsp::Start()
    {
        Synchronized(mLock) {
            if (mRun)
                return false;

            if (mImg) {
                free(mImg);
                mImg = NULL;
            }

            mImg = (uint8_t*)malloc(mSize);
            if (!mImg)
                return false;

            if (mTempImg) {
                free(mTempImg);
                mTempImg = NULL;
            }

            mTempImg = (uint8_t*)malloc(mSize);
            if (!mTempImg) {
                free(mImg);
                mImg = NULL;
                return false;
            }

            mRun = true;
            mThread->Start(StartReceive, this);
            return true;
        }
    }

    void Gvsp::Stop()
    {
        Synchronized(mLock) {
            mRun = false;
            mThread->WaitFor();
        }
    }

    bool Gvsp::IsRunning()
    {
        Synchronized(mLock) {
            return mRun;
        }
    }

    void* Gvsp::StartReceive(void* handle)
    {
        Gvsp* it = (Gvsp*)handle;

        UdpService service;
        service.Open(it->mPort);

        int32_t overtime = GVSP_SELECT_ERROR_TIME;
        char m_buf[2000] = { 0 };

        while (it->mRun) {
            memset(m_buf, 0, sizeof(m_buf));
            int32_t nBytes = service.Recv(m_buf, sizeof(m_buf));
            switch (nBytes) {
            case 0:
                if (--overtime <= 0) {
                    if (!it->mGev.OpenAcquisition(it->mPort)) {
                        Tracker::Instance().Log("Retry OpenAcquisition Fail");
                    }
                    else {
                        overtime = GVSP_SELECT_ERROR_TIME;
                        Tracker::Instance().Log("Retry OpenAcquisition Success");
                    }
                }
                break;

            case -1: {
                Tracker::Instance().Log("StartReceive Failure");
                int32_t port = 0;
                if (!Utils::GetAvaliablePort(port)) {
                    Tracker::Instance().Log("GetAvaliablePort fail");
                    break;
                }

                Sleep(1000);

                service.Close();
                it->mPort = port;
                service.Open(port);

                if (!it->mGev.OpenAcquisition(it->mPort)) {
                    Tracker::Instance().Log("Retry OpenAcquisition Fail\n");
                }
                else {
                    overtime = GVSP_SELECT_ERROR_TIME;
                    Tracker::Instance().Log("Retry OpenAcquisition Success\n");
                }

                break;
            }

            default:
                if (nBytes > 0)
                    it->RecHandler(m_buf, nBytes);
                break;
            }
        }

        service.Close();

        return NULL;
    }

    void Gvsp::RecHandler(char* buf, int32_t nBytes)
    {
        static ReceiveState state = NoHead;
        static int32_t curPartOfFrame = 0;
        uint16_t nPartOfFrame = LittleBigTran16(((uint16_t*)buf)[3]);
        int32_t frame = LittleBigTran32(((uint32_t*)buf)[0]);

        switch (state) {
        case NoHead:
            if (buf[4] == 0x01) {
                uint16_t nWidth = LittleBigTran16(((uint16_t*)buf)[13]);
                uint16_t nHeight = LittleBigTran16(((uint16_t*)buf)[15]);

                if (mWidth != nWidth || mHeight != nHeight)
                    return;

                curPartOfFrame = nPartOfFrame;
                state = HasHead;
            }
            break;

        case HasHead:
            if (buf[4] == 0x03) {
                if ((nPartOfFrame != curPartOfFrame + 1) && (nPartOfFrame > curPartOfFrame)) {
#ifdef _DEBUG
                    printf("Miss packet form %d to %d\n", curPartOfFrame, nPartOfFrame);
#endif
                    state = NoHead;
                    return;
                }
                curPartOfFrame = nPartOfFrame;
                int32_t nPayloadSize = nBytes - 8;

                uint32_t currentSize = (nPartOfFrame - 1)* GVSP_PACKET_DATA_SIZE + nPayloadSize;
                if (currentSize > mSize) {
                    state = NoHead;
                    return;
                }

                memcpy(mTempImg + (nPartOfFrame - 1) * GVSP_PACKET_DATA_SIZE, buf + 8, nPayloadSize);

                if (currentSize == mSize)
                    state = HasLength;
            }
            break;

        case HasLength:
            if (buf[4] == 0x02) {
                Synchronized(mLock) {
                    memcpy(mImg, mTempImg, mSize);
                    mInit = true;
                }
            }
            state = NoHead;
            break;

        default:
            break;
        } 
    }

    bool Gvsp::GetRawData(void* dataAddr, int32_t len)
    {
        if ((!dataAddr) || (mSize != len) || (!mInit))
            return false;

        Synchronized(mLock) {
            memcpy(dataAddr, mImg, mSize);
        }

        return true;
    }
}