#include <string.h>

#if(defined(_WIN32) || defined(_WIN64))
#include <Windows.h>
#pragma comment(lib,"wsock32.lib")
#else
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <netdb.h>
#endif
#include "..\include\Common.h"

namespace Gige
{
    std::vector<std::string> Split(std::string str, const char* delim)
    {
        std::vector<std::string> parts;
        std::string::size_type pos1 = 0, pos2 = 0;
        while (pos1 != std::string::npos) {
            pos1 = str.find(delim, pos2);
            if (pos1 != std::string::npos) {
                std::string temp = str.substr(pos2, pos1 - pos2);
                pos1++;
                pos2 = pos1;
                parts.push_back(temp);
            }
            else if (pos2 != 0) {
                std::string temp = str.substr(pos2, str.size() - pos2);
                parts.push_back(temp);
            }
        }
        return parts;
    }

    char* GetLocalAddr(char* ip)
    {
        if (!ip)
            return NULL;

        struct hostent *hostinfo = NULL;
        char hostname[256] = { 0 };
        gethostname(hostname, sizeof(hostname));
        hostinfo = gethostbyname(hostname);
        if (!hostinfo)
            return NULL;

        std::vector<std::string> var1 = Split(ip, ".");
        for (int i = 0; i < hostinfo->h_length; i++) {
            struct in_addr addr;

#if(defined(_WIN32) || defined(_WIN64))
            addr.S_un.S_addr = *(u_long *)hostinfo->h_addr_list[i];
#else
            addr.s_addr = *(u_long *)hostinfo->h_addr_list[i];
#endif

            char* temp = inet_ntoa(addr);
            if (temp != NULL) {
                std::vector<std::string> var2 = Split(temp, ".");
                if (var2[0] == var1[0]
                    && var2[1] == var1[1]
                    && var2[2] == var1[2]) {
                    return temp;
                }
            }
        }

        return NULL;
    }

    bool GetAvaliablePort(int32_t &port)
    {
        bool result = true;

        SOCKET sock = socket(AF_INET, SOCK_STREAM, 0);

        struct sockaddr_in addr;
        addr.sin_family = AF_INET;
        addr.sin_addr.s_addr = htonl(INADDR_ANY);
        addr.sin_port = 0;

        int ret = bind(sock, (SOCKADDR*)&addr, sizeof(addr));

        if (0 != ret) {
            result = false;
            goto END;
        }

        struct sockaddr_in connAddr;
        int len = sizeof connAddr;
        ret = getsockname(sock, (SOCKADDR*)&connAddr, &len);

        if (0 != ret) {
            result = false;
            goto END;
        }

        port = ntohs(connAddr.sin_port); // 获取端口号

    END:
        if (0 != closesocket(sock))
            result = false;

        return result;
    }

    // inet_aton将 点分十进制串cp 转换为一个网络字节顺序的32位整数 IP地址
    // 例如将串cp "192.168.33.123 "
    // 转为 1100 0000   1010 1000    0010 0001     0111 1011 
    // 成功返回1，出错返回0
    // 转换成功，结果保存在结构体指针ap中
    bool GetHostByte(const char *ip, unsigned long* out)
    {
        int32_t dots = 0;
        register u_long acc = 0, addr = 0;

        do {
            register char cc = *ip;
            switch (cc) {
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                acc = acc * 10 + (cc - '0');
                break;

            case '.':
                if (++dots > 3)
                    return false;
                // Fall through

            case '\0':
                if (acc > 255)
                    return false;

                addr = addr << 8 | acc; // 这句是精华,每次将当前值左移八位加上后面的值
                acc = 0;
                break;

            default:
                return false;
            }
        } while (*ip++);

        // Normalize the address 
        if (dots < 3)
            addr <<= 8 * (3 - dots);

        *out = addr;
        return true;
    }

    bool GetNetworkByte(const char *ip, unsigned long* out)
    {
        if (!GetHostByte(ip, out))
            return false;

        *out = htonl(*out);

        return true;
    }

    void Sleep(int32_t ms)
    {
#if(defined(_WIN32) || defined(_WIN64))
        ::Sleep(ms);
#else
        sleep(x/1000)
#endif
    }
}