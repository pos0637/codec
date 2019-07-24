#ifndef __PERFECT_HASH_H__
#define __PERFECT_HASH_H__

/* C code produced by gperf version 3.0.1 */
/* Command-line: 'c:\\Program Files (x86)\\GnuWin32\\bin\\gperf.exe' perfecthash.txt  */
/* Computed positions: -k'1,3,8' */

#if !((' ' == 32) && ('!' == 33) && ('"' == 34) && ('#' == 35) \
      && ('%' == 37) && ('&' == 38) && ('\'' == 39) && ('(' == 40) \
      && (')' == 41) && ('*' == 42) && ('+' == 43) && (',' == 44) \
      && ('-' == 45) && ('.' == 46) && ('/' == 47) && ('0' == 48) \
      && ('1' == 49) && ('2' == 50) && ('3' == 51) && ('4' == 52) \
      && ('5' == 53) && ('6' == 54) && ('7' == 55) && ('8' == 56) \
      && ('9' == 57) && (':' == 58) && (';' == 59) && ('<' == 60) \
      && ('=' == 61) && ('>' == 62) && ('?' == 63) && ('A' == 65) \
      && ('B' == 66) && ('C' == 67) && ('D' == 68) && ('E' == 69) \
      && ('F' == 70) && ('G' == 71) && ('H' == 72) && ('I' == 73) \
      && ('J' == 74) && ('K' == 75) && ('L' == 76) && ('M' == 77) \
      && ('N' == 78) && ('O' == 79) && ('P' == 80) && ('Q' == 81) \
      && ('R' == 82) && ('S' == 83) && ('T' == 84) && ('U' == 85) \
      && ('V' == 86) && ('W' == 87) && ('X' == 88) && ('Y' == 89) \
      && ('Z' == 90) && ('[' == 91) && ('\\' == 92) && (']' == 93) \
      && ('^' == 94) && ('_' == 95) && ('a' == 97) && ('b' == 98) \
      && ('c' == 99) && ('d' == 100) && ('e' == 101) && ('f' == 102) \
      && ('g' == 103) && ('h' == 104) && ('i' == 105) && ('j' == 106) \
      && ('k' == 107) && ('l' == 108) && ('m' == 109) && ('n' == 110) \
      && ('o' == 111) && ('p' == 112) && ('q' == 113) && ('r' == 114) \
      && ('s' == 115) && ('t' == 116) && ('u' == 117) && ('v' == 118) \
      && ('w' == 119) && ('x' == 120) && ('y' == 121) && ('z' == 122) \
      && ('{' == 123) && ('|' == 124) && ('}' == 125) && ('~' == 126))
/* The character set is not based on ISO-646.  */
error "gperf generated tables don't work with this execution character set. Please report a bug to <bug-gnu-gperf@gnu.org>."
#endif


#define TOTAL_KEYWORDS 93
#define MIN_WORD_LENGTH 3
#define MAX_WORD_LENGTH 21
#define MIN_HASH_VALUE 3
#define MAX_HASH_VALUE 180
/* maximum key range = 178, duplicates = 0 */

#ifdef __GNUC__
__inline
#else
#ifdef __cplusplus
inline
#endif
#endif
static unsigned int
hash (const char *str, unsigned int len)
{
  static unsigned char asso_values[] =
    {
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
       30, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181,  70,  70,  25,  10,  90,
       15,  25, 181,   0, 181,   0,  15,  60,   0,   0,
       10, 181,  55,  35,  20,  40,   0, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181,  10,  10,  60,
       10,   0,   0,  30,  15,  45,   0, 181,  15,   5,
       20,  40,   5, 181,  35,  30,  10,  20,   0, 181,
       35,   0, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181, 181, 181, 181, 181,
      181, 181, 181, 181, 181, 181
    };
  register int hval = len;

  switch (hval)
    {
      default:
        hval += asso_values[(unsigned char)str[7]];
      /*FALLTHROUGH*/
      case 7:
      case 6:
      case 5:
      case 4:
      case 3:
        hval += asso_values[(unsigned char)str[2]];
      /*FALLTHROUGH*/
      case 2:
      case 1:
        hval += asso_values[(unsigned char)str[0]];
        break;
    }
  return hval;
}

/*#ifdef __GNUC__
__inline
#endif
const char *
in_word_set (const char *str, unsigned int len)
{
  static const char * wordlist[] =
    {
      "", "", "",
      "Key",
      "", "", "",
      "OnValue",
      "OffValue",
      "Name",
      "Input",
      "",
      "pLength",
      "pFeature",
      "pSelected",
      "pDependent",
      "",
      "NumericValue",
      "NodeType",
      "pMax",
      "Value",
      "pValue",
      "ImposedVisibility",
      "",
      "pTerminal",
      "",
      "pAlias",
      "ValueDefault",
      "pValueDefault",
      "pInc",
      "DeviceName",
      "pIndex",
      "Timeout",
      "",
      "FeatureID",
      "", "",
      "ValueIndexed",
      "pChunkID",
      "pVariable",
      "VendorName",
      "Length",
      "CommandValue",
      "ValidValueSet",
      "pIsLocked",
      "pEnumEntry",
      "pError",
      "pInvalidator",
      "pValueIndexed",
      "pIsImplemented",
      "pPort",
      "DisplayName",
      "ChunkID",
      "pAddress",
      "pMin",
      "DisplayNotation",
      "PollingTime",
      "Formula",
      "IsLinear",
      "IsFeature",
      "pSelecting",
      "Description",
      "pIsAvailable",
      "Inc",
      "IsSelfClearing",
      "pValueCopy",
      "DisplayPrecision",
      "ToolTip",
      "pCommandValue",
      "Sign",
      "pCastAlias",
      "VersionGuid",
      "StandardNameSpace",
      "pBlockPolling",
      "NameSpace",
      "",
      "FormulaFrom",
      "DocuURL",
      "SchemaMajorVersion",
      "FormulaTo",
      "Slope",
      "p1212Parser",
      "IsDeprecated",
      "Min",
      "ModelName",
      "Visibility",
      "ProductGuid",
      "Address",
      "LSB",
      "Unit",
      "Streamable",
      "SchemaSubMinorVersion",
      "ImposedAccessMode",
      "Cachable",
      "Representation",
      "SubMinorVersion",
      "",
      "EventID",
      "Max",
      "", "", "", "",
      "SwapEndianess",
      "", "", "",
      "MajorVersion",
      "Symbolic",
      "", "", "", "",
      "SchemaMinorVersion",
      "", "", "", "", "",
      "CacheChunkData",
      "", "", "", "", "", "", "",
      "MinorVersion",
      "", "", "", "", "",
      "MSB",
      "", "", "", "", "",
      "Endianess",
      "", "", "", "", "", "", "", "", "",
      "", "", "", "", "", "", "", "", "",
      "", "", "", "", "", "", "", "", "",
      "", "", "", "", "", "", "", "", "",
      "", "", "", "",
      "AccessMode"
    };

  if (len <= MAX_WORD_LENGTH && len >= MIN_WORD_LENGTH)
    {
      register int key = hash (str, len);

      if (key <= MAX_HASH_VALUE && key >= 0)
        {
          register const char *s = wordlist[key];

          if (*str == *s && !strcmp (str + 1, s + 1))
            return s;
        }
    }
  return 0;
}
*/

#endif // __PERFECT_HASH_H__
