#ifndef UDPSOCKET_H
#define UDPSOCKET_H
#include <stdint.h>

namespace Gige
{
    class UdpClient
    {
    public:
        UdpClient();
        ~UdpClient();

        bool Open(const char* ip, uint32_t port);
        void Close();
        bool Connect();
        void SetTimeout(uint32_t ms);
        void PermitBroadcast(bool val);
        int Send(char* buf, uint32_t size);
        int Recv(char* buf, uint32_t size);

    private:
        bool mConnected;
        uint32_t mSock;
    };

    class UdpService
    {
    public:
        UdpService();
        ~UdpService();

        bool Open(uint32_t port);
        void Close();
        int Recv(char* buf, uint32_t size);

    private:
        uint32_t mSock;
    };
}

#endif