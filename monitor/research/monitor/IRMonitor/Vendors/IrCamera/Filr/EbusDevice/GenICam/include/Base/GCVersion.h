//{{NO_DEPENDENCIES}}
//@tab=(8,4)

//-----------------------------------------------------------------------------
//  (c) 2007 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenICam
//  Author:  Fritz Dierks
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
\brief    central versioning utilities
*/

/* Nothing below this point has to be modified to adjust version data.
 *-------------------------------------------------------------------*/

#include "GenICamVersion.h"

#if defined (_DEBUG) || defined (DEBUG)
  #define GENICAM_DEBUGSTRING " (debug)"
#else
  #define GENICAM_DEBUGSTRING ""
#endif

#ifndef GENICAM_FILE_DESCRIPTION
  #if defined(GENICAM_EXE)
    #define GENICAM_FILE_DESCRIPTION(name) #name " Application\0"
  #else
    #define GENICAM_FILE_DESCRIPTION(name) #name " Module\0"
  #endif
#endif

#if GENICAM_VERSION_PRIVATEBUILD
  #define GENICAM_FILE_FLAGS2 VS_FF_PRIVATEBUILD
#else
  #define GENICAM_FILE_FLAGS2 0
#endif

#if defined (_DEBUG) || defined (DEBUG)
  #define GENICAM_FILE_FLAGS GENICAM_FILE_FLAGS2|VS_FF_DEBUG
#elif GENICAM_VERSION_PRERELEASE
  #define GENICAM_FILE_FLAGS GENICAM_FILE_FLAGS2|VS_FF_PRERELEASE
#else
  #define GENICAM_FILE_FLAGS 0
#endif


#if defined(GENICAM_EXE)
  #define GENICAM_FILETYPE VFT_APP
  #define GENICAM_FILESUBTYPE VFT2_UNKNOWN
  #define GENICAM_ORIGINAL_FILENAME(name) #name ".exe\0"
#elif defined(GENICAM_DLL)
  #define GENICAM_FILETYPE VFT_DLL
  #define GENICAM_FILESUBTYPE VFT2_UNKNOWN
  #define GENICAM_ORIGINAL_FILENAME(name) #name ".dll\0"
#else
  #error(Undefined file type)
#endif


#define GENICAM_VERSION(name, priv) \
    LANGUAGE LANG_NEUTRAL, SUBLANG_DEFAULT \
        VS_VERSION_INFO VERSIONINFO \
        FILEVERSION GENICAM_VERSION_MAJOR, GENICAM_VERSION_MINOR, GENICAM_VERSION_SUBMINOR, GENICAM_VERSION_BUILD \
        PRODUCTVERSION GENICAM_VERSION_MAJOR, GENICAM_VERSION_MINOR, GENICAM_VERSION_SUBMINOR, GENICAM_VERSION_BUILD \
        FILEFLAGSMASK VS_FFI_FILEFLAGSMASK \
        FILEFLAGS GENICAM_FILE_FLAGS \
        FILEOS VOS_NT_WINDOWS32 \
        FILETYPE GENICAM_FILETYPE \
        FILESUBTYPE GENICAM_FILESUBTYPE \
    BEGIN \
        BLOCK "StringFileInfo" \
        BEGIN \
            BLOCK "000004b0" \
            BEGIN \
                VALUE "Comments", "\0" \
                VALUE "CompanyName", "GenICam Standard Group\0" \
                VALUE "FileDescription", GENICAM_FILE_DESCRIPTION(name) "\0" \
                VALUE "FileVersion", GENICAM_VERSION_MAJOR_STR "." GENICAM_VERSION_MINOR_STR "." GENICAM_VERSION_SUBMINOR_STR "." GENICAM_VERSION_BUILD_STR GENICAM_DEBUGSTRING "\0" \
                VALUE "InternalName", #name "\0" \
                VALUE "LegalCopyright", "Copyright (c) 2007 EMVA - All rights reserved.\0" \
                VALUE "LegalTrademarks", "GenICam\0" \
                VALUE "OLESelfRegister", "\0" \
                VALUE "OriginalFilename", GENICAM_ORIGINAL_FILENAME(name) \
                VALUE "PrivateBuild", "\0" \
                VALUE "ProductName", "GenICam Reference Implementation\0" \
                VALUE "ProductVersion", GENICAM_VERSION_MAJOR_STR "." GENICAM_VERSION_MINOR_STR "." GENICAM_VERSION_SUBMINOR_STR "." GENICAM_VERSION_BUILD_STR GENICAM_DEBUGSTRING "\0" \
                VALUE "SpecialBuild", #priv \
            END \
        END \
    BLOCK "VarFileInfo" \
    BEGIN \
        VALUE "Translation", 0x0, 1200 \
    END \
    END

