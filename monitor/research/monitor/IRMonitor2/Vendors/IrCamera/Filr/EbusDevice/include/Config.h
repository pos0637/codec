#ifndef CONFIGURE_H
#define CONFIGURE_H

#define GVSP_PACKET_SIZE 1444

// IP header£º20 , UDP header: 8 , GVCP header: 8
#define GVSP_PACKET_DATA_SIZE (1444 - 20 - 8 - 8)

#define GIGE_CONTROL_PORT 3956 // Gige ¿ØÖÆ¶Ë¿Ú

#define GVCP_PACKET_MAX_SIZE 576 

#define GVSP_SELECT_ERROR_TIME 5

#ifndef max
#define max(a,b) (((a) > (b)) ? (a) : (b))
#endif

#ifndef min
#define min(a,b) (((a) < (b)) ? (a) : (b))
#endif

#ifndef LittleBigTran32
#define LittleBigTran32(x) (((x & 0xff) << 24) | ((x & 0xff00) << 8) |\
                                ((x & 0xff0000) >> 8)|((x & 0xff000000) >> 24))
#endif

#ifndef LittleBigTran16
#define LittleBigTran16(x) (((x & 0x00ff) << 8)| ((x & 0xff00) >> 8))
#endif

enum ReceiveState
{
    NoHead,
    HasHead,
    HasLength,
};

enum Bootstrap_Registers
{
    DATASTREAM_PACKET_SIZE = 0x00000d04,
    // GRABBING_STOPPED = 0x0001020c,
    // PIXEL_FORMAT = 0x0001010c,
    First_URL_Register = 0x00000200,
};

enum EnVals
{
    STOP_GRAB_VALUE = 0x01,
    // PIXEL_FORMAT_8BIT_VALUE = 0x01000801,
    // PIXEL_FORMAT_12BIT_VALUE = 0x01000c01,
};

enum Access
{
    OpenAccess = 0x0,
    ExclusiveAccess = 0x1,
    ControlAccess = 0x2,
};

enum Device_Memory_Access
{
    READREG_CMD = 0x0080,
    READREG_ACK = 0x0081,
    WRITEREG_CMD = 0x0082,
    WRITEREG_ACK = 0x0083,
    READMEM_CMD = 0x0084,
    READMEM_ACK = 0x0085,
    WRITEMEM_CMD = 0x0086,
    WRITEMEM_ACK = 0x0087,
    PENDING_ACK = 0x0089,
};

enum Discovery_Protocol_Control
{
    DISCOVERY_CMD = 0x0002,
    DISCOVERY_ACK = 0x0003,
    FORCEIP_CMD = 0x0004,
    FORCEIP_ACK = 0x0005,
};

enum FrameRate
{
    Rate50Hz = 0x0000,
    Rate25Hz = 0x0001,
    Rate12Hz = 0x0002,
    Rate6Hz = 0x0004,
    Rate3Hz = 0x0005,
};

#endif