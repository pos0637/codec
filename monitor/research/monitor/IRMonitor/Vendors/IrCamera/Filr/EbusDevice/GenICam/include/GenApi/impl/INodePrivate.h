//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenApi
//  Author:  MargretAlbrecht
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
\brief    Definition of interface INodePrivate
\ingroup GenApi_PublicImpl
*/


#ifndef GENAPI_INODEPRIVATE_H
#define GENAPI_INODEPRIVATE_H

#include "../GenApiDll.h"
#include "../Types.h"
#include "../INode.h"
#include "../INodeMap.h"

#include <vector>
#include <set>
#include <list>

namespace GenApi
{
    //*************************************************************
    // INodePrivate interface
    //*************************************************************

    interface INodePrivate;
    interface INodeMapPrivate;

    //! a list of node references using the INodePrivate interface
    typedef std::vector<INodePrivate*> NodePrivateList_t;

    //! Set of pointers to nodes
    typedef std::set< INodePrivate* > NodeSet_t;

    /**
    \brief Interface including the methods for node construction common to all nodes
    \ingroup GenApi_PublicImpl
    */
    interface GENAPI_DECL_ABSTRACT INodePrivate : public INode
    {
        //! Three different modes of operation for INodePrivate::SetInvalid()
        enum ESetInvalidMode
        {
            simOnlyMe,  //!< Invalidate only the node itself
            simAll  //!< Invalidate the node and all of its dependents
        };

        //! Registers the node in the node map
        virtual void Register(GenApi::INodeMapPrivate* const pNodeMap, const char *pNodeType, const char *pName, const char *pNameSpace) = 0;

        //! Set property by name as string
        /*! return value true if the property was handled; false else */
        virtual bool SetProperty(const char* pPropertyName, const char* pValueStr) = 0;

        //! Set property by name as string with attribute
        /*! return value true if the property was handled; false else */
        virtual bool SetProperty(const char* pPropertyName, const char* pValueStr, const char* pAttributeStr) = 0;

        //! Initializes the object
        virtual void FinalConstruct() = 0;

        //! Update the registered callbacks
        virtual void CollectCallbacksToFire(std::list<CNodeCallback*> &CallbacksToFire, bool allDependents = false) = 0;

        //! Invalidate the node resp. the node and all of its dependents
        virtual void SetInvalid(ESetInvalidMode simMode) = 0;

        //! \name NodeDependencyHandling
        //!@{
            /*!
            \brief Inserts a node a child to this node
            \param[in] pChild The node to be added as child
            \param[in] AffectsAccessMode If true this node will be affected by the AccessMode of the child
            */
            virtual void AddChild( INode* pChild, bool AffectsAccessMode = true, bool AddParent = true ) = 0;

            /*!
            \brief Inserts a node as parent to this node
            \param[in] pParent The node to be added as parent
            */
            virtual void AddParent( INode* pParent ) = 0;

            //! Returns true, if the node is top-level
            virtual bool IsTopLevelNode() const = 0;

            //! Returns true, if the node is top-level for dependencies
            virtual bool IsDependencyTopLevelNode() const = 0;

            //! Returns true, if this node is terminal
            virtual bool IsTerminalNode() const = 0;

            //! makes the node a writing child, i.e. it cannot be a top level node any more
            virtual void SetNotTopLevel() = 0;

            //! makes the node a writing child, i.e. it cannot be a top level node any more for dependencies
            virtual void SetNotDependencyTopLevel() = 0;

            /*!
            \brief Loop driven method for filling the list of nodes to invalidate on change.
            \param[out] NextNodesToProcess The list of nodes to process in the next step.
            */
            virtual void PropagateDependency( NodePrivateList_t &NextNodesToProcess) = 0;

            /*!
            \brief Internal use Only. Do not call. Loop driven method for filling the list of nodes to invalidate on change.
            \param[in] DependingNodes A list of nodes to the list of nodes to invalidate on change.
            \param[in] pParent The parent node making the call.
            */
            virtual bool PushDependencies( const NodeSet_t& DependingNodes, INodePrivate* pParent) = 0;
            /*!
            \brief Recursive method for seachring pSelected cycles
            \param[in,out] NodeStack The stack of previously visited nodes
            */
            virtual void CheckSelectedCycle( GenApi::NodePrivateList_t &NodeStack ) = 0;

            /*!
            \brief Recursive method for cycle checks
            \param[in,out] NodeStack The stack of previously visited nodes
            */
            virtual void CheckForReadCycles( NodePrivateList_t &NodeStack) = 0;

            /*!
            \brief Recursive method to propagate terminal dependencies.
            \return Reference to all terminal nodes node set
            */
            virtual const NodeSet_t& PropagateTerminals() =0;

            /*!
            \brief Retrieves a list of all nodes depending on this node
            \param[out] AllDependingNodes A list of all depending nodes
            */
            virtual void GetAllDependingNodes( GenApi::NodeList_t &AllDependingNodes ) const = 0;

            //! Returns the list of all terminal nodes a write access to this node will eventually write to
            virtual void GetTerminalNodes(NodeList_t &) const = 0;

        //!@}

        //! Invalidates the node if the polling time has elapsed
        /*!
            \return true : fire callback of that node
        */
        virtual bool Poll( int64_t ElapsedTime ) = 0;

        //! For performance reasons the pDependent entry is added not using
        //! SetProperty. This methods makes sure the property name pDependnet is knownh
        virtual void UpdateDependentEntry() = 0;

        //! Enforces a virtual destructor on every node class
        virtual ~INodePrivate()
        {
        }

    };


}

#endif // ifndef GENAPI_INODEPRIVATE_H
