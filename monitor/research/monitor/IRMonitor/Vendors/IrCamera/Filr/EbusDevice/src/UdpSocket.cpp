#include "..\include\UdpSocket.h"
#include "..\include\Utils.h"
#include "..\include\Tracker.h"

#ifdef OS_WINDOWS
    #include <Windows.h>
    #pragma comment(lib,"wsock32.lib")
#elif defined(OS_LINUX) || defined(OS_UNIX)
    #include <sys/socket.h>
    #include <netinet/in.h>
    #include <arpa/inet.h>
    #include <unistd.h>
    #include <netdb.h>
#endif

namespace Gige
{
    struct sockaddr_in addrServ;

    UdpClient::UdpClient()
    {
#ifdef OS_WINDOWS
        WSADATA data;
        WORD w = MAKEWORD(2, 0);
        ::WSAStartup(w, &data);
#endif
        mSock = INVALID_SOCKET;
        mConnected = false;
    }

    UdpClient::~UdpClient()
    {
#ifdef OS_WINDOWS
        WSACleanup();
#endif
        Close();
    }

    bool UdpClient::Open(const char* ip, uint32_t port)
    {
        mSock = socket(AF_INET, SOCK_DGRAM, 0);

        u_long addr = 0;
        if (!Utils::GetNetworkByte(ip, &addr)) {
            return false;
        }

#ifdef OS_WINDOWS
        addrServ.sin_addr.S_un.S_addr = addr;
#else
        addrServ.sin_addr.s_addr = addr;//inet_addr(sAddr);
#endif
        addrServ.sin_family = AF_INET;
        addrServ.sin_port = htons(port);

        return true;
    }

    void UdpClient::Close()
    {
#ifdef OS_WINDOWS
        closesocket(mSock);
#else
        close(m_sock);
#endif
        mSock = INVALID_SOCKET;
        mConnected = false;
    }

    bool UdpClient::Connect()
    {
        if (connect(mSock, (sockaddr *)&addrServ, sizeof(addrServ)) == -1) {
            Close();
            return false;
        }

        mConnected = true;

        return true;
    }

    void UdpClient::SetTimeout(uint32_t ms)
    {
        //设置发送和接收超时
#ifdef OS_WINDOWS
        uint32_t timeout = ms;
        ::setsockopt(mSock, SOL_SOCKET, SO_SNDTIMEO, (char*)&timeout, sizeof(uint32_t));
        ::setsockopt(mSock, SOL_SOCKET, SO_RCVTIMEO, (char*)&timeout, sizeof(uint32_t));
#else
        struct timeval timeout = { ms/1000, 0 };
        ::setsockopt(m_sock, SOL_SOCKET, SO_SNDTIMEO, (char *)&timeout, sizeof(struct timeval));
        ::setsockopt(m_sock, SOL_SOCKET, SO_RCVTIMEO, (char *)&timeout, sizeof(struct timeval));
#endif
    }

    void UdpClient::PermitBroadcast(bool val)
    {
        // 设置该套接字为广播类型
        ::setsockopt(mSock, SOL_SOCKET, SO_BROADCAST, (char*)&val, sizeof(val));
    }

    int UdpClient::Send(char* buf, uint32_t size)
    {
        if (mConnected)
            return send(mSock, (char*)buf, size, 0);
        else
            return sendto(mSock, (char*)buf, size, 0, (struct sockaddr *)&addrServ, sizeof(addrServ));
    }

    int UdpClient::Recv(char* buf, uint32_t size)
    {
        if (mConnected)
            return recv(mSock, (char*)buf, size, 0);
        else
            return recvfrom(mSock, (char*)buf, size, 0, NULL, NULL);
    }


    fd_set fd;

    UdpService::UdpService()
    {
#ifdef OS_WINDOWS
        WSADATA data;
        WORD w = MAKEWORD(2, 0);
        ::WSAStartup(w, &data);
#endif
        mSock = INVALID_SOCKET;
    }

    UdpService::~UdpService()
    {
#ifdef OS_WINDOWS
        WSACleanup();
#endif
        Close();
    }

    bool UdpService::Open(uint32_t port)
    {
        mSock = socket(AF_INET, SOCK_DGRAM, 0);
        struct sockaddr_in addrServ;

#ifdef OS_WINDOWS
        addrServ.sin_addr.S_un.S_addr = htonl(INADDR_ANY); //inet_addr(it->m_gev.Dev().StreamIP);
#else
        addrServ.sin_addr.s_addr = htonl(INADDR_ANY); //inet_addr(it->m_gev.Dev().StreamIP);
#endif

        addrServ.sin_family = AF_INET;
        addrServ.sin_port = htons(port);

        // 增大udp缓存区
        int32_t value = 65535;
        if (::setsockopt(mSock, SOL_SOCKET, SO_RCVBUF, (char*)&value, sizeof(value)) < 0)
            Tracker::Instance().Log("setsockopt fail\n");

        if (bind(mSock, (struct sockaddr *)&addrServ, sizeof(addrServ)) == -1) {
            Tracker::Instance().Log("bind fail\n");
            return false;
        }

        FD_ZERO(&fd);
        FD_SET(mSock, &fd);

        return true;
    }

    void UdpService::Close()
    {
#ifdef OS_WINDOWS
        closesocket(mSock);
#else
        close(m_sock);
#endif
        mSock = INVALID_SOCKET;
        FD_ZERO(&fd);
    }

    int UdpService::Recv(char* buf, uint32_t size)
    {
        int32_t nBytes = -1;
        fd_set tempfd = fd;
        struct timeval timeout;
        timeout.tv_sec = 2;
        timeout.tv_usec = 500000;

        int32_t nRet = select(FD_SETSIZE, &tempfd, NULL, NULL, &timeout);
        switch (nRet) {
        case 0:
            return 0;
            break;

        case -1:
            return -1;
            break;

        default:
            if (FD_ISSET(mSock, &tempfd)) {
                nBytes = recvfrom(mSock, (char*)buf, size, 0, NULL, NULL);
            }
            break;
        }
        return nBytes;
    }
}