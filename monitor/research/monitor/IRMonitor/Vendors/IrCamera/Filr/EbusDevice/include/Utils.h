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
        // �ַ����ָ�
        static std::vector<std::string> Split(std::string str, const char* delim);

        // ��ȡ�����ֽ���
        static bool GetHostByte(const char *ip, unsigned long* out);

        // ��ȡ�����ֽ���
        static bool GetNetworkByte(const char *ip, unsigned long* out);

        // ˯��
        static void Sleep(int32_t ms);

        // ��ȡ���õ�Port��
        static bool GetAvaliablePort(int32_t &port);

        // ��ȡ����IP��ַ
        static char* GetLocalAddr(char* ip);
    };
}

#endif