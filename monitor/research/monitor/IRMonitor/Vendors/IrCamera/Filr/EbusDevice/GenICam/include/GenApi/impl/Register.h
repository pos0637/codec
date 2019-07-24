//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenApi
//  Author:  Fritz Dierks
//  $Header$
//
//  License: This file is published under the license of the EMVA GenICam  Standard Group.
//  A text file describing the legal terms is included in  your installation as 'GenICam_license.pdf'.
//  If for some reason you are missing  this file please contact the EMVA or visit the website
//  (http://www.genicam.org) for a full copy.
//
//  THIS SOFTWARE IS PROVIDED BY THE EMVA GENICAM STANDARD GROUP "AS IS"
//  AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
//  THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
//  PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE EMVA GENICAM STANDARD  GROUP
//  OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,  SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT  LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,  DATA, OR PROFITS;
//  OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY  THEORY OF LIABILITY,
//  WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT  (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING IN ANY WAY OUT OF THE USE  OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//-----------------------------------------------------------------------------
/*!
\file
\brief    Definition of CRegister
\ingroup GenApi_Implementation
*/

#ifndef GENAPI_REGISTER_H
#define GENAPI_REGISTER_H

#include "NodeT.h"
#include "RegisterT.h"
#include "BaseT.h"
#include "ValueT.h"
#include "Port.h"
#include "NodeMacros.h"
#include "PrivateTypes.h"
#include "Node.h"
#include "../IInteger.h"
#include "../IEnumeration.h"

#ifdef _MSC_VER // *JS*
#pragma warning(push)
#pragma warning(disable: 4275) // non dll-interface XXX used as base for  dll-interface class YYY
#pragma warning(disable: 4251) // class 'xxx' needs to have dll-interface to be used by clients of class 'yyy'
#endif

namespace GenApi
{

    //*************************************************************
    // CEnumInt class
    //*************************************************************

    //! Helper class making IInteger and IEnumeration look like IInteger
    class CEnumInt
    {
    public:
        //! Constructor
        CEnumInt() : m_pInteger(NULL), m_pEnumeration(NULL)
        {}

        //! Assignment
        void operator=( IBase *pT )
        {
            m_pInteger = dynamic_cast<IInteger*>(pT);
            m_pEnumeration = dynamic_cast<IEnumeration*>(pT);
        }

        // this method is currently not used
        #if 0
            //! Set a value to either bool or int
            void SetValue( int64_t Value )
            {
                if(m_pInteger)
                    m_pInteger->SetValue( Value );
                else if(m_pEnumeration)
                    m_pEnumeration->SetIntValue( Value );
                else
                    throw LOGICAL_ERROR_EXCEPTION_NODE( "Cannot dereference NULL pointer" );
            }
        #endif

        //! Get a value from either bool or int
        int64_t GetValue() const
        {
            if(m_pInteger)
                return m_pInteger->GetValue();
            #pragma BullseyeCoverage off
            else if(m_pEnumeration)
                return m_pEnumeration->GetIntValue();
            else
                throw LOGICAL_ERROR_EXCEPTION( "Cannot dereference NULL pointer" );
            #pragma BullseyeCoverage on
        }

    protected:
        //! holds the pointer if it is an IInteger
        IInteger *m_pInteger;

        //! holds the pointer if it is an IEnumeration
        IEnumeration *m_pEnumeration;
    };

    //! List of EnumInt-node references
    typedef std::list<IValue *> ValueList_t;

    //! List of EnumInt-node references
    typedef std::list<CEnumInt> EnumIntList_t;


    //*************************************************************
    // CRegisterImpl class
    //*************************************************************
    /**
    * \ingroup internal_impl register
    * \brief Standard IRegister implementation
    *  Provides a chunk of memory which acts as a proxy to the register
    */
    class GENAPI_DECL CRegisterImpl
        : public IRegister
        , public CNodeImpl
    {
    public:
        //! Default Contructor
        CRegisterImpl();

    protected:
        //-------------------------------------------------------------
        // IValue implementation
        //-------------------------------------------------------------

        //! Get value of the node as string
        virtual GenICam::gcstring InternalToString(bool Verify = false, bool IgnoreCache = false);

        //! Set value of the node as string
        virtual void InternalFromString(const GenICam::gcstring& valueString, bool Verify = true);

    public:

        //! Implementation of IBase::GetPrincipalInterfaceType()
        virtual EInterfaceType InternalGetPrincipalInterfaceType() const
        {
            return intfIRegister;
        }

        //-------------------------------------------------------------
        // INodePrivate implementation
        //-------------------------------------------------------------

        //! Initializes the object
        virtual void SetInvalid(INodePrivate::ESetInvalidMode simMode);

        //! finalizes the construction of the node
        virtual void FinalConstruct();

        //! checks if this node is terminal
        virtual bool IsTerminalNode() const
        {
            // Registers are terminal nodes per default
            return true;
        }

        //-------------------------------------------------------------
        // IBase implementation
        //-------------------------------------------------------------

    protected:
        //! Get the access mode of the node
        virtual EAccessMode InternalGetAccessMode() const;

        //-------------------------------------------------------------
        // IRegister implementation
        //-------------------------------------------------------------

        //! Set the register's contents
        virtual void InternalSet(const uint8_t *pBuffer, int64_t Length, bool Verify = true);

        //! Retrieves a pointer to a buffer containing the register's's contents
        virtual void InternalGet(uint8_t *ppBuffer, int64_t pLength, bool Verify = false, bool IgnoreCache = false) ;

        //! Retrieves the Length of the register [Bytes]
        virtual int64_t InternalGetLength();

        //! Retrieves the Address of the register
        virtual int64_t InternalGetAddress();

        //! Get Caching Mode
        virtual ECachingMode InternalGetCachingMode() const;

    public:
        //-------------------------------------------------------------
        // Initializing
        //-------------------------------------------------------------

        // See comment on the CONVERT_AMO of CNodeImpl

        BEGIN_CONVERT_MAP
            CHAIN_CONVERT_MAP(CNodeImpl)
            SWITCH_CONVERT_MAP
            CONVERT_ENUM_ENTRY(AccessMode_ID, m_AccessMode, EAccessModeClass)
            CONVERT_NODE_REFERENCE(pPort_ID, m_pPort, CPort)
            CONVERT_ENTRY(Length_ID, m_Length)
            CONVERT_LIST_ENTRY(Address_ID, m_AddressValues, int64_t)
            CONVERT_NODE_REFERENCE(pLength_ID, m_Length, IBase)
            CONVERT_NODELIST_REFERENCE(pAddress_ID, m_AddressPointers, IValue, ValueList_t)
            CONVERT_NODELIST_REFERENCE_AUXREF(pIndex_ID, m_IndexPointers, IInteger, m_IndexOffsetPointer, IInteger)
        END_CONVERT_MAP

    protected:
        //! Recomputes the Address
        void UpdateAddress();

        //-------------------------------------------------------------
        // Member variables
        //-------------------------------------------------------------

        //! pointer to Port node providing access to the port
        CPort *m_pPort;

        //! Address of the register
        int64_t m_Address;

        //! TRue if m_Address is valid
        int64_t m_AddressValid;

        //! Length of the register
        CIntegerPolyRef m_Length;

        //! List of addresses/offsets
        Int64List_t m_AddressValues;

        //! List of node references giving addresses/offsets
        ValueList_t m_AddressPointers;

        //! List of node references giving addresses/offsets
        EnumIntList_t m__AddressPointers;

        //! List of node references giving indices/offsets
        IntegerList_t m_IndexPointers;

        //! List of node references giving indices/offsets
        IntegerList_t m_IndexOffsetPointer;

        //! the access mode
        EAccessMode m_AccessMode;
    };



class GENAPI_DECL CRegister
    : public NodeT< RegisterT < ValueT < BaseT < CRegisterImpl > > > >
{};


}

#ifdef _MSC_VER // *JS*
#pragma warning ( pop )
#endif

#endif // ifndef GENAPI_REGISTER_H
