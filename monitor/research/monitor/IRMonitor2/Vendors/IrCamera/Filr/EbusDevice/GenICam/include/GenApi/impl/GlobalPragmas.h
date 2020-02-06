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
\brief    This file must be included FIRST
\ingroup GenApi_PublicImpl
*/

#ifndef GENAPI_GLOBALS_H
#define GENAPI_GLOBALS_H

// GenICam should be compiles on warning level 4
// The following warnings occurr frequently but can be savely ignored

#pragma warning ( disable : 4068 ) // unknown pragma; refers tio BullsEyeCoverage
#pragma warning ( disable : 4251 ) // XXX needs to have dll-interface to be used by clients of class YYY
#pragma warning( disable: 4702 ) // unreachable code in <list>,...
#pragma warning( disable: 4275 ) // non dll-interface structXXX used as base
#pragma warning( disable: 4312 ) // 'type cast' : conversion from 'uintptr_t' to 'void *' of greater size

#if _MSC_VER==1310  // Visual C++ .NET 2003
#   pragma warning( disable: 4512 ) // assigment operator could not be generated (VC71 only)
#   pragma warning( disable: 4724 ) // potential mod by 0 (VC71 only)
#   pragma warning( disable: 4702 ) // unreachable code  (VC71 only)
#endif

#endif // ifndef GENAPI_IVALUE_H
