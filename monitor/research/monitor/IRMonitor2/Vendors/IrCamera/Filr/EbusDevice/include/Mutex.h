#ifndef __MUTEX_H__
#define __MUTEX_H__

#include "../include/Utils.h"

#ifdef OS_WINDOWS
#include <Windows.h>

    class Mutex
    {
    public:

        Mutex()
        {
            ::InitializeCriticalSection(&m_cs);
        }

        ~Mutex()
        {
            ::DeleteCriticalSection(&m_cs);
        }

    public:

        void Lock()
        {
            ::EnterCriticalSection(&m_cs);
        }

        void Unlock()
        {
            ::LeaveCriticalSection(&m_cs);
        }

    private:

        CRITICAL_SECTION m_cs;
    };
#else
#include <pthread.h>

    class Mutex
    {
    public:

        Mutex()
        {
            pthread_mutex_init(&m_mutex,NULL);
        }

        ~Mutex()
        {
            pthread_mutex_destroy(&m_mutex);
        }

    public:

        void Lock()
        {
            pthread_mutex_lock(&m_mutex);
        }

        void Unlock()
        {
            pthread_mutex_unlock(&m_mutex);
        }

    private:

        pthread_mutex_t m_mutex;
    };
#endif

    class CAutoMutex
    {
    public:

        explicit CAutoMutex(
            Mutex * pCMutex)
        {
            m_pCMutex = pCMutex;
            m_pCMutex->Lock();
            m_bLocked = true;
        }

        ~CAutoMutex()
        {
            if ((m_bLocked) && (m_pCMutex))
                m_pCMutex->Unlock();
        }

        void Lock()
        {
            m_pCMutex->Lock();
        }

        void Unlock()
        {
            m_bLocked = false;
            if (m_pCMutex)
                m_pCMutex->Unlock();
        }

        bool IsLocked()
        {
            return m_bLocked;
        }

        bool Invalidate()
        {
            if ((m_bLocked) && (m_pCMutex))
                m_pCMutex->Unlock();

            m_pCMutex = NULL;
        }

    private:

        CAutoMutex()
        {
        }

    private:

        Mutex * m_pCMutex;
        bool m_bLocked;
    };

#define Synchronized(pMutex) \
    for (CAutoMutex lock((pMutex)); lock.IsLocked(); lock.Unlock())

#endif // __MUTEX_H__
