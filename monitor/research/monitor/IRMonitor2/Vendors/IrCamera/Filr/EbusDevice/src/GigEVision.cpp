#include "..\include\GenICam.h"
#include "..\include\Gvcp.h"
#include "..\include\Gvsp.h"
#include "..\include\GigEVision.h"
#include "..\include\Utils.h"
#include "..\include\Config.h"
#include "..\include\DeviceDiscovery.h"
#include "..\include\Tracker.h"

namespace Gige
{
    GigEVision::GigEVision()
    {
        mIp = NULL;
        mGvcp = NULL;
        mGvsp = NULL;
        mBuffer = NULL;
        mWidth = 0;
        mHeight = 0;
        mSize = 0;

        mFrameRate = Rate12Hz;
        mObjectEmissivity = 0.95f;
        mReflectedTemperature = 20.0f;
        mEstimatedTransmission = 0.9f;
        mAtmosphericTemperature = 25.0f;
        mRelativeHumidity = 0.75f;
        mObjectDistance = 5.0f;

        mGvcp = new Gvcp(*this);
        mGvsp = new Gvsp(*this);
    }

    GigEVision::~GigEVision()
    {
        if (mGvsp) {
            delete mGvsp;
            mGvsp = NULL;
        }

        if (mGvcp) {
            delete mGvcp;
            mGvcp = NULL;
        }

        if (mBuffer) {
            delete mBuffer;
            mBuffer = NULL;
        }
    }

    bool GigEVision::Start(char* ip, int32_t len)
    {
        mIp = (char*)malloc(len + 1);
        if (!mIp)
            return false;

        memcpy(mIp, ip, len);
        mIp[len] = '\0';

        Tracker::Instance().Log(mIp);
        if (!mGvcp->Start(mIp))
            return false;

        return true;
    }

    void GigEVision::Stop()
    {
        mGvcp->WriteRegister("Cust::AcquisitionStopReg", STOP_GRAB_VALUE);
        mGvcp->Stop();
        if (mGvsp->IsRunning())
            mGvsp->Stop();
    }

    void GigEVision::OnGvcpConnected()
    {
        if (!mGvsp->IsRunning()) {
            if (!GetWidth(&mWidth))
                return;

            if (!GetHeight(&mHeight))
                return;

            mSize = mWidth * mHeight;
            if (mBuffer) {
                delete mBuffer;
                mBuffer = NULL;
            }

            mBuffer = (uint16_t*)malloc(mSize * sizeof(uint16_t));
            if (!mBuffer)
                return;

            StartAcquisition();
        }
    }

    bool GigEVision::SetFrameRate(int32_t rate)
    {
        FrameRate temp;
        if (rate >= 50)
            temp = Rate50Hz;
        else if (rate / 25 == 1)
            temp = Rate25Hz;
        else if ((rate / 12 >= 1) && (rate / 12 <= 2))
            temp = Rate12Hz;
        else if (rate / 6 == 1)
            temp = Rate6Hz;
        else
            temp = Rate3Hz;

        if (temp == mFrameRate)
            return false;

        if (mGvcp->IsRunning()) {
            if (!mGvcp->WriteRegister("IRFrameRateReg", temp))
                return false;

            mFrameRate = temp;
        }
        else {
            mFrameRate = temp;
        }

        return true;
    }

    bool GigEVision::GetFrameRate(int32_t* value)
    {
        switch (mFrameRate) {
            case Rate50Hz:
                *value = 50;
                break;

            case Rate25Hz:
                *value = 25;
                break;

            case Rate12Hz:
                *value = 12;
                break;

            case Rate6Hz:
                *value = 6;
                break;

            case Rate3Hz:
                *value = 3;
                break;

            default:
                return false;
        }

        return true;
    }

    bool GigEVision::StartAcquisition()
    {
        int32_t height = 0, width = 0;
        if (!GetHeight(&height) || !GetWidth(&width)) {
            Tracker::Instance().Log("Get Width and Height Fail!");
            return false;
        }

        Tracker::Instance().Log("StartAcquisition");

        int32_t port = 0;
        if (!Utils::GetAvaliablePort(port)) {
            Tracker::Instance().Log("GetAvaliablePort fail");
            return false;
        }

        Sleep(1000);

        if (!OpenAcquisition(port)) {
            Tracker::Instance().Log("OpenAcquisition");
            return false;
        }

        mGvsp->SetParameter(port, height, width);

        return mGvsp->Start();
    }

    bool GigEVision::OpenAcquisition(int32_t streamPort)
    {
        mGvcp->WriteRegister("Cust::AcquisitionStopReg", STOP_GRAB_VALUE);

        u_long ip = 0;
        char* localIp = Utils::GetLocalAddr(mIp);
        if (!localIp)
            return false;

        Tracker::Instance().Log(localIp);

        if (!Utils::GetHostByte(localIp, &ip))
            return false;

        if (!mGvcp->WriteRegister("Cust::GevSCPHostPortReg", streamPort)) {
            printf("Cust::GevSCPHostPortReg Failure1\n");
            return false;
        }

        if (!mGvcp->WriteRegister("Cust::GevSCDAReg", ip)) {
            printf("Cust::GevSCPHostPortReg Failure2\n");
            return false;
        }

        if (!mGvcp->WriteRegister("Cust::GevSCPSPacketSizeReg", GVSP_PACKET_SIZE)) {
            printf("Cust::GevSCPHostPortReg Failure3\n");
            return false;
        }

        if (!mGvcp->WriteRegister("Cust::AcquisitionStartReg", 0x1)) {
            printf("Cust::GevSCPHostPortReg Failure4\n");
            return false;
        }

        return true;
    }

    bool GigEVision::SetFocusStep(uint16_t step)
    {
        if (step > 1000)
            step = 1000;
        return mGvcp->WriteRegister("FocusStepReg", step);
    }

    bool GigEVision::AutoFocus()
    {
        bool ret = mGvcp->WriteRegister("AutoFocusReg", 0x0);
        return ret;
    }

    bool GigEVision::SetNearFocus()
    {
        // A615
        // if (!mGvcp->WriteRegister("FocusStepReg", 1000))
        //     return false;
        // if (!mGvcp->WriteRegister("FocusIncrementReg", 0x0))
        //     return false;

        if (!mGvcp->WriteRegister("FocusSpeedReg", 20))
            return false;

        if (!mGvcp->WriteRegister("FocusDirectionReg", 2))
            return false;

        Sleep(200);

        if (!mGvcp->WriteRegister("FocusDirectionReg", 0x0))
            return false;

        return true;
    }

    bool GigEVision::SetFarFocus()
    {
        // A615
        // if (!mGvcp->WriteRegister("FocusStepReg", 1000))
        //     return false;
        // if (!mGvcp->WriteRegister("FocusDecrementReg", 0x0))
        //     return false;

        if (!mGvcp->WriteRegister("FocusSpeedReg", 20))
            return false;

        if (!mGvcp->WriteRegister("FocusDirectionReg", 1))
            return false;

        Sleep(200);

        if (!mGvcp->WriteRegister("FocusDirectionReg", 0x0))
            return false;

        return true;
    }

    bool GigEVision::SetStopFouce()
    {
        return mGvcp->WriteRegister("FocusDirectionReg", 0);
    }

    bool GigEVision::FocusIncrement()
    {
        bool ret = mGvcp->WriteRegister("FocusIncrementReg", 0x0);
        Sleep(1000);
        return ret;
    }

    bool GigEVision::FocusDecrement()
    {
        bool ret = mGvcp->WriteRegister("FocusDecrementReg", 0x0);
        Sleep(1000);
        return ret;
    }

    bool GigEVision::GetReflectedTemperature(float* value)
    {
        *value = mReflectedTemperature;
        return true;
    }

    bool GigEVision::SetReflectedTemperature(float value)
    {
        if (mReflectedTemperature == value)
            return true;

        if (mGvcp->IsRunning()) {
            if (!mGvcp->WriteRegister("ReflectedTemperatureReg", value))
                return false;

            mReflectedTemperature = value;
        }
        else {
            mReflectedTemperature = value;
        }

        return true;
    }

    bool GigEVision::GetAtmosphericTemperature(float* value)
    {
        *value = mAtmosphericTemperature;
        return true;
    }

    bool GigEVision::SetAtmosphericTemperature(float value)
    {
        if (mAtmosphericTemperature == value)
            return true;

        if (mGvcp->IsRunning()) {
            if (!mGvcp->WriteRegister("AtmosphericTemperatureReg", value))
                return false;

            mAtmosphericTemperature = value;
        }
        else {
            mAtmosphericTemperature = value;
        }

        return true;
    }

    bool GigEVision::GetObjectDistance(float* value)
    {
        *value = mObjectDistance;
        return true;
    }

    bool GigEVision::SetObjectDistance(float value)
    {
        if (mObjectDistance == value)
            return true;

        if (mGvcp->IsRunning()) {
            if (!mGvcp->WriteRegister("ObjectDistanceReg", value))
                return false;

            mObjectDistance = value;
        }
        else {
            mObjectDistance = value;
        }

        return true;
    }

    bool GigEVision::GetObjectEmissivity(float* value)
    {
        *value = mObjectEmissivity;
        return true;
    }

    bool GigEVision::SetObjectEmissivity(float value)
    {
        if (mObjectEmissivity == value)
            return true;

        if (mGvcp->IsRunning()) {
            if (!mGvcp->WriteRegister("ObjectEmissivityReg", value))
                return false;

            mObjectEmissivity = value;
        }
        else {
            mObjectEmissivity = value;
        }

        return true;
    }

    bool GigEVision::GetRelativeHumidity(float* value)
    {
        *value = mRelativeHumidity;
        return true;
    }

    bool GigEVision::SetRelativeHumidity(float value)
    {
        if (mRelativeHumidity == value)
            return true;

        if (mGvcp->IsRunning()) {
            if (!mGvcp->WriteRegister("RelativeHumidityReg", value))
                return false;

            mRelativeHumidity = value;
        }
        else {
            mRelativeHumidity = value;
        }

        return true;
    }

    bool GigEVision::GetExtOpticsTemperature(float* value)
    {
        return mGvcp->ReadRegister("ExtOpticsTemperatureReg", value);
    }

    bool GigEVision::SetExtOpticsTemperature(float value)
    {
        return mGvcp->WriteRegister("ExtOpticsTemperatureReg", value);
    }

    bool GigEVision::GetExtOpticsTransmission(float* value)
    {
        return mGvcp->ReadRegister("ExtOpticsTransmissionReg", value);
    }

    bool GigEVision::SetExtOpticsTransmission(float value)
    {
        return mGvcp->WriteRegister("ExtOpticsTransmissionReg", value);
    }

    bool GigEVision::GetEstimatedTransmission(float* value)
    {
        *value = mEstimatedTransmission;
        return true;
    }

    bool GigEVision::SetEstimatedTransmission(float value)
    {
        if (mEstimatedTransmission == value)
            return true;

        if (mGvcp->IsRunning()) {
            if (!mGvcp->WriteRegister("EstimatedTransmissionReg", value))
                return false;

            mEstimatedTransmission = value;
        }
        else {
            mEstimatedTransmission = value;
        }

        return true;
    }

    bool GigEVision::GetWidth(int32_t* value)
    {
        return mGvcp->ReadRegister("SensorWidthReg", value);
    }

    bool GigEVision::GetHeight(int32_t* value)
    {
        return mGvcp->ReadRegister("SensorHeightReg", value);
    }

    bool GigEVision::GetRawDate(void* dataAddr, int32_t len)
    {
        return mGvsp->GetRawData(dataAddr, len);
    }

    bool GigEVision::GetTemperatureData(void* dataAddr, int32_t dataLen)
    {
        int32_t size = mSize * sizeof(float);
        if (dataLen < size)
            return false;

        if (!GetRawDate((void*)mBuffer, mSize * sizeof(short)))
            return false;

        float* dst = (float*)dataAddr;
        uint16_t* src = mBuffer;
        int32_t len = mWidth * mHeight;
        while (len--) {
            if (*src >= 65535)
                *src = 65535;
            *dst++ = (*src++) / 10.0f - 273.15f;
        }
        return true;
    }

    bool GigEVision::GetImageData(void* dataAddr, int32_t dataLen)
    {
        int32_t size = mSize * sizeof(char);
        if (dataLen < size)
            return false;

        if (!GetRawDate((void*)mBuffer, mWidth * mHeight * sizeof(short)))
            return false;

        uint16_t* src = mBuffer;
        uint8_t* dst = (uint8_t*)((void*)dataAddr);
        uint16_t smin = USHRT_MAX, smax = 0;
        size = mSize;
        while (size--) {
            smin = __min(*src, smin);
            smax = __max(*src, smax);
            src++;
        }

        src = mBuffer;
        uint16_t span = smax - smin;
        if (span == 0) span = 1;
        uint32_t sample;
        size = mSize;
        while (size--) {
            sample = ((*src++ - smin) *  UCHAR_MAX / span);
            *dst++ = (uint8_t)sample;
        }
        return true;
    }

    char* GigEVision::Discovery()
    {
        return DeviceDiscovery();
    }
}
