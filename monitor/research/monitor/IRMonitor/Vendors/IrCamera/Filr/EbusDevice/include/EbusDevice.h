#ifndef __DEVICE_H__
#define __DEVICE_H__

#include "GigEVision.h"

using namespace Gige;

public ref class EbusDevice : public Devices::IDevice
{
public:

    EbusDevice();
    ~EbusDevice();

protected:

    !EbusDevice();

public:

    System::Boolean Initialize() override;

    System::Boolean Open() override;

    System::Boolean Close() override;

    System::Boolean Read(
        Devices::ReadMode mode,
        System::IntPtr dataAddr,
        System::Int32 dataLen) override;

    System::Boolean Read(
        Devices::ReadMode mode,
        System::Object^ data,
        System::Int32% useLen) override;

    System::Boolean Write(
        Devices::WriteMode mode,
        System::Object^ data) override;

    System::Boolean Control(Devices::ControlMode mode)override;

    Devices::DeviceType GetDeviceType() override;

    Devices::DeviceStatus GetDeviceStatus() override;

private:

    System::String^ mIp;
    System::Int32 mDeviceType;
    GigEVision * mGigeVision;
};

#endif
