//-----------------------------------------------------------------------------
//  (c) 2006-8 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenApi
//  Author:  Hartmut Nebelung, Fritz Dierks
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
    \ingroup GenApi_Implementation
*/

#ifndef GENAPI_FLOAT_H
#define GENAPI_FLOAT_H

#include "Base/GCString.h"
#include "../IFloat.h"
#include "Node.h"
#include "BaseT.h"
#include "ValueT.h"
#include "NodeT.h"
#include "FloatT.h"
#include <map>

#pragma warning ( push )
#pragma warning ( disable : 4275 ) // non dll-interface XXX used as base for  dll-interface class YYY

namespace GenApi
{

    /*! \brief Core pf the Float node implementation
        \ingroup GenApi_Implementation
    */
    class GENAPI_DECL CFloatImpl : public IFloat, public CNodeImpl
    {
    public:
        //-------------------------------------------------------------
        //! \name Constructor / destructor
        //@{
            //! Constructor
            CFloatImpl();
        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Interface IBase (methods are called by the BaseT class)
        //@{
            //! Get the access mode of the node
            virtual EAccessMode InternalGetAccessMode() const;

            //! Implementation of IBase::GetPrincipalInterfaceType()
            virtual EInterfaceType InternalGetPrincipalInterfaceType() const
            {
                return intfIFloat;
            }
        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Interface IFloat (methods are called by the FloatT class)
        //@{
            //! Set feature value
            virtual void InternalSetValue(double Value, bool Verify = true);

            //! Get feature value
            virtual double InternalGetValue(bool Verify = false, bool IgnoreCache = false) const;

            //! Get minimum value allowed
            virtual double InternalGetMin() const;

            //! Get maximum value allowed
            virtual double InternalGetMax() const;

            //! True if the float has a constant increment
            virtual bool InternalHasInc();

            //! Get the constant increment if there is any
            virtual double InternalGetInc();

            //! Get list of valid value
            virtual const double_autovector_t InternalGetListOfValidValues();

            //! Get recommended representation
            virtual  ERepresentation InternalGetRepresentation() const;

            //! Get the unit
            virtual GenICam::gcstring InternalGetUnit() const;

            //! Get the way the float should be converted to a string
            virtual EDisplayNotation InternalGetDisplayNotation() const;

            //! Get the precision to be used when converting the float to a string
            virtual int64_t InternalGetDisplayPrecision() const;
        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Interface INode (methods are called by the NodeT class)
        //@{

        //! Get Caching Mode
        virtual ECachingMode InternalGetCachingMode() const;
        //@}


        virtual void SetInvalid(ESetInvalidMode simMode);

    public:
        //-------------------------------------------------------------
        //! \name Interface INodePrivate
        //@{
            //! checks if the node is terminal
            virtual bool IsTerminalNode() const;

            //! finishes the constriction of the node
            virtual void FinalConstruct();
        //@}

    //-------------------------------------------------------------
    // Initializing
    //-------------------------------------------------------------
    public:
        BEGIN_CONVERT_MAP
            CHAIN_CONVERT_MAP(CNodeImpl)
            SWITCH_CONVERT_MAP

            CONVERT_ENTRY(Value_ID, m_Value)
            CONVERT_NODE_REFERENCE(pValue_ID, m_Value, IBase)
            CONVERT_NODE_REFERENCE(pValueDefault_ID, m_ValueDefault, IBase)
            CONVERT_NODE_REFERENCE(pMin_ID, m_Min, IBase)
            CONVERT_NODE_REFERENCE(pMax_ID, m_Max, IBase)
            CONVERT_NODE_REFERENCE(pInc_ID, m_Inc, IBase)
            CONVERT_NODE_REFERENCE(pIndex_ID, m_Index, IBase)
            CONVERT_ENTRY(ValueDefault_ID, m_ValueDefault)
            CONVERT_ENTRY(Min_ID, m_Min)
            CONVERT_ENTRY(Max_ID, m_Max)
            CONVERT_ENTRY(Inc_ID, m_Inc)
            CONVERT_ENUM_ENTRY(Representation_ID, m_Representation, ERepresentationClass)
            CONVERT_VALID_VALUE_SET(ValidValueSet_ID, m_ValidValueSet)
            CONVERT_STRING_ENTRY(Unit_ID, m_Unit)
            CONVERT_ENUM_ENTRY(DisplayNotation_ID, m_DisplayNotation, EDisplayNotationClass)
            CONVERT_ENTRY(DisplayPrecision_ID, m_DisplayPrecision)

            //*****************************
            // Special entry for pValueIndexed
            //*****************************

            case pValueIndexed_ID:
            {
                if (Direction == In)
                {
                    // create value and index
                    INode * const pNode = m_pNodeMap->GetNode(ValueStrIn);
                    if(!pNode)
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist.", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn );
                    CFloatPolyRef Value;
                    Value = dynamic_cast<IBase*>(pNode);

                    int64_t Index;
                    if( !String2Value(AttributeStrIn, &Index))
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : cannot convert attribute '%s' to double.", m_Name.c_str(), PropertyName, AttributeStrIn );

                    // add indexed value to the map
                    m_ValuesIndexed.insert(std::pair<int64_t, CFloatPolyRef>(Index, Value));

                    // make the node a child
                    AddChild(pNode);

                    return true;
                }
                else if (Direction == Out)
                {
                    // create a list of values and a list of indices
                    std::map<int64_t, CFloatPolyRef>::iterator ptr;
                    for(ptr=m_ValuesIndexed.begin(); ptr!=m_ValuesIndexed.end(); ptr++ )
                    {
                        GenICam::gcstring _Index;
                        Value2String((*ptr).first, _Index);
                        AttributeStrOut += _Index + "\t";

                        ValueStrOut += (*ptr).second.GetPointer()->GetName() + "\t";
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

            //*****************************
            // Special entry for ValueIndexed
            //*****************************
            case ValueIndexed_ID:
            {
                if (Direction == In)
                {
                    // create value and index
                    CFloatPolyRef Value;
                    if( !String2Value(ValueStrIn, &Value))
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : cannot convert value '%s' to double.", m_Name.c_str(), PropertyName, ValueStrIn );
                    int64_t Index;
                    if( !String2Value(AttributeStrIn, &Index))
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : cannot convert attribute '%s' to int64.", m_Name.c_str(), PropertyName, AttributeStrIn );

                    // add indexed value to the map
                    m_ValuesIndexed.insert(std::pair<int64_t, CFloatPolyRef>(Index, Value));

                    return true;
                }
                else if (Direction == Out)
                {
                    // create a list of values and a list of indices
                    std::map<int64_t, CFloatPolyRef>::iterator ptr;
                    for(ptr=m_ValuesIndexed.begin(); ptr!=m_ValuesIndexed.end(); ptr++ )
                    {
                        GenICam::gcstring _Index;
                        Value2String((*ptr).first, _Index);
                        AttributeStrOut += _Index + "\t";

                        GenICam::gcstring _Value;
                        Value2String((*ptr).second, _Value);
                        ValueStrOut += _Value + "\t";
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

        END_CONVERT_MAP


    protected:
        //-------------------------------------------------------------
        //! \name Property variables
        //@{
            //! Map storing the indexed values by index
            std::map<int64_t, CFloatPolyRef> m_ValuesIndexed;

            //! The value
            CFloatPolyRef m_ValueDefault;

            //! The value
            CFloatPolyRef m_Value;

            //! The minimum value
            CFloatPolyRef m_Min;

            //! The maximum value
            CFloatPolyRef m_Max;

            //! The constant increment if there is any
            CFloatPolyRef m_Inc;

            //! Reference to the index value
            CIntegerPolyRef m_Index;

            //! recommended representation of the value
            mutable ERepresentation m_Representation;

            //! the physical unit name
            GenICam::gcstring m_Unit;

            //! the printf format specifier used to convert the float number to a string
            EDisplayNotation m_DisplayNotation;

            //! the precision the float is converted with to a string
            int64_t m_DisplayPrecision;

            //! the list of valie value for the integer.
            double_autovector_impl m_ValidValueSet;

        //@}
    };

    /*! \brief Float node implementation
        \ingroup GenApi_Implementation
    */
    class GENAPI_DECL CFloat
        : public BaseT< ValueT< NodeT < FloatT < CFloatImpl > > > >
    {
    };
}

#pragma warning ( pop )

#endif // GENAPI_FLOAT_H
