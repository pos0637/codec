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
\brief    Definition of CCategory
\ingroup GenApi_Implementation
*/

#ifndef GENAPI_CATEGORY_H
#define GENAPI_CATEGORY_H

#include "../ICategory.h"
#include "Node.h"
#include "NodeMacros.h"
#include "BaseT.h"
#include "ValueT.h"
#include "NodeT.h"

#pragma warning ( push )
#pragma warning ( disable : 4275 ) // non dll-interface XXX used as base for  dll-interface class YYY

namespace GenApi
{

    //*************************************************************
    // CCategory class
    //*************************************************************

    /**
    * \ingroup internal_impl
    *
    * \brief Holds a list of features and sub-categories
    */
    class GENAPI_DECL CCategoryImpl : public CNodeImpl, public ICategory
    {
    public:

        //! Constructor
        CCategoryImpl();

        //-------------------------------------------------------------
        // IValue implementation (partial only)
        //-------------------------------------------------------------

    protected:
        //! Get the INode interface of the node
        virtual INode* InternalGetNode();

        //! Get display name of the node as string
        virtual GenICam::gcstring InternalToString(bool /*Verify = false*/, bool /*IgnoreCache = false*/)
        {
            return InternalGetNode()->GetDisplayName();
        }

        //! Set content of the node as string
        #pragma BullseyeCoverage off
        virtual void InternalFromString(const GenICam::gcstring& /*ValueStr*/, bool /*Verify*/ )
        {
            // Categories are read-only
            assert(false);
        }
        #pragma BullseyeCoverage on

    public:
        //-------------------------------------------------------------
        // ICategory implementation
        //-------------------------------------------------------------

        //! Get all features of the category (including sub-categories)
        virtual void GetFeatures(FeatureList_t &Features) const;

        //-------------------------------------------------------------
        // Initializing
        //-------------------------------------------------------------

        BEGIN_CONVERT_MAP
            CHAIN_CONVERT_MAP(CNodeImpl)
            SWITCH_CONVERT_MAP
            CONVERT_NODELIST_REFERENCE(pFeature_ID, m_Features, IValue, FeatureList_t)
        END_CONVERT_MAP

        virtual void FinalConstruct();

        //! Returns the current access mode of the node
        virtual EAccessMode InternalGetAccessMode() const;

        //! Implementation of IBase::GetPrincipalInterfaceType()
        virtual EInterfaceType InternalGetPrincipalInterfaceType() const
        {
            return intfICategory;
        }

        void SetInvalid(ESetInvalidMode simMode);

    };

    class GENAPI_DECL CCategory : public BaseT< ValueT< NodeT< CCategoryImpl > > >
    {
    };

}
#pragma warning ( pop )

#endif // ifndef GENAPI_CATEGORY_H
