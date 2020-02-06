//-----------------------------------------------------------------------------
//  (c) 2006-2008 by Basler Vision Technologies
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
    \ingroup GenApi_Implementation
*/

#ifndef GENAPI_INTEGER_H
#define GENAPI_INTEGER_H

#include "../IInteger.h"
#include "Node.h"
#include "BaseT.h"
#include "ValueT.h"
#include "IntegerT.h"
#include "NodeT.h"
#include "PrivateTypes.h"
#include "PolyReference.h"
#include <map>

#pragma warning ( push )
#pragma warning ( disable : 4275 ) // non dll-interface XXX used as base for  dll-interface class YYY

namespace GenApi
{

    /*!
        \brief Core of the Integer node implementation
        \ingroup GenApi_Implementation
    */
    class  GENAPI_DECL CIntegerImpl
        : public IInteger
        , public CNodeImpl
    {
    public:
        //-------------------------------------------------------------
        //! \name Constructor / destructor
        //@{
            //! Constructor
            CIntegerImpl();
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
                return intfIInteger;
            }

        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Interface IInteger (methods are called by the IntegerT class)
        //@{
            //! Set feature value
            virtual void InternalSetValue(int64_t Value, bool Verify = true);

            //! Get feature value
            virtual int64_t InternalGetValue(bool Verify = false, bool IgnoreCache = false );

            //! Get minimum value allowed
            virtual int64_t InternalGetMin();

            //! Get maximum value allowed
            virtual int64_t InternalGetMax();

            //! Get increment
            virtual int64_t InternalGetInc();

	        //! Get list of valid values
            virtual const int64_autovector_t InternalGetListOfValidValues();

            //! Get recommended representation
            virtual  ERepresentation InternalGetRepresentation();

            //! Get the unit
            virtual GenICam::gcstring InternalGetUnit();
        //@}
    public:
        //-------------------------------------------------------------
        //! \name Interface INodePrivate
        //@{
        //! checks if the node is terminal
        virtual bool IsTerminalNode() const;

        //! finishes the constriction of the node
        virtual void FinalConstruct();

        //! Get Caching Mode
        virtual ECachingMode InternalGetCachingMode() const;
        //@}

        virtual void SetInvalid(ESetInvalidMode simMode);

    public:

        BEGIN_CONVERT_MAP
            CHAIN_CONVERT_MAP(CNodeImpl)
            SWITCH_CONVERT_MAP
                
            //*****************************
            // Special entry to deal with Value and ValueDefault
            //*****************************
            case Value_ID:
            {
                if (Direction == In)
                {
                    // create the entry
                    CIntegerPolyRef Value;
                    if( !String2Value(ValueStrIn, &Value))
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : cannot convert value '%s' to int64.", m_Name.c_str(), PropertyName, ValueStrIn );

                    // add Value/ValueDefault to the list and remember it as main value
                    m_ValuesCopy.push_back(Value);
                    m_pMainValue = m_ValuesCopy.end();
                    m_pMainValue--;

                    return true;
                }
                else if (Direction == Out)
                {
                    // return the value
                    if( ! m_Index.IsInitialized() )
                       Value2String((*m_pMainValue), ValueStrOut);
                    else 
                       ValueStrOut = "";
                    return true;
                }
            }
            break;

            //*****************************
            // Special entry to deal with pValue and pValueCopy
            //*****************************
            case pValue_ID:
            case pValueCopy_ID:
            {
                if (Direction == In)
                {
                    // check and create the entry
                    INode * const pNode = m_pNodeMap->GetNode(ValueStrIn);
                    if(!pNode)
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist.", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn );
                    CIntegerPolyRef ListElem;
                    ListElem = dynamic_cast<IBase*>(pNode);

                    // all pValue/pValueCopy entries go to the list
                    m_ValuesCopy.push_back(ListElem);

                    // the one and only pValue is remembered as main value
                    if(strcmp(PropertyName,"pValue") == 0)
                    {
                        m_pMainValue = m_ValuesCopy.end();
                        m_pMainValue--;
                    }

                    // make the node a child
                    AddChild(pNode);

                    return true;
                }
                else if (Direction == Out && strcmp(PropertyName,"pValue") == 0 )
                {
                    if( m_Index.IsInitialized() )
                    {
                       ValueStrOut = "";
                    }

                    // if there is a pValue return the name the poiter is referencing to                 

                    else if( (*m_pMainValue).IsPointer() )
                    {
                        ValueStrOut = (*m_pMainValue).GetPointer()->GetName();
                    }
                    return true;
                }
                else if (Direction == Out && strcmp(PropertyName,"pValueCopy") == 0 )
                {
                    if( m_Index.IsInitialized() )
                    {
                       ValueStrOut = "";
                    }
                    // return a list of all nodes referenced to via pValueCopy entries
                    else 
                    {
                        std::list<CIntegerPolyRef>::iterator ptr;
                        for(ptr=m_ValuesCopy.begin(); ptr!=m_ValuesCopy.end(); ptr++ )
                        {
                            if( ptr != m_pMainValue )
                                ValueStrOut += (*ptr).GetPointer()->GetName() + "\t";
                        }
                        // get rid of the last tab
                        if(ValueStrOut.size() > 1)
                            ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1);
                        return true;
                    }
                }
            }
            break;

            CONVERT_NODE_REFERENCE(pMin_ID, m_Min, IBase)
            CONVERT_NODE_REFERENCE(pMax_ID, m_Max, IBase)
            CONVERT_NODE_REFERENCE(pInc_ID, m_Inc, IBase)
            CONVERT_NODE_REFERENCE(pValueDefault_ID, m_ValueDefault, IBase)
            CONVERT_NODE_REFERENCE(pIndex_ID, m_Index, IBase)
            CONVERT_ENTRY(Min_ID, m_Min)
            CONVERT_ENTRY(Max_ID, m_Max)
            CONVERT_ENTRY(Inc_ID, m_Inc)
            CONVERT_ENTRY(ValueDefault_ID, m_ValueDefault)
            CONVERT_ENUM_ENTRY(Representation_ID, m_Representation, ERepresentationClass)
            CONVERT_VALID_VALUE_SET(ValidValueSet_ID, m_ValidValueSet)
            CONVERT_STRING_ENTRY(Unit_ID, m_Unit)

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
                    CIntegerPolyRef Value;
                    Value = dynamic_cast<IBase*>(pNode);

                    int64_t Index;
                    if( !String2Value(AttributeStrIn, &Index))
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : cannot convert attribute '%s' to int64.", m_Name.c_str(), PropertyName, AttributeStrIn );

                    // add indexed value to the map
                    m_ValuesIndexed.insert(std::pair<int64_t, CIntegerPolyRef>(Index, Value));

                    // make the node a child
                    AddChild(pNode);

                    return true;
                }
                else if (Direction == Out)
                {
                    // create a list of vlaues and a list of indeces
                    std::map<int64_t, CIntegerPolyRef>::iterator ptr;
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
                    CIntegerPolyRef Value;
                    if( !String2Value(ValueStrIn, &Value))
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : cannot convert value '%s' to int64", m_Name.c_str(), PropertyName, ValueStrIn );
                    int64_t Index;
                    if( !String2Value(AttributeStrIn, &Index))
                        throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : cannot convert attribute '%s' to int64", m_Name.c_str(), PropertyName, AttributeStrIn );

                    // add indexed value to the map
                    m_ValuesIndexed.insert(std::pair<int64_t, CIntegerPolyRef>(Index, Value));

                    return true;
                }
                else if (Direction == Out)
                {
                    // create a list of vlaues and a list of indeces
                    std::map<int64_t, CIntegerPolyRef>::iterator ptr;
                    for(ptr=m_ValuesIndexed.begin(); ptr!=m_ValuesIndexed.end(); ptr++ )
                    {
                        GenICam::gcstring _Index;
                        Value2String((*ptr).first, _Index);
                        AttributeStrOut += _Index + "\t";

                        GenICam::gcstring _Value;
                        Value2String((*ptr).second, _Value);
                        AttributeStrOut += _Value + "\t";
                    }

                    // get rid of the respective last tabs of the lists
                    if(AttributeStrOut.size() > 1)
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
            //! List storing the main value and the copy values
            std::list<CIntegerPolyRef> m_ValuesCopy;

            //! References to the main value inside m_ValuesCopy
            std::list<CIntegerPolyRef>::iterator m_pMainValue;

            //! Map storing the indexed values by index
            std::map<int64_t, CIntegerPolyRef> m_ValuesIndexed;

            //! Reference to the default value which is used if the index has no matching entry
            CIntegerPolyRef m_ValueDefault;

            //! Reference to the minimum value
            CIntegerPolyRef m_Min;

            //! Reference to the maximum value
            CIntegerPolyRef m_Max;

            //! Reference to the increment value
            CIntegerPolyRef m_Inc;

            //! Reference to the index value
            CIntegerPolyRef m_Index;

            //! recommended representation of the value
            ERepresentation m_Representation;

            //! the physical unit name
            GenICam::gcstring m_Unit;

            //! The list of valid values for the integer.
            int64_autovector_impl m_ValidValueSet;

        //@}
    };

    /*!
        \brief Integer node implementation
        \ingroup GenApi_Implementation
    */
    class GENAPI_DECL CInteger : public BaseT< ValueT< IntegerT< NodeT < CIntegerImpl> > > >
    {
    };

}

#pragma warning ( pop )

#endif // GENAPI_INTEGER_H
