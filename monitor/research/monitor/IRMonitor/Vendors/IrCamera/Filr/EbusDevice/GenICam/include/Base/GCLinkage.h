//{{NO_DEPENDENCIES}}
//@tab=(8,4)

//-----------------------------------------------------------------------------
//  (c) 2007 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenICam
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
\brief    helpers for pragma linkage
*/

#ifndef LINKAGE_H
#define LINKAGE_H

#include "GenICamVersion.h"

#if defined (_MSC_VER)

#   if defined (_WIN32) && defined (_MT )
#       if !defined(GENICAM_BUILD)
            // for the central installation always the Release version is used
#           define CONFIGURATION "MD"
#       else
#           if defined(_DEBUG) || defined(DEBUG)
#               define CONFIGURATION "MDd"
#           else
#               define CONFIGURATION "MD"
#           endif
#       endif
#   else
#       error Invalid configuration
#   endif

#   if defined(COMPILER) // COMPILER  may be force set from outside
#       undef GENICAM_COMPILER_STR
#       define GENICAM_COMPILER_STR COMPILER
#   endif

    // _MSC_VER==1310 : VC71  : Visual C++ .NET 2003
    // _MSC_VER==1400 : VC80  : Visual C++ 2005
    // _MSC_VER==1500 : VC90  : Visual C++ 2008
    // _MSC_VER==1600 : VC100 : Visual C++ 2010
    // _MSC_VER==1700 : VC110 : Visual C++ 2012
#   if !( _MSC_VER==1310 || _MSC_VER==1400 || _MSC_VER==1500 || _MSC_VER==1600 || _MSC_VER==1700 || _MSC_VER==1800 || _MSC_VER==1900 )
#       error Invalid compiler
#   endif

#   define GENICAM_SUFFIX( CONFIGURATION, GENICAM_COMPILER_STR, VERSION_MAJOR, VERSION_MINOR, EXTENSION ) \
        "_" CONFIGURATION "_" GENICAM_COMPILER_STR "_v" VERSION_MAJOR "_" VERSION_MINOR "." EXTENSION

#   define LIB_SUFFIX \
        GENICAM_SUFFIX( CONFIGURATION, GENICAM_COMPILER_STR, GENICAM_VERSION_MAJOR_STR, GENICAM_VERSION_MINOR_STR, "lib" )
#   define DLL_SUFFIX \
        GENICAM_SUFFIX( CONFIGURATION, GENICAM_COMPILER_STR, GENICAM_VERSION_MAJOR_STR, GENICAM_VERSION_MINOR_STR, "dll" )
#   define EXE_SUFFIX \
        GENICAM_SUFFIX( CONFIGURATION, GENICAM_COMPILER_STR, GENICAM_VERSION_MAJOR_STR, GENICAM_VERSION_MINOR_STR, "exe" )

#elif defined (__GNUC__) && (defined (__linux__) || defined(__APPLE__))
#   if defined (NDEBUG)
#       define CONFIGURATION ""
#   else
#       define CONFIGURATION "_d"
#   endif

#   define GENICAM_SUFFIX( CONFIGURATION, VERSION_MAJOR, VERSION_MINOR, EXTENSION ) \
        CONFIGURATION "." VERSION_MAJOR "." VERSION_MINOR "." EXTENSION

#   define LIB_SUFFIX \
        GENICAM_SUFFIX( CONFIGURATION, GENICAM_VERSION_MAJOR_STR, GENICAM_VERSION_MINOR_STR, "a" )
#   if defined(__linux__)
#   define DLL_SUFFIX \
        GENICAM_SUFFIX( CONFIGURATION, GENICAM_VERSION_MAJOR_STR, GENICAM_VERSION_MINOR_STR, "so" )
#   else
#       define DLL_SUFFIX \
            GENICAM_SUFFIX( CONFIGURATION, GENICAM_VERSION_MAJOR_STR, GENICAM_VERSION_MINOR_STR, "dylib" )
#   endif
#   define EXE_SUFFIX \
        GENICAM_SUFFIX( CONFIGURATION, GENICAM_VERSION_MAJOR_STR, GENICAM_VERSION_MINOR_STR, "" )

#else
#   error Unknown shared library support
#endif

#define LIB_NAME( MODULE ) \
     MODULE LIB_SUFFIX

#define DLL_NAME( MODULE ) \
     MODULE DLL_SUFFIX

#define EXE_NAME( MODULE ) \
     MODULE EXE_SUFFIX

#endif // LINKAGE_H
