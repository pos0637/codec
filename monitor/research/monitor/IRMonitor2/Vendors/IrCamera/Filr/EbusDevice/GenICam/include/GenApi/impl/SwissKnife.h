//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenApi
//  Author:  Margret Albrecht
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
\brief    Definition  of CSwissKnife
\ingroup GenApi_Implementation
*/

#ifndef GENAPI_SWISSKNIFE_H
#define GENAPI_SWISSKNIFE_H

#include "../IFloat.h"
#include "Node.h"
#include "MathParser/MathParser.h"
#include "BaseT.h"
#include "ValueT.h"
#include "NodeT.h"
#include "FloatT.h"

#pragma warning ( push )
#pragma warning ( disable : 4275 ) // non dll-interface XXX used as base for  dll-interface class YYY

namespace GenApi
{
    //*************************************************************
    // CSwissKnife class
    //*************************************************************

    /**
    * \ingroup internal_impl
    *
    * \brief  Specialized SwissKnife for float nodes
    *
    * Used for formula evaluation with ToPhysical
    * and FromPhysical childs
    */
    class GENAPI_DECL CSwissKnifeImpl : public IFloat,  public CNodeImpl
    {
    public:
        //! Constructor
        CSwissKnifeImpl();

        //! Destructor
        virtual ~CSwissKnifeImpl();

    protected:
        //-------------------------------------------------------------
        // IBase implementation
        //-------------------------------------------------------------

        // Get access mode
        virtual EAccessMode InternalGetAccessMode() const;

        //! Implementation of IBase::GetPrincipalInterfaceType()
        virtual EInterfaceType InternalGetPrincipalInterfaceType() const
        {
            return intfIFloat;
        }


    public:
        //-------------------------------------------------------------
        // IInteger implementation
        //-------------------------------------------------------------

        //! Get feature value using m_InputName as hard coded variable name
        /*! This is a helper for the implementation of the converter */
        virtual double GetValueWithInput(double input, bool Verify = false, bool IgnoreCache = false);

        //-------------------------------------------------------------
        // INodePrivate implementation
        //-------------------------------------------------------------
        virtual void FinalConstruct();

    protected:
        //-------------------------------------------------------------
        // IFloat implementation
        //-------------------------------------------------------------

        // Set feature value
        #pragma BullseyeCoverage off
        virtual void InternalSetValue(double /*Value*/, bool /*Verify*/ )
        {
            throw LOGICAL_ERROR_EXCEPTION_NODE("SwissKnife : %s SetValue failed. SwissKnife is read only", m_Name.c_str() );
        }
        #pragma BullseyeCoverage on

        // Get feature value
        virtual double InternalGetValue(bool Verify = false, bool IgnoreCache = false);

        // Get minimum value allowed
        virtual double InternalGetMin()
        {
            return m_Min;
        }

        // Get maximum value allowed
        virtual double InternalGetMax()
        {
            return m_Max;
        }

        //! True if the float has a constant increment
        virtual bool InternalHasInc()
        {
            // a swiss knife does not know about its increments
            return false;
        }

        //! Get the constant increment if there is any
        #pragma BullseyeCoverage off
        // (untestable, function never called)
        virtual double InternalGetInc()
        {
            assert(false);
            return -1.0;
        }
        #pragma BullseyeCoverage on

        //! Get list of valid value
        const virtual double_autovector_t InternalGetListOfValidValues()
        {
            return double_autovector_t();
        }


        // Get recommended representation
        virtual  ERepresentation InternalGetRepresentation()
        {
            if( m_Representation != _UndefinedRepresentation )
                return m_Representation;
            else
                return PureNumber;
        }

        // Get unit
        virtual GenICam::gcstring InternalGetUnit() const
        {
            return m_Unit;
        }

        //! Get the way the float should be converted to a string
        virtual EDisplayNotation InternalGetDisplayNotation() const
        {
            return m_DisplayNotation;
        }

        //! Get the precision to be used when converting the float to a string
        virtual int64_t InternalGetDisplayPrecision() const
        {
            return m_DisplayPrecision;
        }

        //! Get Caching Mode
        virtual ECachingMode InternalGetCachingMode() const;

    public:
        //-------------------------------------------------------------
        // Initializing
        //-------------------------------------------------------------

        //! Convert map
        BEGIN_CONVERT_MAP
            CHAIN_CONVERT_MAP(CNodeImpl)
            SWITCH_CONVERT_MAP
            CONVERT_STRING_ENTRY(Formula_ID, m_Formula)
            CONVERT_STRING_ENTRY(Input_ID, m_InputName)
            CONVERT_ENUM_ENTRY(Representation_ID, m_Representation, ERepresentationClass)
            CONVERT_STRING_ENTRY(Unit_ID, m_Unit)
            CONVERT_ENUM_ENTRY(DisplayNotation_ID, m_DisplayNotation, EDisplayNotationClass)
            CONVERT_ENTRY(DisplayPrecision_ID, m_DisplayPrecision)
            /**********************************************/
            // special treatment of variables
            /**********************************************/
            case pVariable_ID:
            {
                if (Direction == In)
                {
                    // store the symbolic name of the variable
                    m_Symbolics.insert(std::pair<GenICam::gcstring, GenICam::gcstring>(ValueStrIn, AttributeStrIn));

                    // prepare the variable pointer
                    INode * const pNode = m_pNodeMap->GetNode(ValueStrIn);
                    if(!pNode)
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn );
                    AddChild(pNode );

                    // store the pointer to the variables
                    CFloatPolyRef Value;
                    Value = dynamic_cast<IBase*>(pNode);
                    m_Variables.insert(std::pair<GenICam::gcstring, CFloatPolyRef>(AttributeStrIn, Value));
                    return true;
                }
                else if (Direction == Out)
                {
                    // create a list of vlaues and a list of indeces
                    std::map<GenICam::gcstring, GenICam::gcstring>::iterator ptr;
                    for(ptr=m_Symbolics.begin(); ptr!=m_Symbolics.end(); ptr++ )
                    {
                        ValueStrOut += (*ptr).first + "\t";
                        AttributeStrOut += (*ptr).second + "\t";
                    }

                    // get rid of the respective last tabs of the lists
                    if(ValueStrOut.size() > 1)
                    {
                        AttributeStrOut = AttributeStrOut.substr(0, AttributeStrOut.size()-1);
                        ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1);
                    }

                    return true;
                }
            }
            break;
            /**********************************************/
        END_CONVERT_MAP

    protected:

        //-------------------------------------------------------------
        // Member variables
        //-------------------------------------------------------------

        //! minimum value to be stored in the Register
        double  m_Min;

        //! maximum value to be stored in the Register
        double  m_Max;

        //! the formula evaluated by the swiss knife
        GenICam::gcstring m_Formula;

        //! Mapping of the variable's node names to the SYMBOLIC names in the folmulas
        std::map<GenICam::gcstring, GenICam::gcstring> m_Symbolics;

        //! Mapping of SYMBOLIC names to the references of the variables
        std::map<GenICam::gcstring, CFloatPolyRef> m_Variables;

        //! the parser doing the actual work
        CMathParser m_MathParser;

        //! the node's representation
        ERepresentation m_Representation;

        //! the physical unit name
        GenICam::gcstring m_Unit;

        //! the printf format specifier used to convert the float number to a string
        EDisplayNotation m_DisplayNotation;

        //! the precision the float is converted with to a string
        int64_t m_DisplayPrecision;

        //! A hard coded variable name
        /*! This is a helper for the implementation of the converter */
        GenICam::gcstring m_InputName;

    };

    class CSwissKnife : public BaseT< ValueT< NodeT< FloatT<  CSwissKnifeImpl > > > >
    {
    };

}

#pragma warning ( pop )

#endif // GENAPI_SWISSKNIFE_H
