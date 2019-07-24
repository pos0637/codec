/****************************************************************************
(c) 2005 by STEMMER IMAGING

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


****************************************************************************/

/// \file
/// \brief    Portable string implementation
/// \ingroup Base_PublicUtilities


#ifndef GENICAM_GCSTRING_H
#define GENICAM_GCSTRING_H

#include <string>

#include <iostream>
#include "GCTypes.h"

#pragma pack(push, 8)


/** 
\brief Indicates either 'not found' or 'all remaining characters'.
\ingroup GenApi_Implementation
*/
#define GCSTRING_NPOS size_t(-1)

namespace GenICam
{
    /**
    \brief A string class which is a clone of std::string
    \ingroup Base_PublicUtilities
    */
    class GCBASE_API gcstring
    {
#       if defined(_MSC_VER) && !defined(PHARLAP_WIN32)
            //! Helper class for returning wchar_t* on the stack
            class GCBASE_API gcwchar 
            {
            public:
                //! Constructor taking ownership of the memory block allocated with new on the heap
                explicit gcwchar( const wchar_t *pBuffer = NULL );

                // copy constructor taking ownership
                gcwchar( gcwchar& _gwchar );

                //! cast operator to (wchar_t*)
                operator const wchar_t *() const;

                //! destructor freeing the memory block handed in through the constructor
                ~gcwchar();
            protected:
                //! the memory block owned by this class
                const wchar_t *m_pBuffer;

            private:
                // no assignment constructor
                gcwchar& operator=( const gcwchar& );

            };

#       endif

        // Ctor / Dtor
        // -------------------------------------------------------------------------
    public:
        gcstring                ();
        gcstring                ( const char *pc );
        gcstring                ( size_t count, char ch );
        gcstring                ( const gcstring &str );
#       if defined(_MSC_VER) && !defined(PHARLAP_WIN32)
            explicit gcstring   ( const wchar_t *pBufferUTF16 );
#       endif
        virtual  ~gcstring      ( void );

        // Data access
        // -------------------------------------------------------------------------
    public:
        virtual gcstring &  append                ( const gcstring &str );
        virtual gcstring &  append                ( size_t count, char ch );
        virtual gcstring &  assign                ( const gcstring &str );
        virtual gcstring &  assign                ( size_t count, char ch );
#       if defined(_MSC_VER) && !defined(PHARLAP_WIN32)
            virtual gcstring &  assign            ( const wchar_t *pStringUTF16 );
#       endif
        virtual int         compare               ( const gcstring &str )   const;
#       if defined(_MSC_VER) && !defined(PHARLAP_WIN32)
            virtual gcwchar w_str                 ( void )                  const;
#       endif
        virtual const char *c_str                 ( void )                  const;
        virtual bool        empty                 ( void )                  const;
        virtual size_t      find                  ( char ch, size_t offset = 0 ) const;
        virtual size_t      find                  ( const gcstring &str, size_t offset = 0 ) const;
        virtual size_t      find                  ( const gcstring &str, size_t offset, size_t count ) const;
        virtual size_t      find                  ( const char* pc, size_t offset = 0) const;
        virtual size_t      find                  ( const char* pc, size_t offset, size_t count ) const;
        virtual size_t      length                ( void )                  const;
        virtual size_t      size                  ( void )                  const;
        virtual void        resize                ( size_t n );
        virtual size_t      max_size              ( )                       const;
        virtual    gcstring    substr             ( size_t offset = 0, size_t count = GCSTRING_NPOS ) const;
        virtual size_t find_first_of              ( const gcstring &str, size_t offset = 0 ) const;
        virtual size_t find_first_not_of          ( const gcstring &str, size_t offset = 0 ) const;
        static size_t      _npos                  ( void );
        virtual void        swap                  ( gcstring &Right );


        // Operators
        // -------------------------------------------------------------------------
    public:
        bool                operator !=           ( const gcstring &str )   const;
        bool                operator !=           ( const char *pc )        const;
        gcstring &          operator +=           ( const gcstring &str );
        gcstring            operator +=           ( const gcstring &str )   const;
        gcstring &          operator +=           ( const char *pc );

        gcstring &          operator +=           ( char ch );
        gcstring            operator +=           ( char ch )               const;
        gcstring &          operator =            ( const gcstring &str );
#       if defined(_MSC_VER) && !defined(PHARLAP_WIN32)
            gcstring &          operator =        ( const wchar_t *pStringUTF16 );
#       endif
        bool                operator ==           ( const gcstring &str )   const;
        bool                operator ==           ( const char *pc )        const;
        bool                operator <            ( const gcstring &str )   const;
        bool                operator >            ( const gcstring &str )   const;
        operator const char * ( void )                  const;
        void                operator delete       ( void *pWhere );
        void                operator delete       ( void *pWhere, void *pNewWhere );
        void *              operator new          ( size_t uiSize );
        void *              operator new          ( size_t uiSize, void *pWhere );
        GCBASE_API
            friend gcstring     operator +        ( const gcstring &left, const gcstring &right );
        GCBASE_API
            friend gcstring     operator +        ( const gcstring &left, const char *right );
        GCBASE_API
            friend gcstring     operator +        ( const char *left, const gcstring &right );

        // Member
        // -------------------------------------------------------------------------
    private:
        // redundant pointer to make the possible to see the contents of the string in the debugger
        const char* m_psz;
        // actual std::string object
        uint8_t m_opaqueData[64];
        
        const std::string& GetInternalString() const;
        std::string& GetInternalString();

        // Constants
        // -------------------------------------------------------------------------
    public:
        // Beware : this static member prevents delay loading
        // use _npos() instead
        static const size_t npos;
    };

    GCBASE_API
        std::istream &  getline ( std::istream& is, GenICam::gcstring& str );
    GCBASE_API
        std::istream &  getline ( std::istream& is, GenICam::gcstring& str, char delim );
}

//! STL operator out
//! \ingroup Base_PublicUtilities
inline std::ostream & operator <<(std::ostream &ostr, const GenICam::gcstring &str) { return ostr << str.c_str(); }

//! STL operator in
//! \ingroup Base_PublicUtilities
inline std::istream & operator >>(std::istream &istr, GenICam::gcstring &str)
{
    std::string tmp;
    istr >> tmp;
    str = tmp.c_str();
    return istr;
}

#pragma pack(pop)

#endif // GENICAM_GCSTRING_H
