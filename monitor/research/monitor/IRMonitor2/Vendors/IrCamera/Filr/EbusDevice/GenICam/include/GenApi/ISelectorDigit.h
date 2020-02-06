//-----------------------------------------------------------------------------
//  (c) 2010 by Basler Vision Technologies
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
*/

#ifndef GENAPI_ISELECTORDIGIT_H
#define GENAPI_ISELECTORDIGIT_H

#include "GenApiDll.h"
#include "Types.h"
#include "Container.h"

namespace GenApi
{

    //! Interface of a "digit" of the "counter" formed by the selector set
    interface GENAPI_DECL_ABSTRACT ISelectorDigit
    {
        /*! \brief Sets digit to start value
            \return true if the resulting value is valid */
        virtual bool SetFirst() = 0;

        /*! \brief Sets digit to next value
            \param Tick if false the counter does not tick (but realize it could have)
            \return true if the resulting value is valid */
        virtual bool SetNext(bool Tick = true) = 0;

        //! Restores the selectors' values found at creation
        virtual void Restore() = 0;

        //! Returns a string representation of the digit
        virtual GenICam::gcstring ToString() = 0;

        //! Retrieves an ordered list of selectors
        virtual void GetSelectorList( 
            GenApi::FeatureList_t &SelectorList, //!> List to contain the selector pointer
            bool Incremental = false  //!> if true only seletor changed since the last GetNext are contained 
            ) = 0;
    };

}

#endif

