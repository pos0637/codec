//-----------------------------------------------------------------------------
//  (c) 2006 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenApi
//  Author:  Margret Albrecht
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
\brief    Definition of CEnumEntry
\ingroup GenApi_Implementation
*/

#ifndef GENAPI_ENUMENTRY_H
#define GENAPI_ENUMENTRY_H

#include "../IEnumEntry.h"
#include "Node.h"
#include "NodeT.h"
#include "BaseT.h"
#include "ValueT.h"
#include "EnumEntry.h"

#pragma warning ( push )
#pragma warning ( disable : 4275 ) // non dll-interface XXX used as base for  dll-interface class YYY

namespace GenApi
{

    //! Mapping of Enum Values to symbolic values
    class GENAPI_DECL CEnumEntryImpl: public CNodeImpl, public IEnumEntry
    {
    public:
        //-------------------------------------------------------------
        //! \name Constructor / destructor
        //@{
            //! Constructor
            CEnumEntryImpl();
        //@}

        //-------------------------------------------------------------
        //! \name Interface IBase (methods are called by the NodeT class)
        //@{

            //! Get the access mode of the node
            virtual EAccessMode InternalGetAccessMode() const;


            //! Implementation of IBase::GetPrincipalInterfaceType()
            virtual EInterfaceType InternalGetPrincipalInterfaceType() const
            {
                return intfIEnumEntry;
            }
        //@}

        //-------------------------------------------------------------
        //! \name Interface INode (methods are called by the NodeT class)
        //@{
            //! Returns the name to display
            GenICam::gcstring InternalGetDisplayName() const;
        //@}

        //-------------------------------------------------------------
        //! \name Interface INodePrivate
        //@{
            //! checks if the node is terminal
            virtual bool IsTerminalNode() const;

            //! finishes the constriction of the node
            virtual void FinalConstruct();
        //@}

        //-------------------------------------------------------------
        //! \name Interface IValue (methods are called by the ValueT class)
        //@{
            //! Get value of the node as string
            virtual GenICam::gcstring InternalToString(bool Verify = false, bool IgnoreCache = false);

            //! Set content of the node as string (method not used)
            virtual void InternalFromString(const GenICam::gcstring& ValueStr, bool Verify=true);
        //@}


        //-------------------------------------------------------------
        //! \name Interface IEnumEntry
        //@{
            virtual GenICam::gcstring GetSymbolic() const;
            virtual int64_t GetValue();
            virtual double GetNumericValue();
            virtual bool IsSelfClearing();
        //@}


        //! Convert map
        BEGIN_CONVERT_MAP
            CHAIN_CONVERT_MAP(CNodeImpl)
            SWITCH_CONVERT_MAP
            CONVERT_ENTRY(Value_ID, m_Value)
            CONVERT_ENTRY(NumericValue_ID, m_NumericValue)
            CONVERT_STRING_ENTRY(Symbolic_ID, m_Symbolic)
            CONVERT_ENUM_ENTRY(IsSelfClearing_ID, m_IsSelfClearing, EYesNoClass)
        END_CONVERT_MAP

    private:
        //! the integer value of this entry
        int64_t m_Value;

        //! the symbolic name of this entry
        GenICam::gcstring m_Symbolic;

        //! the numeric value associated with this enum entry
        double m_NumericValue;

        //! Yes if this values will be changed back by the device
        EYesNo m_IsSelfClearing;

    };

    //! Implements the EnumEntry node
    class GENAPI_DECL CEnumEntry : public BaseT< ValueT< NodeT < CEnumEntryImpl > > >
    {
    };
}

#pragma warning ( pop )

#endif // ifndef GENAPI_ENUMENTRY_H
