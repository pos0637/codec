#ifndef __PLEORA_MACROS_H__
#define __PLEORA_MACROS_H__

#include "Base/GCException.h"
#include "../GenApiDll.h"
#include "../Types.h"


#define pDependent_ID 0
#define IsFeature_ID 1
#define NameSpace_ID 2
#define pTerminal_ID 3
#define Visibility_ID 4
#define Streamable_ID 5
#define Cachable_ID 6
#define pIsImplemented_ID 7
#define Description_ID 8
#define ToolTip_ID 9
#define pIsLocked_ID 10
#define pSelected_ID 11
#define pSelecting_ID 12
#define EventID_ID 13
#define pInvalidator_ID 14
#define Name_ID 15
#define NodeType_ID 16
#define DeviceName_ID 17
#define pIsAvailable_ID 18
#define pBlockPolling_ID 19
#define pError_ID 20
#define pAlias_ID 21
#define pCastAlias_ID 22
#define ImposedAccessMode_ID 23
#define ImposedVisibility_ID 24
#define PollingTime_ID 25
#define DocuURL_ID 26
#define IsDeprecated_ID 27
#define DisplayName_ID 28
#define Value_ID 29
#define pValue_ID 30
#define OnValue_ID 31
#define OffValue_ID 32
#define pFeature_ID 33
#define AccessMode_ID 34
#define pPort_ID 35
#define Length_ID 36
#define Address_ID 37
#define pLength_ID 38
#define pAddress_ID 39
#define pIndex_ID 40
#define CommandValue_ID 41
#define pCommandValue_ID 42
#define pValueCopy_ID 43
#define pMin_ID 44
#define pMax_ID 45
#define pInc_ID 46
#define pValueDefault_ID 47
#define Min_ID 48
#define Max_ID 49
#define Inc_ID 50
#define ValueDefault_ID 51
#define Representation_ID 52
#define ValidValueSet_ID 53
#define Unit_ID 54
#define ValueIndexed_ID 55
#define pValueIndexed_ID 56
#define DisplayPrecision_ID 57
#define DisplayNotation_ID 58
#define NumericValue_ID 59
#define Symbolic_ID 60
#define IsSelfClearing_ID 61
#define pEnumEntry_ID 62
#define Key_ID 63
#define p1212Parser_ID 64
#define FeatureID_ID 65
#define Timeout_ID 66
#define Endianess_ID 67
#define ChunkID_ID 68
#define pChunkID_ID 69
#define SwapEndianess_ID 70
#define CacheChunkData_ID 71
#define Sign_ID 72
#define LSB_ID 73
#define MSB_ID 74
#define Formula_ID 75
#define Input_ID 76
#define pVariable_ID 77
#define FormulaTo_ID 78
#define FormulaFrom_ID 79
#define Slope_ID 80
#define IsLinear_ID 81
#define ModelName_ID 82
#define VendorName_ID 83
#define StandardNameSpace_ID 84
#define SchemaMajorVersion_ID 85
#define SchemaMinorVersion_ID 86
#define SchemaSubMinorVersion_ID 87
#define MajorVersion_ID 88
#define MinorVersion_ID 89
#define SubMinorVersion_ID 90
#define ProductGuid_ID 91
#define VersionGuid_ID 92
#define Extension_ID 93

namespace GenApi
{
    GENAPI_DECL int GetIDFromMap( const GenICam::gcstring &aName ); 
}

//
// IMPORTANT - READ IF ASSERTS START FLYING ALL OVER THE PLACE AFTER EDITING THE SCHEMA!!
//
// The super fast hash used to accelerate strings comparisons on XML loads (both cache
// and uncached) is generated using gperf. It is used to generate the PerfectHash.h 
// file of this project.
//
// 1. Download, install gperf 
//    see http://gnuwin32.sourceforge.net/packages/gperf.htm
// 2. Copy the list below (edited) in a text file
//    for example Input.txt
// 3. Run gperf against PerfectHash.h with C++ option selected
//    gperf.exe -L=C++ Input.txt > PerfectHash.h
// 4. Rename output to PerfectHash.h, overwrite GenApi file
// 5. Make sure all IDs are defined above
// 6. Edit constructor of IDMap in StringsToID, make sure new strings/IDs pairs are there
// 7. In debug mode, asserts should thoroughly validate if you got it right.
//
// This sounds like a lot to maintain, but the speed boost appears to be worth it.
//
// It would eventually make sense to run gperf as a pre-compilation step of GenApi.
//
// For questions, feel free to contact francois.gobeil@pleora.com
//
// HISTORY of perfect hash maintenance
//
// 2009-10-08 Francois Gobeil: Creation, up-to-date with 1.2 GenApi (2.0 schema)
//

//
// gperf list 
//

/*
pDependent
IsFeature
NameSpace
pTerminal
Visibility
Streamable
Cachable
pIsImplemented
Description
ToolTip
pIsLocked
pSelected
pSelecting
EventID
pInvalidator
Name
NodeType
DeviceName
pIsAvailable
pBlockPolling
pError
pAlias
pCastAlias
ImposedAccessMode
ImposedVisibility
PollingTime
DocuURL
IsDeprecated
DisplayName
Value
pValue
OnValue
OffValue
pFeature
AccessMode
pPort
Length
Address
pLength
pAddress
pIndex
CommandValue
pCommandValue
pValueCopy
pMin
pMax
pInc
pValueDefault
Min
Max
Inc
ValueDefault
Representation
ValidValueSet
Unit
ValueIndexed
pValueIndexed
DisplayPrecision
DisplayNotation
NumericValue
Symbolic
IsSelfClearing
pEnumEntry
Key
p1212Parser
FeatureID
Timeout
Endianess
ChunkID
pChunkID
SwapEndianess
CacheChunkData
Sign
LSB
MSB
Formula
Input
pVariable
FormulaTo
FormulaFrom
Slope
IsLinear
ModelName
VendorName
StandardNameSpace
SchemaMajorVersion
SchemaMinorVersion
SchemaSubMinorVersion
MajorVersion
MinorVersion
SubMinorVersion
ProductGuid
VersionGuid
*/


#endif // __PLEORA_MACROS__
