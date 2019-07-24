#ifndef __THREAD_H__
#define __THREAD_H__

#include "../include/Utils.h"

#ifdef OS_WINDOWS

    #include <Windows.h>
    class Thread
    {
    public:

        Thread()
        {
            mId = 0;
            mHandle = NULL;
            mRoutine = NULL;
        }

        Thread(LPVOID routine)
        {
            mId = 0;
            mHandle = NULL;
            mRoutine = (LPTHREAD_START_ROUTINE)routine;
        }

        virtual ~Thread()
        {
            if (mHandle) {
                CloseHandle(mHandle);
            }
        }

    public:

        bool Start(LPVOID routine, LPVOID param)
        {
            mRoutine = (LPTHREAD_START_ROUTINE)routine;

            return Start(param);
        }

        bool Start(LPVOID param)
        {
            WaitFor();

            mHandle = ::CreateThread(NULL, 0, mRoutine, param, 0, &mId);
            if (!mHandle)
                return false;

            return true;
        }

        bool WaitFor()
        {
            if (mHandle) {
                ::WaitForSingleObject(mHandle, INFINITE);
                ::CloseHandle(mHandle);
                mId = 0;
                mHandle = NULL;
            }

            return true;
        }

        DWORD GetId()
        {
            return mId;
        }

    private:

        DWORD mId;
        HANDLE mHandle;
        LPTHREAD_START_ROUTINE mRoutine;
    };

#else
#include <pthread.h>
    typedef void *(*start_routine)(void *);

    class Thread
    {
    public:

        Thread()
        {
            mHandle = 0;
            mRoutine = NULL;
        }

        Thread(start_routine routine)
        {
            mHandle = 0;
            mRoutine = routine;
        }

        virtual ~Thread()
        {
            if (mHandle) {
                close(mHandle);
            }
        }

    public:

        bool Start(start_routine routine, void* param)
        {
            mRoutine = routine;

            return Start(param);
        }

        bool Start(void* param)
        {
            WaitFor();

            pthread_create(&mHandle, NULL, mRoutine, param);

            if (!mHandle)
                return false;

            return true;
        }

        bool WaitFor()
        {
            if (mHandle) {
                //pthread_cancel(mHandle);
                pthread_join(mHandle, NULL);
                close(mHandle);
                mHandle = 0;
            }

            return true;
        }

        int32_t GetId()
        {
            return mHandle;
        }

    private:

        pthread_t mHandle;
        start_routine mRoutine;
    };

#endif

#endif // __THREAD_H__