
#include <direct.h>
#include <io.h>
#include "../include/Tracker.h"

#define ACCESS _access
#define MKDIR(a) _mkdir((a))

Tracker * Tracker::mTracker = NULL;

Mutex Tracker::mMutex;

Tracker::Tracker(char* dirName)
{
    mFile = NULL;

    mCurrentDate.tm_year = 0;
    mCurrentDate.tm_mon = 0;
    mCurrentDate.tm_mday = 0;

    char path[MAX_PATH];
    GetModuleFileNameA(NULL, path, MAX_PATH);
    if (dirName)
        sprintf(mDirectoryPath, "%s\\..\\Log\\%s", path, dirName);
    else
        sprintf(mDirectoryPath, "%s\\..\\Log", path);
}

Tracker::~Tracker()
{
    if (mFile != NULL)
        fclose(mFile);
}

Tracker * Tracker::GetTracker(char* dirName)
{
    if (NULL == mTracker) {
        Synchronized(&mMutex) {
            if (NULL == mTracker) {
                mTracker = new Tracker(dirName);
                int32_t ret = mTracker->CreatDir();
                if (ret < 0) {
                    delete mTracker;
                    mTracker = NULL;
                    return NULL;
                }
            }
        }
    }

    return mTracker;
}

void Tracker::DestoryTracker()
{
    Synchronized(&mMutex) {
        if (NULL != mTracker) {
            delete mTracker;
            mTracker = NULL ;
        }
    }
}

int32_t Tracker::CreateLogFile()
{
    int32_t iRet = 0;
    time_t curTime = time(NULL);
    struct tm *mt = localtime(&curTime);
    if (mt == NULL)
        return -1;

    if (mCurrentDate.tm_year != mt->tm_year ||
        mCurrentDate.tm_mon != mt->tm_mon ||
        mCurrentDate.tm_mday != mt->tm_mday) {
            mCurrentDate.tm_year = mt->tm_year;
            mCurrentDate.tm_mon = mt->tm_mon;
            mCurrentDate.tm_mday = mt->tm_mday;

            if (mFile) {
                fclose(mFile);
                mFile = NULL;
            }

#ifndef SINGLE_LOG
            // 根据日期组成文件名称
            sprintf(mLogFileName, "%s\\%d%02d%02d.log", mDirectoryPath, mt->tm_year + 1900, mt->tm_mon + 1, mt->tm_mday);
            mFile = fopen(mLogFileName, "a+");
            if (!mFile)
                iRet = -1;
#else
            sprintf(mLogFileName, "%s\\tracker.log", mDirectoryPath);
            mFile = fopen(mLogFileName, "w");
            if (!mFile)
                iRet = -1;
#endif

    }

    return iRet;
}

int32_t Tracker::CreatDir()
{
    int32_t i = 0;
    int32_t iRet;
    int32_t iLen;
    char pszDir[MAX_PATH];

    memcpy(pszDir, mDirectoryPath, MAX_PATH);
    iLen = strlen(pszDir);

    // 创建中间目录
    for(i = 0;i < iLen;i ++) {
        if (pszDir[i] == '\\' || pszDir[i] == '/') {
            pszDir[i] = '\0';

            // 如果不存在,创建
            iRet = ACCESS(pszDir,0);
            if (iRet != 0) {
                iRet = MKDIR(pszDir);
                if (iRet != 0)
                    return -1;
            }

            pszDir[i] = '\\';
        } 
    }

    iRet = ACCESS(pszDir,0);
    if (iRet != 0)
        iRet = MKDIR(pszDir);

    return iRet;
}

void Tracker::Log(char* log)
{
    if (!log)
        return;

    char logText[MAX_PATH];
    time_t curTime = time(NULL);
    struct tm *mt = localtime(&curTime);
    if (!mt)
        return;

    int32_t retLength = sprintf(logText,
        "[%04d-%02d-%02d %02d:%02d:%02d] %s\n",
        mt->tm_year + 1900,
        mt->tm_mon + 1,
        mt->tm_mday,
        mt->tm_hour,
        mt->tm_min,
        mt->tm_sec,
        log);

    Synchronized(&mMutex) {
        int32_t ret = CreateLogFile();
        if (ret < 0)
            return;

        fwrite(logText, retLength, 1, mFile);
        fflush(mFile);
    }
}