
#ifndef __TRACKER_H__
#define __TRACKER_H__

#include <stdio.h>
#include <time.h>
#include "mutex.h"

//#define SINGLE_LOG

class Tracker
{

public:

    // 默认日志保存在当前运行程序目录下的Log目录;
    // 否则,Log目录下的自定义目录
    static Tracker* GetTracker(char* dirName = NULL);

    static Tracker& Instance()
    {
        return *mTracker;
    }

    static void DestoryTracker();

    void Log(char* log);

private:

    Tracker(char* dirName);

    ~Tracker();

    Tracker(const Tracker&);

    int32_t CreatDir();

    int32_t CreateLogFile();

public:


private:
    static Tracker * mTracker;

    static Mutex mMutex;

    FILE * mFile;

    char mDirectoryPath[MAX_PATH];

    char mLogFileName[MAX_PATH];

    struct tm mCurrentDate;
};

#endif // __TRACKER_H__
