#ifndef UTIL_H_
#define UTIL_H_

#include <iostream>
#include <vector>
#include <stdint.h>

#if defined(WIN32) || defined(_WIN32)
#define OS_WINDOWS
#elif defined(__APPLE__) || defined(APPLE)
#define OS_UNIX
#elif defined(__linux__) || defined(linux)
#define OS_LINUX
#endif

namespace Gige
{
    class Utils {
    public:
        // 字符串分割
        static std::vector<std::string> Split(std::string str, const char* delim);

        // 获取主机字节序
        static bool GetHostByte(const char *ip, unsigned long* out);

        // 获取网络字节序
        static bool GetNetworkByte(const char *ip, unsigned long* out);

        // 睡眠
        static void Sleep(int32_t ms);

        // 获取可用的Port口
        static bool GetAvaliablePort(int32_t &port);

        // 获取本地IP地址
        static char* GetLocalAddr(char* ip);
    };
}

#endif