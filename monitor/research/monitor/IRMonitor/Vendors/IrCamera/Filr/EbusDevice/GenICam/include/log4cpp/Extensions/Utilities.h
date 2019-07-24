//-----------------------------------------------------------------------------
//  (c) 2005 by Basler Vision Technologies
//  Section: Vision Components
//  Project: GenICam
//  Author:  Fritz Dierks
//  $Header$
//-----------------------------------------------------------------------------
/*!
\file
\brief    helper functions
*/

#ifndef _LOG4CPP_UTILITIES_H
#define _LOG4CPP_UTILITIES_H

#include <string>
#if defined (_MSC_VER)
#include <Windows.h>
#endif

namespace log4cpp
{
    // replaces entries of type $(VARIABLE) by the corresponding environemnt variable's content
    bool ReplaceEnvironmentVariables(std::string &Buffer);
}

#endif // _LOG4CPP_UTILITIES_H
