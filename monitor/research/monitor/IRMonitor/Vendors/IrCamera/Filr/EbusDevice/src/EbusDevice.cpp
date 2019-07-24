#include <algorithm>
#include "..\include\EbusDevice.h"
#include "..\include\Tracker.h"

EbusDevice::EbusDevice()
{
    mGigeVision = new GigEVision();
    mDeviceType = 0;
}

EbusDevice::~EbusDevice()
{
    if (mGigeVision != NULL) {
        delete mGigeVision;
        mGigeVision = NULL;
    }
}

EbusDevice::!EbusDevice()
{
    if (mGigeVision != NULL) {
        delete mGigeVision;
        mGigeVision = NULL;
    }
}

System::Boolean EbusDevice::Initialize()
{
    return true;
}

System::Boolean EbusDevice::Open()
{
    if (!Tracker::GetTracker("EbusDeviceLog"))
        return false;

    char* ip = (char*)(void*)System::Runtime::InteropServices::Marshal::StringToHGlobalAnsi(mIp);
    if (!mGigeVision->Start(ip, mIp->Length))
        return false;

    return true;
}

System::Boolean EbusDevice::Close()
{
    mGigeVision->Stop();
    return true;
}

System::Boolean EbusDevice::Read(
    Devices::ReadMode mode,
    System::IntPtr dataAddr,
    System::Int32 dataLen)
{
    if (dataAddr == System::IntPtr::Zero)
        return false;

    if (mode == Devices::ReadMode::TemperatureArray)
        return mGigeVision->GetTemperatureData((void*)dataAddr, dataLen);
    else if (mode == Devices::ReadMode::ImageArray)
        return mGigeVision->GetImageData((void*)dataAddr, dataLen);

    return false;
}

System::Boolean EbusDevice::Read(
    Devices::ReadMode mode,
    System::Object^ data,
    System::Int32% useLen)
{
    if (data == nullptr)
        return false;

    float tempF = 0.0f;
    int tempI = 0;

    switch (mode) {
    case Devices::ReadMode::FrameRate: {
        if (!mGigeVision->GetFrameRate(&tempI))
            return false;

        System::Nullable<System::Int32> temp = (System::Nullable<System::Int32>)data;
        temp = tempI;
        return true;
        break;
    }

    case Devices::ReadMode::AtmosphericTemperature: {
        if (!mGigeVision->GetAtmosphericTemperature(&tempF))
            return false;

        System::Nullable<System::Single> temp = (System::Nullable<System::Single>)data;
        temp = tempF;
        return true;
        break;
    }

    case Devices::ReadMode::RelativeHumidity: {
        if (!mGigeVision->GetRelativeHumidity(&tempF))
            return false;

        System::Nullable<System::Single> temp = (System::Nullable<System::Single>)data;
        temp = tempF;
        return true;
        break;
    }

    case Devices::ReadMode::ReflectedTemperature: {
        if (!mGigeVision->GetReflectedTemperature(&tempF))
            return false;

        System::Nullable<System::Single> temp = (System::Nullable<System::Single>)data;
        temp = tempF;
        return true;
        break;
    }

    case Devices::ReadMode::ObjectDistance: {
        if (!mGigeVision->GetObjectDistance(&tempF))
            return false;

        System::Nullable<System::Single> temp = (System::Nullable<System::Single>)data;
        temp = tempF;
        return true;
        break;
    }

    case Devices::ReadMode::Emissivity: {
        if (!mGigeVision->GetObjectEmissivity(&tempF))
            return false;

        System::Nullable<System::Single> temp = (System::Nullable<System::Single>)data;
        temp = tempF;
        return true;
        break;
    }

    case Devices::ReadMode::Transmission: {
        if (!mGigeVision->GetObjectEmissivity(&tempF))
            return false;

        System::Nullable<System::Single> temp = (System::Nullable<System::Single>)data;
        temp = tempF;
        return true;
        break;
    }

    default:
        return false;
        break;
    }
}

System::Boolean EbusDevice::Write(Devices::WriteMode mode, System::Object^ data)
{
    if (data == nullptr)
        return false;

    bool ret = true;

    switch (mode) {
    case Devices::WriteMode::ConnectionString: {
        mIp = (System::String^)data;
        break;
    }

    case Devices::WriteMode::FrameRate:
        ret = mGigeVision->SetFrameRate((System::Int32)data);
        break;

    case Devices::WriteMode::AtmosphericTemperature:
        ret = mGigeVision->SetAtmosphericTemperature((System::Single)data);
        break;

    case Devices::WriteMode::RelativeHumidity:
        ret = mGigeVision->SetRelativeHumidity((System::Single)data);
        break;

    case Devices::WriteMode::ReflectedTemperature:
        ret = mGigeVision->SetReflectedTemperature((System::Single)data);
        break;

    case Devices::WriteMode::ObjectDistance:
        ret = mGigeVision->SetObjectDistance((System::Single)data);
        break;

    case Devices::WriteMode::Emissivity:
        ret = mGigeVision->SetObjectEmissivity((System::Single)data);
        break;

    case Devices::WriteMode::Transmission:
        ret = mGigeVision->SetEstimatedTransmission((System::Single)data);
        break;

    default:
        break;
    }

    return ret;
}

System::Boolean EbusDevice::Control(Devices::ControlMode mode)
{
    switch (mode) {
    case Devices::ControlMode::AutoFocus:
        return mGigeVision->AutoFocus();

    case Devices::ControlMode::FocusFar:
        return mGigeVision->SetFarFocus();

    case Devices::ControlMode::FocusNear:
        return mGigeVision->SetNearFocus();

    default:
        return false;
    }
}

Devices::DeviceType EbusDevice::GetDeviceType()
{
    return Devices::DeviceType::IrCamera;
}

Devices::DeviceStatus EbusDevice::GetDeviceStatus()
{
    return Devices::DeviceStatus::Running;
}