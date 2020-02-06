//-----------------------------------------------------------------------------
//  (c) 2006-2009 by Basler Vision Technologies
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

#ifndef GENAPI_NODE_H
#define GENAPI_NODE_H

#include <sstream>
#include <cctype>

#include "Base/GCException.h"
#include "../GenApiDll.h"
#include "../Synch.h"
#include "../Types.h"
#include "../INode.h"
#include "../ICategory.h"
#include "INodePrivate.h"
#include "INodeMapPrivate.h"
#include "../IInteger.h"
#include "../IBoolean.h"
#include "../ISelector.h"
#include "../NodeCallback.h"
#include "NodeMacros.h"
#include "StringsToIDs.h"
#include "../EnumClasses.h"
#include "../Log.h"
#include "Value2String.h"
#include "PolyReference.h"
#include <list>

#ifdef _MSC_VER
#   pragma warning(push)
#   pragma warning(disable: 4251) // class 'xxx' needs to have dll-interface to be used by clients of class 'yyy'
#   pragma warning(disable: 4275) // non dll-interface struct 'GenApi::INodePrivate' used as base for dll-interface class 'GenApi::CNodeImpl'
#endif

namespace GenApi
{
    class CNodeImpl;

    //*************************************************************
    // CNodeImpl class
    //*************************************************************

    /*!
        \brief Standard implementation for the INode and the ISelector interface
        \ingroup GenApi_Implementation
    */
    class GENAPI_DECL CNodeImpl : public INodePrivate, public ISelector
    {
    public:
        //-------------------------------------------------------------
        //! \name Constructor / destructor
        //@{
            //! Constructor
            CNodeImpl();

            //! Destructor
            virtual ~CNodeImpl();
        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Interface IBase (methods are called by the BaseT class)
        //@{
            //! Get the access mode of the node
            virtual EAccessMode InternalGetAccessMode() const;

            //! Implementation of IBase::GetPrincipalInterfaceType()
            #pragma BullseyeCoverage off
            virtual EInterfaceType InternalGetPrincipalInterfaceType() const
            {
                return intfIBase;
            }
            #pragma BullseyeCoverage on

            //! Default implementation of GetAccessMode taking into account another node
            EAccessMode InternalGetAccessMode(IBase* pValue) const;
        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Interface INode (methods are called by the NodeT class)
        //@{
            //! Get node name
            virtual GenICam::gcstring InternalGetName(bool FullQualified=false) const;

            //! Get name space
            virtual GenApi::ENameSpace InternalGetNameSpace()const;

            //! Get the recommended visibility of the node
            virtual EVisibility InternalGetVisibility() const;

            // Invalidate the node
            virtual void InternalInvalidateNode( std::list<CNodeCallback*> &CallbacksToFire );

            //! Is the node value cachable
            virtual bool InternalIsCachable() const;

            //! Get Caching Mode
            virtual ECachingMode InternalGetCachingMode() const;

            //! recommended polling time (for not cachable nodes)
            virtual int64_t InternalGetPollingTime() const;

            //! Get a short description of the node
            virtual GenICam::gcstring InternalGetToolTip() const;

            //! Get a long description of the node
            virtual GenICam::gcstring InternalGetDescription() const;

            // Get node display name
            virtual GenICam::gcstring InternalGetDisplayName() const;

            //! Get a name of the device
            virtual GenICam::gcstring InternalGetDeviceName() const;

            //! Get all children of the node
            virtual void InternalGetChildren(GenApi::NodeList_t &Children, ELinkType LinkType) const;

            //! Register change callback
            /*! Takes ownership of the CNodeCallback object */
            virtual CallbackHandleType InternalRegisterCallback( CNodeCallback *pCallback );

            //! Deregister change callback
            /*! Destroys CNodeCallback object
                \return true if the callback handle was valid
            */
            virtual bool InternalDeregisterCallback( CallbackHandleType hCallback );

            //! Retrieves the node map
            virtual INodeMap* InternalGetNodeMap() const;

            //! Get the EventId of the node
            virtual GenICam::gcstring GetEventID() const;

            //! True if the node is streamable
            virtual bool IsStreamable() const;

            //! Returns a list of the names all properties set during initialization
            virtual void GetPropertyNames(GenICam::gcstring_vector &PropertyNames) const;

            //! Retrieves a property plus an additional attribute by name
            /*! If a property has multiple values/attribute they come with Tabs as delimiters */
            virtual bool GetProperty(const GenICam::gcstring& PropertyName, GenICam::gcstring& ValueStr, GenICam::gcstring& AttributeStr);

            //! Imposes an access mode to the natural access mode of the node
            virtual void ImposeAccessMode(EAccessMode ImposedAccessMode);

            //! Imposes a visibility  to the natural visibility of the node
            virtual void ImposeVisibility(EVisibility ImposedVisibility);

            //! Retrieves the a node which describes the same feature in a different way
            virtual INode* GetAlias() const;

            //! Retrieves the a node which describes the same feature so that it can be casted
            virtual INode* GetCastAlias() const;

            //! Checks for an explicitly via the <pError> element defined error
            virtual void InternalCheckError() const;

            //! Gets a URL pointing to the documentation of that feature
            virtual GenICam::gcstring InternalGetDocuURL() const;

            //! True if the node should not be used any more
            virtual bool InternalIsDeprecated() const;

            //! True if the node can be reached via category nodes from a category node named "Std::Root"
            virtual bool IsFeature() const;

            //! True if the AccessMode can be cached
            virtual EYesNo InternalIsAccessModeCacheable() const;

            //! returns true, if the AccessModeCache is valid
            /*! Handles silently the cycle breaker */
            inline bool IsAccessModeCached() const
            {
                if( _UndefinedAccesMode == m_AccessModeCache )
                    return false;
#pragma BullseyeCoverage off
                if( _CycleDetectAccesMode == m_AccessModeCache )
#pragma BullseyeCoverage on
                {
                    // the cycle is neutralized by making at least one node AccessMode cacheable
                    m_AccessModeCache = RW;
                    GCLOG_WARN(m_pAccessLog)( "InternalGetAccessMode : ReadCycle detected at = '%s', ReadingChild = '%s'", m_Name.c_str() );
                }
                return true;

            }
        //@}

    public:
        //-------------------------------------------------------------
        //! \name Interface INodePrivate
        //@{
            virtual void CollectCallbacksToFire(std::list<CNodeCallback*> &CallbacksToFire, bool allDependents = false);
            virtual void FinalConstruct();
            virtual void SetInvalid(ESetInvalidMode simMode);
            virtual void Register(GenApi::INodeMapPrivate* const pNodeMap, const char *pNodeType, const char *pName, const char *pNameSpace);
            virtual void PropagateDependency( NodePrivateList_t &NextNodesToProcess);
            virtual bool PushDependencies( const NodeSet_t& DependingNodes, INodePrivate* pParent);
            virtual void CheckSelectedCycle( GenApi::NodePrivateList_t &NodeStack );
            virtual void CheckForReadCycles( NodePrivateList_t &NodeStack);
            virtual const NodeSet_t& PropagateTerminals();
            virtual void GetParents(GenApi::NodeList_t &Parents) const;
            virtual bool IsTerminalNode() const;
            virtual void SetNotTopLevel();
            virtual void SetNotDependencyTopLevel();
            virtual void GetTerminalNodes( GenApi::NodeList_t& Terminals ) const;
            virtual bool IsTopLevelNode() const;
            virtual bool IsDependencyTopLevelNode() const;
            virtual void AddChild( INode* pChild, bool AffectsAccessMode = true, bool AddParent = true );
            virtual void AddParent( INode* pParent );
            virtual bool Poll( int64_t ElapsedTime );
            virtual void GetAllDependingNodes( GenApi::NodeList_t &AllDependingNodes ) const;
            virtual void UpdateDependentEntry();

        //@}

    public:
        //-------------------------------------------------------------
        //! \name Interface ISelector
        //@{
            virtual bool IsSelector() const;
            virtual void GetSelectedFeatures( FeatureList_t& list ) const;
            virtual void GetSelectingFeatures( FeatureList_t& ) const;
        //@}

    public:

        //-------------------------------------------------------------
        //! \name Implementation of the CONVERT_MAP
            BEGIN_CONVERT_MAP
                SWITCH_CONVERT_MAP
                CONVERT_NODESET_REFERENCE(pDependent_ID, m_AllDependingNodes, INodePrivate, NodeSet_t)
                CONVERT_ENUM_ENTRY(IsFeature_ID, m_IsFeature, EYesNoClass)
                CONVERT_ENUM_ENTRY(NameSpace_ID, m_NameSpace, ENameSpaceClass)
                CONVERT_NODESET_REFERENCE(pTerminal_ID, m_AllTerminalNodes, INodePrivate, NodeSet_t)
                CONVERT_ENUM_ENTRY(Visibility_ID, m_Visibility, EVisibilityClass)
                CONVERT_ENUM_ENTRY(Streamable_ID, m_IsStreamable, EYesNoClass)
                CONVERT_ENUM_ENTRY(Cachable_ID, m_CachingMode, ECachingModeClass)
                CONVERT_NODE_REFERENCE(pIsImplemented_ID, m_IsImplemented, IBase)
                CONVERT_STRING_ENTRY(Description_ID, m_Description)
                CONVERT_STRING_ENTRY(ToolTip_ID, m_ToolTip)
                CONVERT_NODE_REFERENCE(pIsLocked_ID, m_IsLocked, IBase)
                CONVERT_NODELIST_REFERENCE2(pSelected_ID, m_Selected, IValue)
                CONVERT_NODELIST_REFERENCE4(pSelecting_ID, m_Selecting, IValue)
                CONVERT_STRING_ENTRY(EventID_ID, m_EventID)
                CONVERT_INVALIDATOR( pInvalidator_ID )
                CONVERT_STRING_ENTRY(DisplayName_ID, m_DisplayName)
                CONVERT_STRING_ENTRY(Name_ID, m_Name)
                CONVERT_STRING_ENTRY(NodeType_ID, m_NodeType)
                CONVERT_STRING_ENTRY(DeviceName_ID, m_DeviceName)
                CONVERT_NODE_REFERENCE(pIsAvailable_ID, m_IsAvailable, IBase)
                CONVERT_NODE_REFERENCE(pBlockPolling_ID, m_BlockPolling, IBase)
                CONVERT_NODE_REFERENCE(pError_ID, m_pError, IEnumeration)
                CONVERT_REFERENCE(pAlias_ID, m_pAlias, INode)
                CONVERT_REFERENCE(pCastAlias_ID, m_pCastAlias, INode)
                CONVERT_ENUM_ENTRY(ImposedAccessMode_ID, m_ImposedAccessMode, EAccessModeClass)
                CONVERT_ENUM_ENTRY(ImposedVisibility_ID, m_ImposedVisibility, EVisibilityClass)
                CONVERT_ENTRY(PollingTime_ID, m_PollingTime)
                CONVERT_STRING_ENTRY(DocuURL_ID, m_DocuURL)
                CONVERT_ENUM_ENTRY(IsDeprecated_ID, m_IsDeprecated, EYesNoClass)
                CONVERT_STRING_ENTRY( Extension_ID, m_Extension );
            END_CONVERT_MAP

    protected:
        //-------------------------------------------------------------
        //! \name Members properties and helpers
        //@{
            //! The type of the node, e.g. MaskedIntReg
            GenICam::gcstring m_NodeType;

            //! The name of the node
            GenICam::gcstring m_Name;

            //! The metadata from the extension
            GenICam::gcstring m_Extension;

            //! The namespace of the node
            ENameSpace m_NameSpace;

            //! The device name of the node tree
            GenICam::gcstring m_DeviceName;

            //! The display name string of the node
            GenICam::gcstring m_DisplayName;

            //! The ToolTip for the node
            GenICam::gcstring m_ToolTip;

            //! The Description of the node
            GenICam::gcstring m_Description;

            //! recommended visibility;
            EVisibility m_Visibility;

            //! Reference to a Node, which indicates if the node is implemented
            CBooleanPolyRef m_IsImplemented;

            //! Reference to a Node, which indicates if the node is available
            CBooleanPolyRef m_IsAvailable;

            //! Reference to a Node, which indicates if the node is locked (i.e. not writable)
            CBooleanPolyRef m_IsLocked;

            //! Reference to a Node, which indicates if the node is not cached
            CBooleanPolyRef m_BlockPolling;

            //! Pointer to a Error class
            IEnumeration *m_pError;

            //! Pointer to a Node, which describes the same feature as this one
            INode *m_pAlias;

            //! Pointer to a Node, which describes the same feature as this one so that it can be casted
            INode *m_pCastAlias;

            //! indicates that the node has changed
            mutable ECachingMode m_CachingMode;

            //! List of node references to features
            FeatureList_t m_Features;

            //! List of selected features
            FeatureList_t m_Selected;

            //! List of selecting features
            FeatureList_t m_Selecting;

            //! The EventID
            GenICam::gcstring m_EventID;

            //! indicates if the node is streamable
            EYesNo m_IsStreamable;

            //! Access mode imposed on the natural access mode of the node
            EAccessMode m_ImposedAccessMode;

            //! Visibility imposed to the natural visibility of the node
            EVisibility m_ImposedVisibility;

            //! recommended polling time in [ms]
            int64_t m_PollingTime;

            //! All nodes which will be invalidated if this node becomes invalid
            NodeSet_t m_AllDependingNodes;

            //! All terminal nodes which may be written to by this node
            NodeSet_t m_AllTerminalNodes;

            //! States when propagating terminals
            enum ETerminalPropagationState
            {
                eNotVisited, //!< PropagateTerminals not run for node
                eBeingVisited, //!< PropagateTerminals is in progress for node, used for detecting cycles
                eTerminalPropagationDone //! PropagateTerminals is done for node, can just copy from m_AllTerminalNodes
            };

            //! Current state for propagating terminals
            ETerminalPropagationState m_propagationState;

            //! List of references to nodes which may invalidate this node
            NodeList_t m_Invalidators;

            //! A URL pointing or the documentation of this featrues
            GenICam::gcstring m_DocuURL;

            //! indicates that the feature should not be used any more
            EYesNo m_IsDeprecated;

            //! Helper: A list of all properties belonging to this node
            GenICam::gcstring_vector m_PropertyNames;

            //! indicates that the node is a feature that is reachable from the Root node via categories
            EYesNo m_IsFeature;

        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Implementation of the node dependency handling
        //@{
            //! All nodes for which this node is at least a DependecyChild
            NodeSet_t m_Parents;

            //! All child nodes which will cause this node to be invalidated
            NodeSet_t m_DependingChildren;

            //! All child nodes which influence this node's AccessMode
            NodeSet_t m_ReadingChildren;

            //! All child nodes which may be written to
            NodeSet_t m_WritingChildren;

            //! Makes a node a writing child
            void SetWritingChild(IBase *pWritingChild);

            //! If true this node is no writing child to another node
            bool m_IsTopLevelNode;

            //! If true this node is no writing child to another node for dependencies
            bool m_IsDependencyTopLevelNode;
        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Implementation of the cache
        //@{
            //! stores the cached value of the CachingMode
            mutable ECachingMode m_CachingModeCache;

            //! cache access mode
            mutable EAccessMode m_AccessModeCache;

            //! true if the value cache is valid
            mutable bool m_ValueCacheValid;

            //! true if the list of valid value is cached
            mutable bool m_ListOfValidValuesCacheValid;

            //! Checks if the value comes from cache or is requested from another node
            virtual bool InternalIsValueCacheValid() const;

            //! indicates if the AccessMode is cacheable
            mutable EYesNo m_AccessModeCacheability;

        //@}

    protected:
        //-------------------------------------------------------------
        //! \name Implementation members and methods
        //@{
            //! Pointer to the node map
            INodeMapPrivate*  m_pNodeMap;

            //! time elapsed since the last poll
            int64_t m_ElapsedTime;

            //! shortcut for the lock type
            typedef AutoLock Lock;

            //! Acquire central lock
            CLock& GetLock() const;

            //! The bathometer is a counter used to measure the depth of SetValue-like call chains
            Counter& GetBathometer() const;

            //! Invalidates all nodes which will become affected by a SetValue call into the node tree
            void PreSetValue();

            //! Fires callback on all nodes which became affected by a SetValue call into the node tree
            void PostSetValue( std::list<CNodeCallback*> &CallbacksToFire );

            //! indicates that the cache has been filled and should not be cleared at the end of the operation
            bool m_DontDeleteThisCache;

            //! Used to ensure that PostSetValue() is called in any case
            class PostSetValueFinalizer
            {
            public:
                //! Constructor
                PostSetValueFinalizer(CNodeImpl* pThis,  std::list<CNodeCallback*> &CallbacksToFire ) :
                    m_pThis( pThis ),
                    m_CallbacksToFire( CallbacksToFire )
                {}

                //! Destructor calling PostSetValue()
                ~PostSetValueFinalizer()
                {
                    m_pThis->PostSetValue( m_CallbacksToFire );
                }

                //! pointer to owner object
                CNodeImpl* m_pThis;

                //! list of callbacks to file
                std::list<CNodeCallback*> &m_CallbacksToFire;

            private:
                //! \name Assignment and copying is not supported
                // \{
                PostSetValueFinalizer(const PostSetValueFinalizer&);
                PostSetValueFinalizer& operator=(const PostSetValueFinalizer&);
                // \}
            };

            //! List of callbacks
            std::list<CNodeCallback*> m_Callbacks;

            //! Used to ensure that PostSetValue() is called in any case
            class EntryMethodFinalizer
            {
            public:
                //! Constructor
                EntryMethodFinalizer(const INodePrivate* pThis, EMethod EntryMethod, bool IgnoreCache = false ) 
                {
                    assert(pThis);
                    m_pNodeMapPrivate = dynamic_cast<INodeMapPrivate*>( pThis->GetNodeMap() );
                    m_pNodeMapPrivate->SetEntryPoint( EntryMethod, pThis, IgnoreCache );
                }

                //! Destructor calling 
                ~EntryMethodFinalizer()
                {
                    m_pNodeMapPrivate->ResetEntryPoint();                  
                }

            private:
                //! Private cache for the INodeMapPrivate pointer
                INodeMapPrivate *m_pNodeMapPrivate;
            };

            //! Creates the full qualified name
            GenICam::gcstring GetQualifiedName(GenICam::gcstring Name, ENameSpace NameSpace) const;

        //@}

        //-------------------------------------------------------------
        //! \name Implementation of the loggers
        //@{
            // for safety reasons please keep these member variables at the
            // end of the class' memory layout

            //! Logger for messages concerning the AccessMode
            log4cpp::Category *m_pAccessLog;

            //! Logger for messages concerning the getting and setting values
            log4cpp::Category *m_pValueLog;

            //! Logger for messages concerning the range check
            log4cpp::Category *m_pRangeLog;

            //! Logger for messages concerning the port access
            log4cpp::Category *m_pPortLog;

            //! Logger for messages concerning the caching access
            log4cpp::Category *m_pCacheLog;

            //! Logger for things done during pre-processing of the node map, e.g. determining dependencies
            log4cpp::Category *m_pPreProcLog;

            //! Logger for messages concerning miscellaneous access which does not fit to the other categories
            log4cpp::Category *m_pMiscLog;

        //@}

    };

    //! Helper function for DeleteDoubleCallbacks
    bool GENAPI_DECL DeleteDoubleCallbacksCompare (GenApi::CNodeCallback* pA, GenApi::CNodeCallback* pB);

    //! deletes double callbacks from list
    void GENAPI_DECL DeleteDoubleCallbacks( std::list<CNodeCallback*> &CallbackList );

}

#ifdef _MSC_VER
#   pragma warning(pop)
#endif

#endif // ifndef GENAPI_NODE_H
