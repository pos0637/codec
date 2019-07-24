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
\brief    Defintion of macros for property conversion map
\ingroup GenApi_Implementation
*/

#ifndef GENAPI_NODEMACROS_H
#define GENAPI_NODEMACROS_H

//*************************************************************
// Macros forming the CONVERSION_MAP
//*************************************************************

typedef enum
{
    In,
    Out
} Direction_t;

static GenICam::gcstring EmptyString;

//! ---------------------------------------------------------------------
#define BEGIN_CONVERT_MAP \
    virtual bool SetProperty(const char* PropertyName, const char* ValueStr) \
    { \
        return SetProperty(PropertyName, ValueStr, ""); \
    } \
virtual bool SetProperty(const char* PropertyName, const char* ValueStr, const char* AttributeStr) \
    { \
        bool PropertyAlreadyPresent=false; \
        GenICam::gcstring_vector::iterator ptr; \
        for(ptr=m_PropertyNames.begin(); ptr!=m_PropertyNames.end(); ptr++) \
            if(*ptr == PropertyName) \
                PropertyAlreadyPresent = true; \
        if(!PropertyAlreadyPresent) \
            m_PropertyNames.push_back(PropertyName); \
        return AccessProperty( PropertyName, ValueStr, AttributeStr, EmptyString, EmptyString, In ); \
    } \
virtual bool AccessProperty(const char* PropertyName, const char* ValueStrIn, const char* AttributeStrIn, GenICam::gcstring& ValueStrOut, GenICam::gcstring& AttributeStrOut, Direction_t Direction) \
  { \
    ValueStrIn=ValueStrIn;          \
    AttributeStrIn=AttributeStrIn;  \
    if(    Direction == Out ) \
    { \
        ValueStrOut = ""; \
        AttributeStrOut = ""; \
    }


//! ---------------------------------------------------------------------
#define CHAIN_CONVERT_MAP(TARGET) \
  if(Direction == Out) {    \
      if(TARGET::AccessProperty(PropertyName, "", "", ValueStrOut, AttributeStrOut, Out)) \
        return true;    \
  } \
  else { \
      if(TARGET::AccessProperty(PropertyName, ValueStrIn, AttributeStrIn, EmptyString, EmptyString, In)) \
        return true;    \
  }

//! ---------------------------------------------------------------------
#define SWITCH_CONVERT_MAP \
    int lID = GetIDFromMap( PropertyName ); \
    switch ( lID ) \
    {


//! ---------------------------------------------------------------------
#define CONVERT_ENUM_ENTRY(_ID, Member, _EnumDelegate) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            if( !_EnumDelegate::FromString(ValueStrIn, &(Member))) \
                throw PROPERTY_EXCEPTION_NODE("property '%s' : cannot convert value '%s' to target type.", PropertyName, ValueStrIn ); \
            return true; \
        } \
        else if (Direction == Out) \
        { \
            ValueStrOut = _EnumDelegate::ToString(Member); \
            return true; \
        } \
    } \
    break;

//! ---------------------------------------------------------------------
#define CONVERT_NODESET_REFERENCE(_ID, Member, Interface, ContainerType ) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            INode * const pNode = m_pNodeMap->GetNode(ValueStrIn); \
            if(!pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist.", m_Name.c_str(), PropertyName, ValueStrIn ); \
            Interface * const pListElem = dynamic_cast<Interface*>(pNode); \
            if(!pListElem) \
                throw PROPERTY_EXCEPTION_NODE("property list element '%s' : node '%s' has no interface '" #Interface "'.", PropertyName, ValueStrIn ); \
            Member.insert(pListElem);\
            return true; \
        } \
        else if (Direction == Out) \
        { \
            ContainerType::iterator ptr; \
            for(ptr=Member.begin(); ptr!=Member.end(); ptr++ ) \
                ValueStrOut += (*ptr)->GetName() + "\t"; \
            if(ValueStrOut.size() > 1) \
                ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1); \
            return true; \
        } \
    } \
    break;

//! ---------------------------------------------------------------------
#define CONVERT_NODE_REFERENCE(_ID, Member, Interface) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            INode * const pNode = m_pNodeMap->GetNode(ValueStrIn); \
            if(!pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist.", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Member = dynamic_cast<Interface*>(pNode); \
            if( Member == NULL ) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' has no interface '" #Interface "'.", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            if (dynamic_cast<INode*>(this) == pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : self reference", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName ); \
            AddChild(pNode ); \
            return true; \
        } \
        else if (Direction == Out) \
        { \
            INode *pNode = (INode*)Member; \
            if(pNode) \
                ValueStrOut = pNode->GetName(); \
            return true; \
        } \
    } \
    break;


//! ---------------------------------------------------------------------
#define CONVERT_STRING_ENTRY(_ID, Member) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            Member = ValueStrIn; \
            return true; \
        } \
        else if (Direction == Out) \
        { \
            ValueStrOut = Member; \
            return true; \
        } \
    } \
    break;


//! ---------------------------------------------------------------------
#define CONVERT_NODELIST_REFERENCE2(_ID, Member, Interface) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            INode * const pNode = m_pNodeMap->GetNode(ValueStrIn); \
            if(!pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Interface * const pListElem = dynamic_cast<Interface*>(pNode); \
            if(!pListElem) \
                throw PROPERTY_EXCEPTION_NODE("%s : property list element '%s' : node '%s' has no interface '" #Interface "'", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Member.push_back(pListElem);\
            return true; \
        } \
        else if (Direction == Out) \
        { \
            FeatureList_t::iterator ptr; \
            for(ptr=Member.begin(); ptr!=Member.end(); ptr++ ) \
            { \
                ValueStrOut += (*ptr)->GetNode()->GetName() + "\t"; \
            } \
            if(ValueStrOut.size() > 1) \
                ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1); \
            return true; \
        } \
    } \
    break;

//! ---------------------------------------------------------------------
#define CONVERT_NODELIST_REFERENCE4(_ID, Member, Interface) \
	case _ID: \
	{ \
	if (Direction == In) \
		{ \
		INode * const pNode = m_pNodeMap->GetNode(ValueStrIn); \
		if(!pNode) \
		throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
		Interface * const pListElem = dynamic_cast<Interface*>(pNode); \
		if(!pListElem) \
		throw PROPERTY_EXCEPTION_NODE("%s : property list element '%s' : node '%s' has no interface '" #Interface "'", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
		Member.push_back(pListElem);\
		m_DependingChildren.insert( dynamic_cast<INodePrivate*>(pNode) ); \
		return true; \
		} \
		else if (Direction == Out) \
		{ \
		FeatureList_t::iterator ptr; \
		for(ptr=Member.begin(); ptr!=Member.end(); ptr++ ) \
			{ \
			ValueStrOut += (*ptr)->GetNode()->GetName() + "\t"; \
			} \
			if(ValueStrOut.size() > 1) \
			ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1); \
			return true; \
		} \
	} \
	break;


//! ---------------------------------------------------------------------
#define CONVERT_INVALIDATOR( _ID ) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            INode *pNode = m_pNodeMap->GetNode( ValueStrIn ); \
            if (! pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            m_Invalidators.push_back( pNode ); \
            m_DependingChildren.insert( dynamic_cast< INodePrivate*>(pNode) ); \
            dynamic_cast< INodePrivate*>(pNode)->SetNotDependencyTopLevel(); \
            return true; \
        } \
        else if (Direction == Out) \
        { \
            NodeList_t::iterator ptr; \
            for(ptr=m_Invalidators.begin(); ptr!=m_Invalidators.end(); ptr++ ) \
            { \
                ValueStrOut += (*ptr)->GetName() + "\t"; \
            } \
            if(ValueStrOut.size() > 1) \
                ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1); \
            return true; \
        } \
    } \
    break;


//! ---------------------------------------------------------------------
#define CONVERT_ENTRY(_ID, Member) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            if( !String2Value(ValueStrIn, &Member)) \
                throw PROPERTY_EXCEPTION("%s : property '%s' : cannot convert value '%s'", m_Name.c_str(), PropertyName, ValueStrIn ); \
            return true; \
        } \
        else if (Direction == Out) \
        { \
            Value2String(Member, ValueStrOut); \
            return true; \
        } \
    } \
    break;


//! ---------------------------------------------------------------------
#define CONVERT_REFERENCE(_ID, Member, Interface) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            INode * const pNode = m_pNodeMap->GetNode(ValueStrIn); \
            if(!pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Member = dynamic_cast<Interface*>(pNode); \
            if(!Member) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' has no interface '" #Interface "'", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            return true; \
        } \
        else if (Direction == Out) \
        { \
            INode *pNode = (INode*)Member; \
            if(pNode) \
                ValueStrOut = pNode->GetName(); \
            return true; \
        } \
    } \
    break;


//! ---------------------------------------------------------------------
#define CONVERT_NODELIST_REFERENCE(_ID, Member, Interface, ContainerType ) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            INode * const pNode = m_pNodeMap->GetNode(ValueStrIn); \
            if(!pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist.", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Interface * const pListElem = dynamic_cast<Interface*>(pNode); \
            if(!pListElem) \
                throw PROPERTY_EXCEPTION_NODE("%s : property list element '%s' : node '%s' has no interface '" #Interface "'", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Member.push_back(pListElem);\
            dynamic_cast<CNodeImpl *>(this)->AddChild(pNode); \
            return true; \
        } \
        else if (Direction == Out) \
        { \
            ContainerType::iterator ptr; \
            for(ptr=Member.begin(); ptr!=Member.end(); ptr++ ) \
                ValueStrOut += (*ptr)->GetNode()->GetName() + "\t"; \
            if(ValueStrOut.size() > 1) \
                ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1); \
            return true; \
        } \
    } \
    break;


//! ---------------------------------------------------------------------
#define CONVERT_LIST_ENTRY(_ID, Member, _Type) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            _Type listElem;\
            if( !String2Value(ValueStrIn, &listElem)) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : cannot convert value '%s'", m_Name.c_str(), PropertyName, ValueStrIn ); \
            Member.push_back(listElem);\
            return true; \
        } \
        else if (Direction == Out) \
       {  Values2String( Member, ValueStrOut ); return true; } \
        } \
    break;


//! ---------------------------------------------------------------------
#define CONVERT_NODELIST_REFERENCE_AUXREF(_ID, Member, Interface, AuxMember, AuxInterface ) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            INode * const pNode = m_pNodeMap->GetNode(ValueStrIn); \
            if(!pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Interface * const pListElem = dynamic_cast<Interface*>(pNode); \
            if(!pListElem) \
                throw PROPERTY_EXCEPTION_NODE("%s : property list element '%s' : node '%s' has no interface '" #Interface "'", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            INode * const pAux = m_pNodeMap->GetNode(AttributeStrIn); \
            if(!pAux) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : attr '%s' does not exist", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, AttributeStrIn ); \
            AuxInterface * const pListElemAux = dynamic_cast<AuxInterface*>(pAux); \
            if(!pListElemAux) \
                throw PROPERTY_EXCEPTION_NODE("%s : property list element '%s' : attr '%s' has no interface '" #AuxInterface "'", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, AttributeStrIn ); \
            Member.push_back(pListElem);\
            AuxMember.push_back(pListElemAux);\
            dynamic_cast<CNodeImpl*>(this)->AddChild(pNode); \
            dynamic_cast<CNodeImpl*>(this)->AddChild(pAux); \
            return true; \
        } \
        else if (Direction == Out) \
        { \
            IntegerList_t::iterator ptrValue; \
            IntegerList_t::iterator ptrAttribute; \
            for(ptrValue=Member.begin(), ptrAttribute=AuxMember.begin(); \
                ptrValue!=Member.end();  \
                ptrValue++, ptrAttribute++ ) \
            { \
                ValueStrOut += (*ptrValue)->GetNode()->GetName() + "\t"; \
                AttributeStrOut += (*ptrAttribute)->GetNode()->GetName() + "\t"; \
            } \
            if(ValueStrOut.size() > 1) \
            { \
                ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1); \
                AttributeStrOut = AttributeStrOut.substr(0, AttributeStrOut.size()-1); \
            }\
            return true; \
        } \
    } \
    break;


//! ---------------------------------------------------------------------
#define CONVERT_NODELIST_REFERENCE3(_ID, Member, Interface, ContainerType ) \
    case _ID: \
    { \
        if (Direction == In) \
        { \
            INode * const pNode = m_pNodeMap->GetNode(ValueStrIn); \
            if(!pNode) \
                throw PROPERTY_EXCEPTION_NODE("%s : property '%s' : node '%s' does not exist", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Interface * const pListElem = dynamic_cast<Interface*>(pNode); \
            if(!pListElem) \
                throw PROPERTY_EXCEPTION_NODE("%s : property list element '%s' : node '%s' has no interface '" #Interface "'", dynamic_cast<INode*>(this)->GetName().c_str(), PropertyName, ValueStrIn ); \
            Member.push_back(pListElem);\
            return true; \
        } \
        else if (Direction == Out) \
        { \
            ContainerType::iterator ptr; \
            for(ptr=Member.begin(); ptr!=Member.end(); ptr++ ) \
                ValueStrOut += (*ptr)->GetName() + "\t"; \
            if(ValueStrOut.size() > 1) \
                ValueStrOut = ValueStrOut.substr(0, ValueStrOut.size()-1); \
            return true; \
        } \
    } \
    break;

#define CONVERT_VALID_VALUE_SET(_ID, member) \
    case _ID:\
    { \
        if (Direction == In) \
        { \
            GenICam::gcstring_vector::const_iterator it; \
            GenICam::gcstring_vector valueVector; \
            GenICam::Tokenize( ValueStrIn, valueVector, ";"); \
            m_ValidValueSet = valueVector; \
        } \
        else if (Direction == Out) \
        { \
            GenICam::gcstring_vector valueVector; \
            m_ValidValueSet.ToStrings(valueVector); \
            for( GenICam::gcstring_vector::const_iterator it = valueVector.begin(); \
                        it != valueVector.end(); it++) \
            {\
                if( ValueStrOut.size())\
                    ValueStrOut.append(";");\
                ValueStrOut.append(*it); \
            }\
        } \
    } \
    break;

//! ---------------------------------------------------------------------
#define END_CONVERT_MAP \
    } \
    return false; \
};


#endif  // GENAPI_NODEMACROS_H
