#include <stdint.h>
#include "..\include\DeviceDiscovery.h"
#include "..\include\Config.h"
#include "..\include\UdpSocket.h"
#include "..\include\Utils.h"

#ifdef OS_WINDOWS
    #include <Windows.h>
#else
    #include <sys/socket.h>
    #include <netinet/in.h>
    #include <arpa/inet.h>
    #include <unistd.h>
    #include <netdb.h>
#endif

namespace Gige
{
    char* DeviceDiscovery()
    {
        UdpClient client;

        client.Open("255.255.255.255", GIGE_CONTROL_PORT);
        client.PermitBroadcast(true);

        uint8_t send_buf[8] = { 0x42, 0x11, 0x00, DISCOVERY_CMD, 0x00, 0x00 };
        ((uint16_t*)&send_buf)[3] = htons(1);

        int32_t ret = client.Send((char*)send_buf, sizeof(send_buf));

        uint8_t recv_buf[1024] = { 0 };
        ret = client.Recv((char*)recv_buf, sizeof(recv_buf));

        in_addr addr;

#ifdef OS_WINDOWS
        addr.S_un.S_addr = ntohl(LittleBigTran32(((uint32_t*)recv_buf)[11]));
#else
        addr.s_addr = ntohl(LittleBigTran32(((uint32_t*)recv_buf)[11]));
#endif

        client.Close();

        return inet_ntoa(addr);
    }
}