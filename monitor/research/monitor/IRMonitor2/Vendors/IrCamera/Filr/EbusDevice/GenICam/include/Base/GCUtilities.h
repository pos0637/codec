//-----------------------------------------------------------------------------
//  (c) 2005 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenICam
//  Author:  Fritz Dierks
//  $Header$
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
\brief    GenICam common utilities
\ingroup Base_PublicUtilities
*/

#ifndef GENAPI_GENAPIUTILITIES_DEF_H_
#define GENAPI_GENAPIUTILITIES_DEF_H_

#if defined (_WIN32)
# include <windows.h>
#endif

#include "../GenICamVersion.h"

#include "GCTypes.h"
#include "GCString.h"
#include "GCStringVector.h"
#include "GCException.h"
#include "GCLinkage.h"


#if defined (_MSC_VER)
#   if defined (_WIN64)
#       define PLATFORM_NAME "Win64_x64"
#   else
#       define PLATFORM_NAME "Win32_i86"
#   endif
#elif defined (__GNUC__)
#   if defined (__LP64__)
#      if defined (__linux__)
#       define PLATFORM_NAME "Linux64_x64"
#      elif defined (__APPLE__)
#       define PLATFORM_NAME "Maci64_x64"
#   else
#       error Unknown Platform
#      endif
#   else
#      if defined (__linux__)
#       define PLATFORM_NAME "Linux32_i86"
#      elif defined (__APPLE__)
#       error Unsupported Platform
#      else
#       error Unknown Platform
#      endif
#   endif
#else
#   error Unknown Platform
#endif

#ifndef GC_COUNTOF
#   define GC_COUNTOF(arr) (sizeof (arr) / sizeof (arr)[0] )
#endif

namespace GenICam
{
    //! This verifies at runtime if there was no loss of data if an type Ts (e.g. int64t) was downcast
    //! to type Td (e.g. int32_t)
    template<typename Td, typename Ts>
    inline Td INTEGRAL_CAST2( Ts s )
    {
        const Td d = static_cast<Td>( s );
        if ( static_cast<Ts>( d ) != s ){
            throw RUNTIME_EXCEPTION("INTEGRAL_CAST failed");
        }
        return d;
    }

    //! This verifies at runtime if there was no loss of data if an int64_t was downcast
    //! to type T (e.g. int32_t)
    template<typename T>
    inline T INTEGRAL_CAST( int64_t ll )
    {
        return INTEGRAL_CAST2<T, int64_t>( ll );
    }

    //! Returns true if an environment variable exists
    GCBASE_API bool DoesEnvironmentVariableExist( const gcstring &VariableName );

    //! Retrieve the value of an environment variable
    //! \throw runtime_exception if not found
    GCBASE_API gcstring GetValueOfEnvironmentVariable( const gcstring &VariableName );

    //! Converts \ to / and replaces all unsave characters by their %xx equivalent
    //! \ingroup Base_PublicUtilities
    GCBASE_API gcstring UrlEncode(const GenICam::gcstring& Input);

    //! Replaces %xx escapes by their char equivalent
    //! \ingroup Base_PublicUtilities
    GCBASE_API GenICam::gcstring UrlDecode(const GenICam::gcstring& Input);

    //! Replaces $(ENVIRONMENT_VARIABLES) in a string and replace ' ' with %20
    //! \ingroup Base_PublicUtilities
    GCBASE_API void ReplaceEnvironmentVariables(gcstring &Buffer, bool ReplaceBlankBy20 = false);

    //! retrieve the root folder of the GenICam installation
    /*! This function retrieves the content of the environment variable GENICAM_ROOT_V1_2
        whereas the actual version numbers depend on the GenICam version used
    */
    GCBASE_API gcstring GetGenICamRootFolder(void);

    //! retrieves the bin folder of the GenICam installation
    GCBASE_API gcstring GetGenICamDLLFolder(void);

    //! retrieves the full name of a module's DLL
    GCBASE_API gcstring GetGenICamDLLName(const gcstring&  ModuleName );

    //! retrieves the full path of a module's DLL
    GCBASE_API gcstring GetGenICamDLLPath(const gcstring&  ModuleName );

    //! splits str input string into a list of tokens using the delimiter
    GCBASE_API void Tokenize(
        const gcstring& str,                    //!< string to be split
        gcstring_vector& tokens,          //!< result of the splitting operation
        const gcstring& delimiters = " "  //!< delimiters for the splitting
        );

    //! Gets a list of files or directories matching a given FileTemplate
    GCBASE_API void GetFiles(
        const gcstring &FileTemplate,           //!> The file template. Can contain environment variables like, e.g. $(GENICAM_ROOT)
        gcstring_vector &FileNames,             //!> A list of files matching the file template
        const bool DirectoriesOnly = false );   //!> true = only subdirectories (ex . and ..) are retrieved; false = only files are retrieved


#define GENICAM_UNUSED(unused_var)    ((void)(unused_var))
}

#endif // GENAPI_GENAPIUTILITIES_DEF_H_
