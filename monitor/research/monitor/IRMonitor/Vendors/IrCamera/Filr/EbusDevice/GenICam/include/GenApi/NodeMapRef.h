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
\brief    Definition of CNodeMapRef
\ingroup GenApi_PublicInterface
*/

#ifndef GENAPI_NODEMAPREF_H
#define GENAPI_NODEMAPREF_H

#include "../GenICamVersion.h"
#include "../Base/GCString.h"
#include "../Base/GCException.h"

#include "Pointer.h"
#include "NodeMapUtils.h"

#if !defined (GENICAM_XML_PATH)
#  define GENICAM_XML_PATH "$(" GENICAM_ROOT_VERSION ")/xml/GenApi/"
#endif

namespace GenApi
{

#   ifdef _WIN32

        // see below in the Linux branch
        inline INodeMapDyn *CastToINodeMapDyn(INodeMap *pNodeMap)
        {
            return dynamic_cast<INodeMapDyn *>(pNodeMap);
        }

        // see below in the Linux branch
        inline IDestroy *CastToIDestroy(INodeMap *pNodeMap)
        {
            return dynamic_cast<IDestroy *>(pNodeMap);
        }

#   else
        //! makes sure the dynamic_cast operator is implemented in the DLL (due to a Linux bug)
        GENAPI_DECL INodeMapDyn *CastToINodeMapDyn(INodeMap *pNodeMap);

        //! makes sure the dynamic_cast operator is implemented in the DLL (due to a Linux bug)
        GENAPI_DECL IDestroy *CastToIDestroy(INodeMap *pNodeMap);
#   endif

    /**
    \brief Smartpointer template for NodeMaps with create function
    \ingroup GenApi_PublicInterface
    \tparam TCameraParams  The camera specific parameter class (auto generated from camera xml file)
    */
    template<class TCameraParams>
    class CNodeMapRefT : public TCameraParams
    {
    public:
        //! Constructor
        CNodeMapRefT(GenICam::gcstring DeviceName = "Device" );

        //! Destructor
        virtual ~CNodeMapRefT();

        //! Destroys the node map
        void _Destroy();

        //! Creates the object from the default DLL
        /*! \note Can only be used if the class TCameraParams was auto generated from a specific camera xml file */
        void _LoadDLL(void);

        //! Creates the object from a DLL whose name is deduced from vendor and model name
        void _LoadDLL(GenICam::gcstring VendorName, GenICam::gcstring ModelName);

        //! Creates the object from a DLL with given file name
        void _LoadDLL(GenICam::gcstring FileName);

        //! Creates the object from a XML file whose name is deduced from vendor and model name
        void _LoadXMLFromFile(GenICam::gcstring VendorName, GenICam::gcstring ModelName);

        //! Creates the object from a XML file with given file name
        void _LoadXMLFromFile(GenICam::gcstring FileName);

        //! Creates the object from a ZIP'd XML file with given file name
        void _LoadXMLFromZIPFile(GenICam::gcstring ZipFileName);

	     //! Creates the object from a ZIP'd XML file given in a string
		void _LoadXMLFromZIPData(const void* zipData, size_t zipSize);

        //! Creates the object from a XML target and an inject file with given file name
        void _LoadXMLFromFileInject(GenICam::gcstring TargetFileName, GenICam::gcstring InjectFileName);

        //! Creates the object from XML data given in a string
        void _LoadXMLFromString(const GenICam::gcstring& XMLData);

        //! Creates the object from XML data given in a string with injection
        void _LoadXMLFromStringInject(const GenICam::gcstring& TargetXMLDataconst, const GenICam::gcstring& InjectXMLData);

        //! Loads an XML, checks it for correctness, applies a stylesheet and outputs it
        void _PreprocessXMLFromFile(const GenICam::gcstring& XMLFileName,
                                    const GenICam::gcstring& StyleSheetFileName,
                                    const GenICam::gcstring& OutputFileName,
                                    const uint32_t XMLValidation = xvDefault );

        //! Loads a Zipped XML, checks it for correctness, applies a stylesheet and outputs it
		void _PreprocessXMLFromZIPFile(const GenICam::gcstring& ZIPFileName, 
										const GenICam::gcstring& StyleSheetFileName, 
										const GenICam::gcstring& OutputFileName, 
										const uint32_t XMLValidation  = xvDefault);

        //! Injects an XML file into a target file
        virtual void _MergeXMLFiles(
            const GenICam::gcstring& TargetFileName,      //!< Name of the target XML file to process
            const GenICam::gcstring& InjectedFileName,    //!< Name of the Injected XML file to process
            const GenICam::gcstring& OutputFileName       //!< Name of the oputput file
            );

        //! Extract independent subtree
        virtual void _ExtractIndependentSubtree(
            const GenICam::gcstring& XMLData,            //!< The XML data the subtree is extracted from.
            const GenICam::gcstring& InjectXMLData,      //!< Optional XML data that is injected before extracting the subtree. No effect if an empty string is passed.
            const GenICam::gcstring& SubTreeRootNodeName,//!< The name of the node that represents the root of the subtree that shall be extracted.
            GenICam::gcstring& ExtractedSubtree          //!< The returned extracted subtree as string.
            );

        //! Gets a list of supported schema versions
        /*! Each list entry is a string with the format "{Major}.{Minor}" were {Major} and {Minor} are integers
        Example: {"1.1", "1.2"} indicates that the schema v1.1 and v1.2 are supported.
        The SubMinor version number is not given since it is for fully compatible bug fixes only
        */
        virtual void _GetSupportedSchemaVersions( GenICam::gcstring_vector &SchemaVersions );

        //! Get device name
        virtual GenICam::gcstring _GetDeviceName();

        //! Fires nodes which have a polling time
        virtual void _Poll( int64_t ElapsedTime );

        //! Clears the cache of the camera description files
        static bool _ClearXMLCache() { return ClearXMLCache(); }

        //----------------------------------------------------------------
        // INodeMap
        //----------------------------------------------------------------

        //! Retrieves all nodes in the node map
        virtual void _GetNodes(NodeList_t &Nodes) const;

        //! Retrieves the node from the central map by name
        virtual INode* _GetNode( const GenICam::gcstring& key) const;

        //! Invalidates all nodes
        virtual void _InvalidateNodes() const;

        //! Connects a port to a port node with given name
        virtual bool _Connect( IPort* pPort, const GenICam::gcstring& PortName) const;

        //! Connects a port to the standard port "Device"
        virtual bool _Connect( IPort* pPort) const;

        //! Pointer to the NodeMap
        INodeMap *_Ptr;


    private:
        //! False, when the dll already has been loaded
        bool _loaded;

        //! The name of this device
        GenICam::gcstring _DeviceName;

        CNodeMapRefT(const CNodeMapRefT&);
        CNodeMapRefT& operator=(const CNodeMapRefT&);
    };

    template<class TCameraParams>
    inline CNodeMapRefT<TCameraParams>::CNodeMapRefT(GenICam::gcstring DeviceName )
        : _Ptr(NULL)
        , _loaded(false)
        , _DeviceName(DeviceName)
    {
    }

    template<class TCameraParams>
    inline CNodeMapRefT<TCameraParams>::~CNodeMapRefT()
    {
        _Destroy();
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_Destroy()
    {
        if(_Ptr)
        {
            GenApi::IDestroy *pDestroy = CastToIDestroy(_Ptr);
            assert(pDestroy);
            pDestroy->Destroy();
            _Ptr = NULL;
        }
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadDLL(void)
    {
        _LoadDLL(TCameraParams::_GetVendorName(), TCameraParams::_GetModelName());
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadDLL(GenICam::gcstring VendorName, GenICam::gcstring ModelName)
    {
        // Load the DLL
        if( _loaded )
            throw RUNTIME_EXCEPTION("DLL already loaded");
        _Ptr = _InternalLoadDLL(VendorName, ModelName, _DeviceName);
        _loaded = true;

        // Initialize the references
        TCameraParams::_Initialize(_Ptr);
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadDLL(GenICam::gcstring FileName)
    {
        if( _loaded )
            throw RUNTIME_EXCEPTION("DLL already loaded");

        // Load the DLL
        _Ptr = _InternalLoadDLL(FileName, _DeviceName);
        _loaded = true;

        // Initialize the references
        TCameraParams::_Initialize(_Ptr);
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadXMLFromFile(GenICam::gcstring VendorName, GenICam::gcstring ModelName)
    {
        // Build the XML file name
        GenICam::gcstring FileName = GENICAM_XML_PATH + VendorName + "/" + ModelName + ".xml";
        GenICam::ReplaceEnvironmentVariables(FileName);

        // Load the XML file
        _LoadXMLFromFile(FileName);
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadXMLFromFile(GenICam::gcstring FileName)
    {
        // Load the DLL
        if( _loaded )
            throw RUNTIME_EXCEPTION("DLL already loaded");
        _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
        _loaded = true;

        // Load the XML file
        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->LoadXMLFromFile(FileName);
        
        // Initialize the references
        TCameraParams::_Initialize(_Ptr);
    }

	template<class TCameraParams>
	inline void CNodeMapRefT<TCameraParams>::_LoadXMLFromZIPFile(GenICam::gcstring ZipFileName) 
	{
        // Load the DLL
        if( _loaded )
            throw RUNTIME_EXCEPTION("DLL already loaded");
        _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
        _loaded = true;

        // Load the XML file
        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->LoadXMLFromZIPFile(ZipFileName);

        // Initialize the references
        TCameraParams::_Initialize(_Ptr);
	}

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadXMLFromFileInject(GenICam::gcstring TargetFileName, GenICam::gcstring InjectFileName)
    {
        // Load the DLL
        if( _loaded )
            throw RUNTIME_EXCEPTION("DLL already loaded");
        _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
        _loaded = true;

        // Load the XML file
        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->LoadXMLFromFileInject(TargetFileName, InjectFileName);

        // Initialize the references
        TCameraParams::_Initialize(_Ptr);
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadXMLFromString(const GenICam::gcstring& XMLData)
    {
        // Load the DLL
        if( _loaded )
            throw RUNTIME_EXCEPTION("DLL already loaded");
        _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
        _loaded = true;

        // Load the XML data
        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->LoadXMLFromString(XMLData);

        // Initialize the references
        TCameraParams::_Initialize(_Ptr);
    }


    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadXMLFromZIPData(const void* zipData, size_t zipSize)
    {
        // Load the DLL
        if( _loaded )
            throw RUNTIME_EXCEPTION("DLL already loaded");
        _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
        _loaded = true;

        // Load the XML data
        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->LoadXMLFromZIPData(zipData, zipSize);

        // Initialize the references
        TCameraParams::_Initialize(_Ptr);
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_LoadXMLFromStringInject(const GenICam::gcstring& TargetXMLData, const GenICam::gcstring& InjectXMLData)
    {
        // Load the DLL
        if( _loaded )
            throw RUNTIME_EXCEPTION("DLL already loaded");
        _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
        _loaded = true;

        // Load the XML data
        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->LoadXMLFromStringInject(TargetXMLData, InjectXMLData);

        // Initialize the references
        TCameraParams::_Initialize(_Ptr);
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_PreprocessXMLFromFile(const GenICam::gcstring& XMLFileName, const GenICam::gcstring& StyleSheetFileName, const GenICam::gcstring& OutputFileName, const uint32_t XMLValidation )
    {
        if( !_loaded )
        {
            // Load the DLL
            _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
            _loaded = true;
        }

        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->PreprocessXMLFromFile( XMLFileName, StyleSheetFileName, OutputFileName, XMLValidation );
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_PreprocessXMLFromZIPFile(const GenICam::gcstring& ZIPFileName, const GenICam::gcstring& StyleSheetFileName, const GenICam::gcstring& OutputFileName, const uint32_t XMLValidation )
    {
        if( !_loaded )
        {
            // Load the DLL
            _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
            _loaded = true;
        }

        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->PreprocessXMLFromZIPFile( ZIPFileName, StyleSheetFileName, OutputFileName, XMLValidation );
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_MergeXMLFiles( const GenICam::gcstring& TargetFileName, const GenICam::gcstring& InjectedFileName, const GenICam::gcstring& OutputFileName )
    {
        if( !_loaded )
        {
            // Load the DLL
            _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
            _loaded = true;
        }

        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->MergeXMLFiles( TargetFileName, InjectedFileName, OutputFileName );
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_ExtractIndependentSubtree( const GenICam::gcstring& XMLData, const GenICam::gcstring& InjectedXMLData, const GenICam::gcstring& SubTreeRootNodeName, GenICam::gcstring& ExtractedSubtree)
    {
        if( !_loaded )
        {
            // Load the DLL
            _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
            _loaded = true;
        }

        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->ExtractIndependentSubtree( XMLData, InjectedXMLData, SubTreeRootNodeName, ExtractedSubtree);
    }


    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_GetSupportedSchemaVersions( GenICam::gcstring_vector &SchemaVersions )
    {
        if( !_loaded )
        {
            // Load the DLL
            _Ptr = _InternalLoadDLL("Generic", "XMLLoader", _DeviceName);
            _loaded = true;
        }

        INodeMapDyn *pNodeMapDyn = CastToINodeMapDyn(_Ptr);
        assert(pNodeMapDyn);
        pNodeMapDyn->GetSupportedSchemaVersions( SchemaVersions );
    }

    template<class TCameraParams>
    inline GenICam::gcstring CNodeMapRefT<TCameraParams>::_GetDeviceName()
    {
        if(_Ptr)
            return _Ptr->GetDeviceName();
        else
            throw ACCESS_EXCEPTION("Feature not present (reference not valid)");
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_Poll( int64_t ElapsedTime )
    {
        if(_Ptr)
            return _Ptr->Poll(ElapsedTime);
        else
            throw ACCESS_EXCEPTION("Feature not present (reference not valid)");
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_GetNodes(NodeList_t &Nodes) const
    {
        if(_Ptr)
            return _Ptr->GetNodes(Nodes);
        else
            throw ACCESS_EXCEPTION("Feature not present (reference not valid)");
    }

    template<class TCameraParams>
    inline INode* CNodeMapRefT<TCameraParams>::_GetNode( const GenICam::gcstring& key) const
    {
        if(_Ptr)
            return _Ptr->GetNode(key) ;
        else
            throw ACCESS_EXCEPTION("Feature not present (reference not valid)");
    }

    template<class TCameraParams>
    inline void CNodeMapRefT<TCameraParams>::_InvalidateNodes() const
    {
        if(_Ptr)
            return _Ptr->InvalidateNodes();
        else
            throw ACCESS_EXCEPTION("Feature not present (reference not valid)");
    }

    template<class TCameraParams>
    inline bool CNodeMapRefT<TCameraParams>::_Connect( IPort* pPort, const GenICam::gcstring& PortName) const
    {
        if(_Ptr)
            return _Ptr->Connect( pPort, PortName );
        else
            throw ACCESS_EXCEPTION("Feature not present (reference not valid)");
    }

    template<class TCameraParams>
    inline bool CNodeMapRefT<TCameraParams>::_Connect( IPort* pPort) const
    {
        if(_Ptr)
            return _Ptr->Connect( pPort);
        else
            throw ACCESS_EXCEPTION("Feature not present (reference not valid)");
    }



    /**
    \brief Empty base class used by class CNodeMapRef as generic template argument
    \ingroup GenApi_PublicInterface
    */
    class CGeneric_XMLLoaderParams
    {
    protected:
        virtual void _Initialize(GenApi::INodeMap*) {}
    };


    /**
    \brief Smartpointer for NodeMaps with create function
    \ingroup GenApi_PublicInterface
    \note This class is a simple typedef definition. The class syntax is only used,
          because Doxygen has to generate a useful documentation.
    */
    class CNodeMapRef : public CNodeMapRefT<GenApi::CGeneric_XMLLoaderParams>
    {
    public:
        //! Constructor
        CNodeMapRef(GenICam::gcstring DeviceName = "Device" )
            : CNodeMapRefT<GenApi::CGeneric_XMLLoaderParams>(DeviceName)
        {
        }
    };


}

#endif // ifndef GENAPI_NODEMAPPTR_H
