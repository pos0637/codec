//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenApi
//  Author:  Hartmut Nebelung
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
\brief    Definition of the CConverter class
\ingroup GenApi_Implementation
*/

#ifndef GENAPI_INTCONVERTER_H
#define GENAPI_INTCONVERTER_H

#include "Base/GCString.h"
#include "../IInteger.h"
#include "Node.h"
#include "BaseT.h"
#include "ValueT.h"
#include "NodeT.h"
#include "IntSwissKnife.h"
#include "IntegerT.h"
#include "MathParser/Int64MathParser.h"
#include "PolyReference.h"


#pragma warning ( push )
#pragma warning ( disable : 4275 ) // non dll-interface XXX used as base for  dll-interface class YYY

namespace GenApi
{
    //*************************************************************
    // CIntConverter class
    //*************************************************************

    //! IInteger implementation with integrated conversion
    /*! Works like a Integer, but has integrated conversion formulas
        by which the values are converted before writing
        and after reading.

    The Representation may be defined using one of the values in ERepresentation.
    The default is _UndefinedRepresentation.
    */
    class GENAPI_DECL CIntConverterImpl : public  IInteger, public CNodeImpl
    {
    public:
        //! Constructor
        CIntConverterImpl();
    protected:

        //-------------------------------------------------------------
        // IInteger implementation
        //-------------------------------------------------------------

        // Set feature value
        virtual void InternalSetValue(int64_t Value, bool Verify = true);

        // Get feature value
        virtual int64_t InternalGetValue(bool Verify = false, bool IgnoreCache = false);

        // Get minimum value allowed
        virtual int64_t InternalGetMin();

        // Get maximum value allowed
        virtual int64_t InternalGetMax();

        // Get increment
        virtual int64_t InternalGetInc();

        //! Get list of valid value
        const int64_autovector_t InternalGetListOfValidValues();

        // Get recommended representation
        virtual  ERepresentation InternalGetRepresentation() const
        {
            if (m_Representation != _UndefinedRepresentation)
                return m_Representation;
            else
                return m_Value.GetRepresentation();

        }

        // Get the access mode of the node
        virtual EAccessMode InternalGetAccessMode() const;

        //! Implementation of IBase::GetPrincipalInterfaceType()
        virtual EInterfaceType InternalGetPrincipalInterfaceType() const
        {
            return intfIInteger;
        }

        //! Get the unit
        virtual GenICam::gcstring InternalGetUnit()
        {
            if( m_Unit.empty() )
                return m_Value.GetUnit();

            return m_Unit;
        }

        //! Get Caching Mode
        virtual ECachingMode InternalGetCachingMode() const;

    public:
        //! Checks if the node is temrina
        virtual bool IsTerminalNode() const
        {
            return !m_Value.IsPointer();
        }

        //! finalises construction
        virtual void FinalConstruct();

        //-------------------------------------------------------------
        // Initializing
        //-------------------------------------------------------------
    public:
        BEGIN_CONVERT_MAP
            CHAIN_CONVERT_MAP(CNodeImpl)
            SWITCH_CONVERT_MAP

            // forwarding the entries to the right swiss knife
            case FormulaTo_ID:
            {
                if (Direction == In)
                    return m_pConvertTo->SetProperty("Formula", ValueStrIn, AttributeStrIn);
                else if (Direction == Out)
                    return m_pConvertTo->GetProperty("Formula", ValueStrOut, AttributeStrOut);
            }
            break;

            case FormulaFrom_ID:
            {
                if (Direction == In)
                    return m_pConvertFrom->SetProperty("Formula", ValueStrIn, AttributeStrIn);
                else if (Direction == Out)
                    return m_pConvertFrom->GetProperty("Formula", ValueStrOut, AttributeStrOut);
            }
            break;

            // forwarding the entry to both swissknife
            case pVariable_ID:
            {
                if (Direction == In)
                {
                    m_pConvertFrom->SetProperty(PropertyName, ValueStrIn, AttributeStrIn);
                    m_pConvertTo->SetProperty(PropertyName, ValueStrIn, AttributeStrIn);
                    return true;
                }
                else if (Direction == Out)
                {
                    m_pConvertFrom->GetProperty(PropertyName, ValueStrOut, AttributeStrOut);
                    m_pConvertTo->GetProperty(PropertyName, ValueStrOut, AttributeStrOut);
                    return true;
                }
            }
            break;

            CONVERT_NODE_REFERENCE(pValue_ID, m_Value, IBase)
            CONVERT_ENUM_ENTRY(Representation_ID, m_Representation, ERepresentationClass)
            CONVERT_STRING_ENTRY(Unit_ID, m_Unit)
            CONVERT_ENUM_ENTRY(Slope_ID, m_Slope, ESlopeClass)
        END_CONVERT_MAP


        // Registers the node
        virtual void Register(GenApi::INodeMapPrivate* const pNodeMap, const char *pNodeType, const char *pName, const char *pNameSpace);

        // determine if the conversion function is increasing or decreasing
        void CheckIncreasing();


    private:
        //! Checks if <Min> has ben set
        bool IsMinUninititialized() const;

        //! Checks if <Max> has ben set
        bool IsMaxUninitialized() const;

        //-------------------------------------------------------------
        // Member variables
        //-------------------------------------------------------------

        //! The SwissKnife formula for Set
        GenICam::gcstring m_FormulaTo;

        //! The SwissKnife formula for Get
        GenICam::gcstring m_FormulaFrom;

        //! The Name of the external Variable
        GenICam::gcstring m_InputName;

        //! The Swiss Knife for Set
        CIntSwissKnife *m_pConvertTo;

        //! The Swiss Knife for Get
        CIntSwissKnife *m_pConvertFrom;

        //! Refeerence to the the value
        CIntegerPolyRef m_Value;

        //! recommended representation of the value
        mutable ERepresentation m_Representation;

        //! the physical unit name
        GenICam::gcstring m_Unit;

        //! indicates if the formula is strictly monotonous increasing or decreating
        ESlope m_Slope;
    };

    //! class implementingthe converter object
    class GENAPI_DECL CIntConverter
        : public BaseT< ValueT< NodeT < IntegerT < CIntConverterImpl > > > >
    {
    };
}

#pragma warning ( pop )

#endif
