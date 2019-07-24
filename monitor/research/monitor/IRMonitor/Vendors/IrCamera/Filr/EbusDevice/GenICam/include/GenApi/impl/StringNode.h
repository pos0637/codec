
//-----------------------------------------------------------------------------
//  (c) 2008 by Basler Vision Technologies
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
\brief    Interface to the CStringRegister class.
\ingroup GenApi_Implementation
*/

#ifndef GENAPI_STRINGNODE_H
#define GENAPI_STRINGNODE_H

#if _MSC_VER>1000
    #pragma once
#endif // _MSC_VER>1000

#include "GenApi/Types.h"
#include "GenApi/IString.h"
#include "GenApi/impl/BaseT.h"
#include "GenApi/impl/ValueT.h"
#include "GenApi/impl/NodeT.h"
#include "GenApi/impl/StringT.h"
#include "GenApi/impl/Node.h"
#include "PolyReference.h"

#pragma warning ( push )
#pragma warning ( disable : 4275 ) // non dll-interface XXX used as base for  dll-interface class YYY

//! Part of the generic device API
namespace GenApi
{
    // *************************************************************
    // CStringNode class
    // *************************************************************
    //! Implements a floating string node

    class GENAPI_DECL CStringNodeImpl : public CNodeImpl, public IString
    {
    public:
        CStringNodeImpl(void);


        //-------------------------------------------------------------
        // Implementation of INodePrivate
        //-------------------------------------------------------------
        //! \name INodePrivate implementation
        //\{
        virtual void FinalConstruct();
        //! Get Caching Mode
        virtual EAccessMode InternalGetAccessMode() const;
        //@}


        //! Implementation of IBase::GetPrincipalInterfaceType()
        virtual EInterfaceType InternalGetPrincipalInterfaceType() const
        {
            return intfIString;
        }

        //! Indicates if the node is a terminal node
        virtual bool IsTerminalNode() const ;

    protected:
        //-------------------------------------------------------------
        // Implementation of IValue
        //-------------------------------------------------------------
        //! \name IValue implementation
        //\{
        virtual GenICam::gcstring InternalToString(bool Verify = false, bool IgnoreCache = false);

        //! Set value of the node as string
        virtual void InternalFromString(const GenICam::gcstring& valueString, bool Verify = true);

        //! Checks if the value comes from cache or is requested from another node
        virtual bool InternalIsValueCacheValid() const;
        //@}


        //\}

        //-------------------------------------------------------------
        // Implementation of IString
        //-------------------------------------------------------------
        //! \name IString implementation
        //\{

        // Set node value
        virtual void InternalSetValue(const GenICam::gcstring& Value, bool Verify = true);

        // Get node value
        virtual GenICam::gcstring InternalGetValue(bool Verify = false, bool IgnoreCache = false);
        
        //! Retrieves the maximum length of the string in bytes
        virtual int64_t InternalGetMaxLength();

        //\}

    public:
        //-------------------------------------------------------------
        // Initializing
        //-------------------------------------------------------------
        BEGIN_CONVERT_MAP
            CHAIN_CONVERT_MAP(CNodeImpl)
            SWITCH_CONVERT_MAP
            CONVERT_NODE_REFERENCE(pValue_ID, m_Value, IBase)
            CONVERT_ENTRY(Value_ID, m_Value)
        END_CONVERT_MAP

    private:
        //! The Value of the floating node
        CStringPolyRef m_Value;
    };


    class GENAPI_DECL CStringNode : public BaseT< ValueT< NodeT< StringT< CStringNodeImpl > > > >
    {
    };

}

#pragma warning ( pop )

#endif // GENAPI_STRINGNODE_H
