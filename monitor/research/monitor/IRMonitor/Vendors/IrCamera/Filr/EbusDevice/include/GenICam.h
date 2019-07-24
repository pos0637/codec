#ifndef GENICAM_H
#define GENICAM_H

#include <string>
#include "GenApi/GenApi.h"

class Mutex;

namespace Gige
{
    class Gvcp;

    class MyGenICam
    {
    public:

        MyGenICam(Gvcp& gev);
        ~MyGenICam();

        void Clear();
        bool ReadXmlFile();
        bool GetAddress(const std::string& sKey, uint32_t* ret);
        //bool IsReadable(const std::string& sKey);
        //bool IsWriteable(const std::string& sKey);
        //void PrintNodes(bool bWithProperties = false) const;

    private:

        Mutex* mLock;
        Gvcp& mGvcp;
        GenApi::CNodeMapRef* mCam;
    };
}

#endif // GENICAM_H