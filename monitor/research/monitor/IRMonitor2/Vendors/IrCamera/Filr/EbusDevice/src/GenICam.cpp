#include <cstdlib>
#include <exception>
#include <sstream>
#include <fstream>
#include <vector>
#include "..\include\GigEVision.h"
#include "..\include\Gvcp.h"
#include "..\include\GenICam.h"
#include "..\include\Utils.h"
#include "..\include\Config.h"
#include "..\include\Tracker.h"
#include "..\include\mutex.h"

namespace Gige
{
    MyGenICam::MyGenICam(Gvcp& gev) : mGvcp(gev)
    {
        mLock = new Mutex();
        mCam = NULL;
    }

    MyGenICam::~MyGenICam()
    {
        Clear();

        if (mLock) {
            delete mLock;
            mLock = NULL;
        }
    }

    bool MyGenICam::ReadXmlFile()
    {
        Synchronized(mLock) {
            std::vector<uint8_t> data = mGvcp.ReadMemory(First_URL_Register, 512, 512);
            if (data.size() < 512)
                return false;

            std::string sTxt(data.begin(), data.end());
            std::vector<std::string> aParts = Utils::Split(sTxt, ";");
            if (aParts.size() != 3) {
                Tracker::Instance().Log("Error getting genicam file location on camera");
                return false;
            }

            int32_t nAddr, nSize;
            std::stringstream ss;
            ss << std::hex << aParts[1];
            ss >> nAddr;
            ss.clear();
            ss << std::hex << aParts[2];
            ss >> nSize;

            data = mGvcp.ReadMemory(nAddr, 536, nSize);
            if (data.size() < (size_t)nSize)
                return false;

            if (mCam) {
                delete mCam;
                mCam = NULL;
            }

            try {
                mCam = new GenApi::CNodeMapRef();
                mCam->_LoadXMLFromZIPData(&data[0], data.size());
                return true;
            }
            catch (std::exception& e) {
                std::cout << e.what() << std::endl;
                if (mCam) {
                    delete mCam;
                    mCam = NULL;
                }
                return false;
            }
            finally {
                data.clear();
            }
        }
    }

    bool MyGenICam::GetAddress(const std::string& sKey, uint32_t* ret)
    {
        Synchronized(mLock) {
            if (mCam == NULL)
                return false;

            GenApi::INode* pN = mCam->_GetNode(sKey.c_str());
            uint32_t nAddr = 0;
            if (pN != 0) {
                GenICam::gcstring val, att;
                if (pN->GetProperty("Address", val, att)) {
                    nAddr = std::atoi(val.c_str());
                }
                if (pN->GetProperty("pAddress", val, att) && val.length() > 0) {
                    std::string sTmp(val);
                    std::vector<std::string> parts = Utils::Split(sTmp, "\t");

                    for (size_t i = 0; i < parts.size(); ++i) {
                        GenApi::INode* pAddrNode = mCam->_GetNode(parts[i].c_str());
                        if (pAddrNode) {
                            GenICam::gcstring sAddr, sAtt;
                            if (pAddrNode->GetProperty("Value", sAddr, sAtt))
                                nAddr += std::atoi(sAddr.c_str());
                        }
                    }
                }
                *ret = nAddr;
                return true;
            }

            return false;
        }
    }

    void MyGenICam::Clear()
    {
        Synchronized(mLock) {
            if (mCam != NULL) {
                delete mCam;
                mCam = NULL;
            }
        }
    }
}

//
//bool GenICamManager::IsReadable(const std::string& sKey)
//{
//    GenApi::INode* pN = m_cam._GetNode(sKey.c_str());
//    if (pN != 0)
//    {
//        GenICam::gcstring val, att;
//
//        if (pN->GetProperty("AccessMode", val, att))
//        {
//            if (!strcmp(val.c_str(), "RO") || !strcmp(val.c_str(), "RW"))
//                return true;  
//            else
//                return false;
//        }
//    }
//    else
//        return false;
//
//}
//
//bool GenICamManager::IsWriteable(const std::string& sKey)
//{
//    GenApi::INode* pN = m_cam._GetNode(sKey.c_str());
//    if (pN != 0)
//    {
//        GenICam::gcstring val, att;
//        GenApi::EAccessMode mode;
//        if (pN->GetProperty("AccessMode", val, att))
//        {
//            if (!strcmp(val.c_str(), "WO") || !strcmp(val.c_str(), "RW"))
//                return true;
//            else
//                return false;
//        }
//    }
//    else
//        return false;
//}
//
//void GenICamManager::PrintNodes(bool bWithProperties) const
//{
//    GenApi::NodeList_t nodes;
//    m_cam._GetNodes(nodes);
//
//    for(GenApi::NodeList_t::const_iterator it = nodes.begin(); it!=nodes.end(); ++it)
//    {
//        std::cout << "Node: " << (*it)->GetName(true) << std::endl;
//        if(bWithProperties)
//        {
//            GenICam::gcstring_vector props;
//            (*it)->GetPropertyNames(props);
//            for(GenICam::gcstring_vector::const_iterator pit = props.begin(); pit!=props.end(); ++pit)
//            std::cout << "\tProp: " << *pit << std::endl;
//        }
//    }
//}


