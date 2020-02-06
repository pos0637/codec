#include <iterator>
#include <memory>
#include "..\include\GigEVision.h"
#include "..\include\GenICam.h"
#include "..\include\Gvcp.h"
#include "..\include\thread.h"
#include "..\include\Config.h"
#include "..\include\Tracker.h"

namespace Gige
{
    Gvcp::Gvcp(GigEVision& gev) : mGev(gev)
    {
        mState = Idle;
        mIsRun = false;
        mGenicam = new MyGenICam(*this);
        mThread = new Thread();
        mLock = new Mutex();
        mMsgNr = 2;
    }

    Gvcp::~Gvcp()
    {
        Stop();

        if (mThread) {
            delete mThread;
            mThread = NULL;
        }

        if (mLock) {
            delete mLock;
            mLock = NULL;
        }
    }

    bool Gvcp::Start(const char* sAddr)
    {
        Synchronized(mLock) {
            if (mState != Idle)
                return false;

            mState = None;
            mIsRun = true;
            mIpAddr = (char*)sAddr;
            mThread->Start(Run, this);
            return true;
        }
    }

    void Gvcp::Stop()
    {
        Synchronized(mLock) {
            mIsRun = false;
            mThread->WaitFor();
            mClient.Close();
            mGenicam->Clear();
            mState = Idle;
        }
    }

    void* Gvcp::Run(void* handle)
    {
        int32_t errorTime = 0;
        Gvcp* it = (Gvcp*)handle;

        while (it->mIsRun) {
            switch (it->mState)
            {
            case None: {
                if (!it->mClient.Open(it->mIpAddr, GIGE_CONTROL_PORT))
                    continue;

                it->mClient.SetTimeout(2000);
                if (!it->mClient.Connect())
                    continue;

                errorTime = 0;
                it->mState = ReadXML;
                break;
            }

            case ReadXML: { // 获取xml
                if (!it->mGenicam->ReadXmlFile()) {
                    Tracker::Instance().Log("Read Xml File Failure");
                    if (errorTime++ >= 3) {
                        it->mClient.Close();
                        it->mState = None;
                    }
                    continue;
                }

                errorTime = 0;
                it->mState = GetControl;
                Tracker::Instance().Log("Read Xml File Success");
                break;
            }

            case GetControl: {
                if (!it->WriteRegister("GevCCPReg", ControlAccess)) {
                    Tracker::Instance().Log("Get Control Failure");
                    if (errorTime++ >= 3) {
                        it->mClient.Close();
                        it->mState = None;
                    }
                    continue;
                }

                errorTime = 0;
                it->mState = SetParameters;
                Tracker::Instance().Log("Get Control Success");
                break;
            }

            case SetParameters: {
                // 固定心跳时候为5S
                if (!it->WriteRegister("GevHeartbeatTimeoutReg", 5000)
                    || !it->WriteRegister("IRFrameRateReg", it->mGev.mFrameRate)
                    || !it->WriteRegister("ObjectEmissivityReg", it->mGev.mEstimatedTransmission)
                    || !it->WriteRegister("ReflectedTemperatureReg", it->mGev.mReflectedTemperature)
                    || !it->WriteRegister("EstimatedTransmissionReg", it->mGev.mEstimatedTransmission)
                    || !it->WriteRegister("AtmosphericTemperatureReg", it->mGev.mAtmosphericTemperature)
                    || !it->WriteRegister("RelativeHumidityReg", it->mGev.mReflectedTemperature)
                    || !it->WriteRegister("ObjectDistanceReg", it->mGev.mObjectDistance))
                {
                    if (errorTime++ >= 3) {
                        it->mClient.Close();
                        it->mState = None;
                    }
                    continue;
                }

                printf("Device Open Success\n");
                errorTime = 0;
                it->mState = SendHeartbeat;
                break;
            }

            case SendHeartbeat: {
                uint32_t key = 0;
                if (!it->ReadRegister("GevCCPReg", &key)) {
                    if (errorTime++ >= 3) {
                        it->mClient.Close();
                        it->mState = None;
                    }
                    continue;
                }

                if (key == 0) {
                    bool ret = it->WriteRegister("GevCCPReg", ControlAccess);
                    if (ret == false) {
                        if (errorTime++ >= 3) {
                            it->mClient.Close();
                            it->mState = None;
                        }
                        continue;
                    }
                }

                it->mGev.OnGvcpConnected();

                errorTime = 0;
                break;
            }

            default:
                break;
            }

            Sleep(1000);
        }

        return NULL;
    }

    bool Gvcp::IsRunning()
    {
        Synchronized(mLock) {
            return (this->mState == SendHeartbeat);
        }
    }

    std::vector<uint8_t> Gvcp::ReadMemory(uint32_t addr, uint32_t sSize, uint32_t nSize)
    {
        std::vector<uint8_t> retVec;

        Synchronized(mLock) {
            static const int32_t cnHdLen = 12;
            uint8_t sendbuf[16] = { 0x42, 0x01, 0x00, READMEM_CMD, 0x00, 0x08 };
            uint8_t recvbuf[548] = { 0 };
            size_t len = 0;

            do {
                memset(recvbuf, 0, sizeof(recvbuf));
                int32_t lenOfPacket = sSize;

                ((uint16_t*)&sendbuf)[3] = htons(mMsgNr++);
                ((uint32_t*)&sendbuf)[2] = htonl(addr + len);
                ((uint32_t*)&sendbuf)[3] = htonl(lenOfPacket);
                mClient.Send((char*)sendbuf, sizeof(sendbuf));

                int32_t nRead = mClient.Recv((char*)recvbuf, sizeof(recvbuf));
                if (nRead <= 0)
                    return retVec;

                nRead -= cnHdLen;
                if (nRead != lenOfPacket)
                    continue;

                len += nRead;
                std::copy(recvbuf + cnHdLen, recvbuf + cnHdLen + nRead, std::back_inserter(retVec));

            } while (len < nSize);

            return retVec;
        }

        return retVec;
    }
}