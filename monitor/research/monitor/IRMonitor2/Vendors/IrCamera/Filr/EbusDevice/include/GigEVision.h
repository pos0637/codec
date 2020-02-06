#ifndef GIGEVISION_H
#define GIGEVISION_H

#include <stdint.h>
#include "../include/Config.h"

namespace Gige
{
    // forwarding
    class Gvcp;
    class Gvsp;

    class GigEVision
    {
    public:

        GigEVision();
        ~GigEVision();

    public:

        void OnGvcpConnected();

        bool Start(char* ip, int32_t len);
        void Stop();
        bool OpenAcquisition(int32_t streamPort);

        // frame rate
        bool SetFrameRate(int32_t rate);
        bool GetFrameRate(int32_t* value);

        // focus
        bool SetFocusStep(uint16_t step = 1000);
        bool AutoFocus();
        bool SetNearFocus();
        bool SetFarFocus();
        bool SetStopFouce();
        bool FocusIncrement();
        bool FocusDecrement();

        // object parameters
        bool GetReflectedTemperature(float* value);
        bool GetAtmosphericTemperature(float* value);
        bool GetObjectDistance(float* value);
        bool GetObjectEmissivity(float* value);
        bool GetRelativeHumidity(float* value);
        bool GetExtOpticsTemperature(float* value);
        bool GetExtOpticsTransmission(float* value);
        bool GetEstimatedTransmission(float* value);

        bool SetReflectedTemperature(float value);
        bool SetAtmosphericTemperature(float value);
        bool SetObjectDistance(float value);
        bool SetObjectEmissivity(float value);
        bool SetRelativeHumidity(float value);
        bool SetExtOpticsTemperature(float value);
        bool SetExtOpticsTransmission(float value);
        bool SetEstimatedTransmission(float value);

        // size
        bool GetWidth(int32_t* value);
        bool GetHeight(int32_t* value);

        // Raw
        bool GetRawDate(void* dataAddr, int32_t len);
        bool GetTemperatureData(void* dataAddr, int32_t dataLen);
        bool GetImageData(void* dataAddr, int32_t dataLen);

        // discovery carmer
        char* Discovery();

    private:
        bool StartAcquisition();

    public:

        FrameRate mFrameRate;
        float mReflectedTemperature; // 表面翻转温度
        float mAtmosphericTemperature; // 大气温度
        float mObjectDistance; // 距离
        float mObjectEmissivity;  // 辐射系数
        float mRelativeHumidity; // 相对湿度
        float mEstimatedTransmission;

    private:

        int32_t mWidth;
        int32_t mHeight;
        int32_t mSize;
        uint16_t* mBuffer = NULL;
        char* mIp;
        Gvcp* mGvcp;
        Gvsp* mGvsp;
    };
}

#endif // GIGEVISION_H
