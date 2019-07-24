#ifndef COMMON_H
#define COMMON_H

#include <iostream>
#include <vector>
#include <stdint.h>

namespace Gige
{
    std::vector<std::string> Split(std::string str, const char* delim);
    bool GetHostByte(const char *ip, unsigned long* out);
    bool GetNetworkByte(const char *ip, unsigned long* out);
    void Sleep(int32_t ms);

    // 获取可用的Port口
    bool GetAvaliablePort(int32_t &port);

    // 获取本地IP地址
    char* GetLocalAddr(char* ip);
}

#endif