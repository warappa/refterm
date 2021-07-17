using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// From https://github.com/nesterovsky-bros/BidiVisualConverter/blob/master/BidiVisualConverter/usp10.cs
// Free to use https://github.com/nesterovsky-bros/BidiVisualConverter/issues/2#issuecomment-525154548: "We permit the use, copy and change this code without any restrictions."
namespace Refterm.Uniscribe
{
    using System;
    using System.Runtime.InteropServices;

    public class Defaults
    {
        public static uint LOCALE_USER_DEFAULT = 0x0400;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_CONTROL
    {

        /// uDefaultLanguage : 16
        ///fContextDigits : 1
        ///fInvertPreBoundDir : 1
        ///fInvertPostBoundDir : 1
        ///fLinkStringBefore : 1
        ///fLinkStringAfter : 1
        ///fNeutralOverride : 1
        ///fNumericOverride : 1
        ///fLegacyBidiClass : 1
        ///fMergeNeutralItems : 1
        ///fUseStandardBidi : 1
        ///fReserved : 6
        public uint bitvector1;

        public uint uDefaultLanguage
        {
            get
            {
                return ((uint)((this.bitvector1 & 65535u)));
            }
            set
            {
                this.bitvector1 = ((uint)((value | this.bitvector1)));
            }
        }

        public uint fContextDigits
        {
            get
            {
                return ((uint)(((this.bitvector1 & 65536u)
                            / 65536)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 65536)
                            | this.bitvector1)));
            }
        }

        public uint fInvertPreBoundDir
        {
            get
            {
                return ((uint)(((this.bitvector1 & 131072u)
                            / 131072)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 131072)
                            | this.bitvector1)));
            }
        }

        public uint fInvertPostBoundDir
        {
            get
            {
                return ((uint)(((this.bitvector1 & 262144u)
                            / 262144)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 262144)
                            | this.bitvector1)));
            }
        }

        public uint fLinkStringBefore
        {
            get
            {
                return ((uint)(((this.bitvector1 & 524288u)
                            / 524288)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 524288)
                            | this.bitvector1)));
            }
        }

        public uint fLinkStringAfter
        {
            get
            {
                return ((uint)(((this.bitvector1 & 1048576u)
                            / 1048576)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 1048576)
                            | this.bitvector1)));
            }
        }

        public uint fNeutralOverride
        {
            get
            {
                return ((uint)(((this.bitvector1 & 2097152u)
                            / 2097152)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 2097152)
                            | this.bitvector1)));
            }
        }

        public uint fNumericOverride
        {
            get
            {
                return ((uint)(((this.bitvector1 & 4194304u)
                            / 4194304)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 4194304)
                            | this.bitvector1)));
            }
        }

        public uint fLegacyBidiClass
        {
            get
            {
                return ((uint)(((this.bitvector1 & 8388608u)
                            / 8388608)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 8388608)
                            | this.bitvector1)));
            }
        }

        public uint fMergeNeutralItems
        {
            get
            {
                return ((uint)(((this.bitvector1 & 16777216u)
                            / 16777216)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 16777216)
                            | this.bitvector1)));
            }
        }

        public uint fUseStandardBidi
        {
            get
            {
                return ((uint)(((this.bitvector1 & 33554432u)
                            / 33554432)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 33554432)
                            | this.bitvector1)));
            }
        }

        public uint fReserved
        {
            get
            {
                return ((uint)(((this.bitvector1 & 4227858432u)
                            / 67108864)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 67108864)
                            | this.bitvector1)));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_STATE
    {

        /// uBidiLevel : 5
        ///fOverrideDirection : 1
        ///fInhibitSymSwap : 1
        ///fCharShape : 1
        ///fDigitSubstitute : 1
        ///fInhibitLigate : 1
        ///fDisplayZWG : 1
        ///fArabicNumContext : 1
        ///fGcpClusters : 1
        ///fReserved : 1
        ///fEngineReserved : 2
        public ushort bitvector1;

        public ushort uBidiLevel
        {
            get
            {
                return ((ushort)((this.bitvector1 & 31u)));
            }
            set
            {
                this.bitvector1 = ((ushort)((value | this.bitvector1)));
            }
        }

        public ushort fOverrideDirection
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 32u)
                            / 32)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 32)
                            | this.bitvector1)));
            }
        }

        public ushort fInhibitSymSwap
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 64u)
                            / 64)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 64)
                            | this.bitvector1)));
            }
        }

        public ushort fCharShape
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 128u)
                            / 128)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 128)
                            | this.bitvector1)));
            }
        }

        public ushort fDigitSubstitute
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 256u)
                            / 256)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 256)
                            | this.bitvector1)));
            }
        }

        public ushort fInhibitLigate
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 512u)
                            / 512)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 512)
                            | this.bitvector1)));
            }
        }

        public ushort fDisplayZWG
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 1024u)
                            / 1024)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 1024)
                            | this.bitvector1)));
            }
        }

        public ushort fArabicNumContext
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 2048u)
                            / 2048)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 2048)
                            | this.bitvector1)));
            }
        }

        public ushort fGcpClusters
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 4096u)
                            / 4096)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 4096)
                            | this.bitvector1)));
            }
        }

        public ushort fReserved
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 8192u)
                            / 8192)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 8192)
                            | this.bitvector1)));
            }
        }

        public ushort fEngineReserved
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 49152u)
                            / 16384)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 16384)
                            | this.bitvector1)));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_ANALYSIS
    {

        /// eScript : 10
        ///fRTL : 1
        ///fLayoutRTL : 1
        ///fLinkBefore : 1
        ///fLinkAfter : 1
        ///fLogicalOrder : 1
        ///fNoGlyphIndex : 1
        public ushort bitvector1;

        /// SCRIPT_STATE->SCRIPT_STATE
        public SCRIPT_STATE s;

        public ushort eScript
        {
            get
            {
                return ((ushort)((this.bitvector1 & 1023u)));
            }
            set
            {
                this.bitvector1 = ((ushort)((value | this.bitvector1)));
            }
        }

        public ushort fRTL
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 1024u)
                            / 1024)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 1024)
                            | this.bitvector1)));
            }
        }

        public ushort fLayoutRTL
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 2048u)
                            / 2048)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 2048)
                            | this.bitvector1)));
            }
        }

        public ushort fLinkBefore
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 4096u)
                            / 4096)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 4096)
                            | this.bitvector1)));
            }
        }

        public ushort fLinkAfter
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 8192u)
                            / 8192)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 8192)
                            | this.bitvector1)));
            }
        }

        public ushort fLogicalOrder
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 16384u)
                            / 16384)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 16384)
                            | this.bitvector1)));
            }
        }

        public ushort fNoGlyphIndex
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 32768u)
                            / 32768)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 32768)
                            | this.bitvector1)));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_ITEM
    {

        /// int
        public int iCharPos;

        /// SCRIPT_ANALYSIS->SCRIPT_ANALYSIS
        public SCRIPT_ANALYSIS a;
    }

    public enum SCRIPT_JUSTIFY
    {

        /// SCRIPT_JUSTIFY_NONE -> 0
        SCRIPT_JUSTIFY_NONE = 0,

        /// SCRIPT_JUSTIFY_ARABIC_BLANK -> 1
        SCRIPT_JUSTIFY_ARABIC_BLANK = 1,

        /// SCRIPT_JUSTIFY_CHARACTER -> 2
        SCRIPT_JUSTIFY_CHARACTER = 2,

        /// SCRIPT_JUSTIFY_RESERVED1 -> 3
        SCRIPT_JUSTIFY_RESERVED1 = 3,

        /// SCRIPT_JUSTIFY_BLANK -> 4
        SCRIPT_JUSTIFY_BLANK = 4,

        /// SCRIPT_JUSTIFY_RESERVED2 -> 5
        SCRIPT_JUSTIFY_RESERVED2 = 5,

        /// SCRIPT_JUSTIFY_RESERVED3 -> 6
        SCRIPT_JUSTIFY_RESERVED3 = 6,

        /// SCRIPT_JUSTIFY_ARABIC_NORMAL -> 7
        SCRIPT_JUSTIFY_ARABIC_NORMAL = 7,

        /// SCRIPT_JUSTIFY_ARABIC_KASHIDA -> 8
        SCRIPT_JUSTIFY_ARABIC_KASHIDA = 8,

        /// SCRIPT_JUSTIFY_ARABIC_ALEF -> 9
        SCRIPT_JUSTIFY_ARABIC_ALEF = 9,

        /// SCRIPT_JUSTIFY_ARABIC_HA -> 10
        SCRIPT_JUSTIFY_ARABIC_HA = 10,

        /// SCRIPT_JUSTIFY_ARABIC_RA -> 11
        SCRIPT_JUSTIFY_ARABIC_RA = 11,

        /// SCRIPT_JUSTIFY_ARABIC_BA -> 12
        SCRIPT_JUSTIFY_ARABIC_BA = 12,

        /// SCRIPT_JUSTIFY_ARABIC_BARA -> 13
        SCRIPT_JUSTIFY_ARABIC_BARA = 13,

        /// SCRIPT_JUSTIFY_ARABIC_SEEN -> 14
        SCRIPT_JUSTIFY_ARABIC_SEEN = 14,

        /// SCRIPT_JUSTIFY_ARABIC_SEEN_M -> 15
        SCRIPT_JUSTIFY_ARABIC_SEEN_M = 15,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_VISATTR
    {

        /// uJustification : 4
        ///fClusterStart : 1
        ///fDiacritic : 1
        ///fZeroWidth : 1
        ///fReserved : 1
        ///fShapeReserved : 8
        public ushort bitvector1;

        public ushort uJustification
        {
            get
            {
                return ((ushort)((this.bitvector1 & 15u)));
            }
            set
            {
                this.bitvector1 = ((ushort)((value | this.bitvector1)));
            }
        }

        public ushort fClusterStart
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 16u)
                            / 16)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 16)
                            | this.bitvector1)));
            }
        }

        public ushort fDiacritic
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 32u)
                            / 32)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 32)
                            | this.bitvector1)));
            }
        }

        public ushort fZeroWidth
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 64u)
                            / 64)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 64)
                            | this.bitvector1)));
            }
        }

        public ushort fReserved
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 128u)
                            / 128)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 128)
                            | this.bitvector1)));
            }
        }

        public ushort fShapeReserved
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 65280u)
                            / 256)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 256)
                            | this.bitvector1)));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tagGOFFSET
    {

        /// LONG->int
        public int du;

        /// LONG->int
        public int dv;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_LOGATTR
    {

        /// fSoftBreak : 1
        ///fWhiteSpace : 1
        ///fCharStop : 1
        ///fWordStop : 1
        ///fInvalid : 1
        ///fReserved : 3
        public byte bitvector1;

        public byte fSoftBreak
        {
            get
            {
                return ((byte)((this.bitvector1 & 1u)));
            }
            set
            {
                this.bitvector1 = ((byte)((value | this.bitvector1)));
            }
        }

        public byte fWhiteSpace
        {
            get
            {
                return ((byte)(((this.bitvector1 & 2u)
                            / 2)));
            }
            set
            {
                this.bitvector1 = ((byte)(((value * 2)
                            | this.bitvector1)));
            }
        }

        public byte fCharStop
        {
            get
            {
                return ((byte)(((this.bitvector1 & 4u)
                            / 4)));
            }
            set
            {
                this.bitvector1 = ((byte)(((value * 4)
                            | this.bitvector1)));
            }
        }

        public byte fWordStop
        {
            get
            {
                return ((byte)(((this.bitvector1 & 8u)
                            / 8)));
            }
            set
            {
                this.bitvector1 = ((byte)(((value * 8)
                            | this.bitvector1)));
            }
        }

        public byte fInvalid
        {
            get
            {
                return ((byte)(((this.bitvector1 & 16u)
                            / 16)));
            }
            set
            {
                this.bitvector1 = ((byte)(((value * 16)
                            | this.bitvector1)));
            }
        }

        public byte fReserved
        {
            get
            {
                return ((byte)(((this.bitvector1 & 224u)
                            / 32)));
            }
            set
            {
                this.bitvector1 = ((byte)(((value * 32)
                            | this.bitvector1)));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_PROPERTIES
    {

        /// langid : 16
        ///fNumeric : 1
        ///fComplex : 1
        ///fNeedsWordBreaking : 1
        ///fNeedsCaretInfo : 1
        ///bCharSet : 8
        ///fControl : 1
        ///fPrivateUseArea : 1
        ///fNeedsCharacterJustify : 1
        ///fInvalidGlyph : 1
        ///fInvalidLogAttr : 1
        ///fCDM : 1
        ///fAmbiguousCharSet : 1
        ///fClusterSizeVaries : 1
        ///fRejectInvalid : 1
        public uint bitvector1;

        public uint langid
        {
            get
            {
                return ((uint)((this.bitvector1 & 65535u)));
            }
            set
            {
                this.bitvector1 = ((uint)((value | this.bitvector1)));
            }
        }

        public uint fNumeric
        {
            get
            {
                return ((uint)(((this.bitvector1 & 65536u)
                            / 65536)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 65536)
                            | this.bitvector1)));
            }
        }

        public uint fComplex
        {
            get
            {
                return ((uint)(((this.bitvector1 & 131072u)
                            / 131072)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 131072)
                            | this.bitvector1)));
            }
        }

        public uint fNeedsWordBreaking
        {
            get
            {
                return ((uint)(((this.bitvector1 & 262144u)
                            / 262144)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 262144)
                            | this.bitvector1)));
            }
        }

        public uint fNeedsCaretInfo
        {
            get
            {
                return ((uint)(((this.bitvector1 & 524288u)
                            / 524288)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 524288)
                            | this.bitvector1)));
            }
        }

        public uint bCharSet
        {
            get
            {
                return ((uint)(((this.bitvector1 & 267386880u)
                            / 1048576)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 1048576)
                            | this.bitvector1)));
            }
        }

        public uint fControl
        {
            get
            {
                return ((uint)(((this.bitvector1 & 268435456u)
                            / 268435456)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 268435456)
                            | this.bitvector1)));
            }
        }

        public uint fPrivateUseArea
        {
            get
            {
                return ((uint)(((this.bitvector1 & 536870912u)
                            / 536870912)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 536870912)
                            | this.bitvector1)));
            }
        }

        public uint fNeedsCharacterJustify
        {
            get
            {
                return ((uint)(((this.bitvector1 & 1073741824u)
                            / 1073741824)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 1073741824)
                            | this.bitvector1)));
            }
        }

        public uint fInvalidGlyph
        {
            get
            {
                return ((uint)(((this.bitvector1 & 2147483648u)
                            / 2147483648)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 2147483648)
                            | this.bitvector1)));
            }
        }

        public uint fInvalidLogAttr
        {
            get
            {
                return ((uint)(((this.bitvector1 & 1u)
                            / 4294967296)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 4294967296)
                            | this.bitvector1)));
            }
        }

        public uint fCDM
        {
            get
            {
                return ((uint)(((this.bitvector1 & 2u)
                            / 8589934592)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 8589934592)
                            | this.bitvector1)));
            }
        }

        public uint fAmbiguousCharSet
        {
            get
            {
                return ((uint)(((this.bitvector1 & 4u)
                            / 17179869184)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 17179869184)
                            | this.bitvector1)));
            }
        }

        public uint fClusterSizeVaries
        {
            get
            {
                return ((uint)(((this.bitvector1 & 8u)
                            / 34359738368)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 34359738368)
                            | this.bitvector1)));
            }
        }

        public uint fRejectInvalid
        {
            get
            {
                return ((uint)(((this.bitvector1 & 16u)
                            / 68719476736)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 68719476736)
                            | this.bitvector1)));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_FONTPROPERTIES
    {

        /// int
        public int cBytes;

        /// WORD->unsigned short
        public ushort wgBlank;

        /// WORD->unsigned short
        public ushort wgDefault;

        /// WORD->unsigned short
        public ushort wgInvalid;

        /// WORD->unsigned short
        public ushort wgKashida;

        /// int
        public int iKashidaWidth;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_TABDEF
    {

        /// int
        public int cTabStops;

        /// int
        public int iScale;

        /// int*
        public IntPtr pTabStops;

        /// int
        public int iTabOrigin;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SCRIPT_DIGITSUBSTITUTE
    {

        /// NationalDigitLanguage : 16
        ///TraditionalDigitLanguage : 16
        ///DigitSubstitute : 8
        public uint bitvector1;

        /// DWORD->unsigned int
        public uint dwReserved;

        public uint NationalDigitLanguage
        {
            get
            {
                return ((uint)((this.bitvector1 & 65535u)));
            }
            set
            {
                this.bitvector1 = ((uint)((value | this.bitvector1)));
            }
        }

        public uint TraditionalDigitLanguage
        {
            get
            {
                return ((uint)(((this.bitvector1 & 4294901760u)
                            / 65536)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 65536)
                            | this.bitvector1)));
            }
        }

        public uint DigitSubstitute
        {
            get
            {
                return ((uint)(((this.bitvector1 & 255u)
                            / 4294967296)));
            }
            set
            {
                this.bitvector1 = ((uint)(((value * 4294967296)
                            | this.bitvector1)));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct opentype_feature_record
    {

        /// OPENTYPE_TAG->ULONG->unsigned int
        public uint tagFeature;

        /// LONG->int
        public int lParameter;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct textrange_properties
    {

        /// OPENTYPE_FEATURE_RECORD*
        public IntPtr potfRecords;

        /// int
        public int cotfRecords;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct script_charprop
    {

        /// fCanGlyphAlone : 1
        ///reserved : 15
        public ushort bitvector1;

        public ushort fCanGlyphAlone
        {
            get
            {
                return ((ushort)((this.bitvector1 & 1u)));
            }
            set
            {
                this.bitvector1 = ((ushort)((value | this.bitvector1)));
            }
        }

        public ushort reserved
        {
            get
            {
                return ((ushort)(((this.bitvector1 & 65534u)
                            / 2)));
            }
            set
            {
                this.bitvector1 = ((ushort)(((value * 2)
                            | this.bitvector1)));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct script_glyphprop
    {

        /// SCRIPT_VISATTR->SCRIPT_VISATTR
        public SCRIPT_VISATTR sva;

        /// WORD->unsigned short
        public ushort reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tagSIZE
    {

        /// LONG->int
        public int cx;

        /// LONG->int
        public int cy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ABC
    {

        /// int
        public int abcA;

        /// UINT->unsigned int
        public uint abcB;

        /// int
        public int abcC;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tagRECT
    {

        /// LONG->int
        public int left;

        /// LONG->int
        public int top;

        /// LONG->int
        public int right;

        /// LONG->int
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HDC__
    {

        /// int
        public int unused;
    }

    public partial class NativeMethods
    {

        /// Return Type: HRESULT->LONG->int
        ///psc: SCRIPT_CACHE*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptFreeCache", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptFreeCache(ref IntPtr psc);

        /// Return Type: HRESULT->LONG->int
        ///pwcInChars: WCHAR*
        ///cInChars: int
        ///cMaxItems: int
        ///psControl: SCRIPT_CONTROL*
        ///psState: SCRIPT_STATE*
        ///pItems: SCRIPT_ITEM*
        ///pcItems: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptItemize", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptItemize(
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcInChars,
          int cInChars,
          int cMaxItems,
          ref SCRIPT_CONTROL psControl,
          ref SCRIPT_STATE psState,
          [Out] SCRIPT_ITEM[] pItems,
          out int pcItems);

        /// Return Type: HRESULT->LONG->int
        ///cRuns: int
        ///pbLevel: BYTE*
        ///piVisualToLogical: int*
        ///piLogicalToVisual: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptLayout", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptLayout(
          int cRuns,
          [In] byte[] pbLevel,
          [Out] int[] piVisualToLogical,
          [Out] int[] piLogicalToVisual);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///pwcChars: WCHAR*
        ///cChars: int
        ///cMaxGlyphs: int
        ///psa: SCRIPT_ANALYSIS*
        ///pwOutGlyphs: WORD*
        ///pwLogClust: WORD*
        ///psva: SCRIPT_VISATTR*
        ///pcGlyphs: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptShape", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptShape(
          IntPtr hdc,
          ref IntPtr psc,
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcChars,
          int cChars,
          int cMaxGlyphs,
          ref SCRIPT_ANALYSIS psa,
          [Out] ushort[] pwOutGlyphs,
          [Out] ushort[] pwLogClust,
          [Out] SCRIPT_VISATTR[] psva,
          out int pcGlyphs);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///pwGlyphs: WORD*
        ///cGlyphs: int
        ///psva: SCRIPT_VISATTR*
        ///psa: SCRIPT_ANALYSIS*
        ///piAdvance: int*
        ///pGoffset: GOFFSET*
        ///pABC: ABC*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptPlace", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptPlace(
          IntPtr hdc,
          ref IntPtr psc,
          ushort[] pwGlyphs,
          int cGlyphs,
          SCRIPT_VISATTR[] psva,
          ref SCRIPT_ANALYSIS psa,
          [Out] int[] piAdvance,
          [Out] tagGOFFSET[] pGoffset,
          ref ABC pABC);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///x: int
        ///y: int
        ///fuOptions: UINT->unsigned int
        ///lprc: RECT*
        ///psa: SCRIPT_ANALYSIS*
        ///pwcReserved: WCHAR*
        ///iReserved: int
        ///pwGlyphs: WORD*
        ///cGlyphs: int
        ///piAdvance: int*
        ///piJustify: int*
        ///pGoffset: GOFFSET*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptTextOut", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptTextOut(
          IntPtr hdc,
          ref IntPtr psc,
          int x,
          int y,
          uint fuOptions,
          [In] tagRECT[] lprc,
          ref SCRIPT_ANALYSIS psa,
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcReserved,
          int iReserved,
          [In] ushort[] pwGlyphs,
          int cGlyphs,
          [In] int[] piAdvance,
          [In] int[] piJustify,
          ref tagGOFFSET pGoffset);

        /// Return Type: HRESULT->LONG->int
        ///psva: SCRIPT_VISATTR*
        ///piAdvance: int*
        ///cGlyphs: int
        ///iDx: int
        ///iMinKashida: int
        ///piJustify: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptJustify", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptJustify(
          [In] SCRIPT_VISATTR[] psva,
          [In] int[] piAdvance,
          int cGlyphs,
          int iDx,
          int iMinKashida,
          [Out] int[] piJustify);

        /// Return Type: HRESULT->LONG->int
        ///pwcChars: WCHAR*
        ///cChars: int
        ///psa: SCRIPT_ANALYSIS*
        ///psla: SCRIPT_LOGATTR*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptBreak", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptBreak(
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcChars,
          int cChars,
          ref SCRIPT_ANALYSIS psa,
          [Out] SCRIPT_LOGATTR[] psla);

        /// Return Type: HRESULT->LONG->int
        ///iCP: int
        ///fTrailing: BOOL->int
        ///cChars: int
        ///cGlyphs: int
        ///pwLogClust: WORD*
        ///psva: SCRIPT_VISATTR*
        ///piAdvance: int*
        ///psa: SCRIPT_ANALYSIS*
        ///piX: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptCPtoX", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptCPtoX(
          int iCP,
          [MarshalAs(UnmanagedType.Bool)] bool fTrailing,
          int cChars,
          int cGlyphs,
          [In] ushort[] pwLogClust,
          [In] SCRIPT_VISATTR[] psva,
          [In] int[] piAdvance,
          ref SCRIPT_ANALYSIS psa,
          [Out] int[] piX);

        /// Return Type: HRESULT->LONG->int
        ///iX: int
        ///cChars: int
        ///cGlyphs: int
        ///pwLogClust: WORD*
        ///psva: SCRIPT_VISATTR*
        ///piAdvance: int*
        ///psa: SCRIPT_ANALYSIS*
        ///piCP: int*
        ///piTrailing: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptXtoCP", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptXtoCP(
          int iX,
          int cChars,
          int cGlyphs,
          [In] ushort[] pwLogClust,
          [In] SCRIPT_VISATTR[] psva,
          [In] int[] piAdvance,
          ref SCRIPT_ANALYSIS psa,
          [Out] int[] piCP,
          [Out] int[] piTrailing);

        /// Return Type: HRESULT->LONG->int
        ///psa: SCRIPT_ANALYSIS*
        ///cChars: int
        ///cGlyphs: int
        ///piGlyphWidth: int*
        ///pwLogClust: WORD*
        ///psva: SCRIPT_VISATTR*
        ///piDx: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetLogicalWidths", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetLogicalWidths(
          ref SCRIPT_ANALYSIS psa,
          int cChars,
          int cGlyphs,
          [In] int[] piGlyphWidth,
          [In] ushort[] pwLogClust,
          ref SCRIPT_VISATTR psva,
          [Out] int[] piDx);

        /// Return Type: HRESULT->LONG->int
        ///piDx: int*
        ///cChars: int
        ///cGlyphs: int
        ///pwLogClust: WORD*
        ///psva: SCRIPT_VISATTR*
        ///piAdvance: int*
        ///psa: SCRIPT_ANALYSIS*
        ///pABC: ABC*
        ///piJustify: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptApplyLogicalWidth", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptApplyLogicalWidth(
          [In] int[] piDx,
          int cChars,
          int cGlyphs,
          [In] ushort[] pwLogClust,
          ref SCRIPT_VISATTR psva,
          [In] int[] piAdvance,
          ref SCRIPT_ANALYSIS psa,
          ref ABC pABC,
          [Out] int[] piJustify);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///pwcInChars: WCHAR*
        ///cChars: int
        ///dwFlags: DWORD->unsigned int
        ///pwOutGlyphs: WORD*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetCMap", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetCMap(
          IntPtr hdc,
          ref IntPtr psc,
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcInChars,
          int cChars,
          uint dwFlags,
          [Out] ushort[] pwOutGlyphs);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///wGlyph: WORD->unsigned short
        ///pABC: ABC*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetGlyphABCWidth", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetGlyphABCWidth(
          IntPtr hdc,
          ref IntPtr psc,
          ushort wGlyph,
          ref ABC pABC);

        /// Return Type: HRESULT->LONG->int
        ///ppSp: SCRIPT_PROPERTIES***
        ///piNumScripts: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetProperties", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetProperties(
          out IntPtr ppSp,
          out int piNumScripts);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///sfp: SCRIPT_FONTPROPERTIES*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetFontProperties", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetFontProperties(
          IntPtr hdc,
          ref IntPtr psc,
          out SCRIPT_FONTPROPERTIES sfp);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///tmHeight: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptCacheGetHeight", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptCacheGetHeight(
          IntPtr hdc,
          ref IntPtr psc,
          out int tmHeight);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///pString: void*
        ///cString: int
        ///cGlyphs: int
        ///iCharset: int
        ///dwFlags: DWORD->unsigned int
        ///iReqWidth: int
        ///psControl: SCRIPT_CONTROL*
        ///psState: SCRIPT_STATE*
        ///piDx: int*
        ///pTabdef: SCRIPT_TABDEF*
        ///pbInClass: BYTE*
        ///pssa: SCRIPT_STRING_ANALYSIS*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptStringAnalyse", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptStringAnalyse(
          IntPtr hdc,
          IntPtr pString,
          int cString,
          int cGlyphs,
          int iCharset,
          uint dwFlags,
          int iReqWidth,
          ref SCRIPT_CONTROL psControl,
          ref SCRIPT_STATE psState,
          [In] int[] piDx,
          ref SCRIPT_TABDEF pTabdef,
          ref byte pbInClass,
          ref IntPtr pssa);

        /// Return Type: HRESULT->LONG->int
        ///pssa: SCRIPT_STRING_ANALYSIS*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptStringFree", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptStringFree(ref IntPtr pssa);

        /// Return Type: SIZE*
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptString_pSize", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ScriptString_pSize(IntPtr ssa);

        /// Return Type: int*
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptString_pcOutChars", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ScriptString_pcOutChars(IntPtr ssa);

        /// Return Type: SCRIPT_LOGATTR*
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptString_pLogAttr", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr ScriptString_pLogAttr(IntPtr ssa);

        /// Return Type: HRESULT->LONG->int
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        ///puOrder: UINT*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptStringGetOrder", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptStringGetOrder(
          IntPtr ssa,
          out uint[] puOrder);

        /// Return Type: HRESULT->LONG->int
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        ///icp: int
        ///fTrailing: BOOL->int
        ///pX: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptStringCPtoX", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptStringCPtoX(
          IntPtr ssa,
          int icp,
          [MarshalAs(UnmanagedType.Bool)] bool fTrailing,
          ref int pX);

        /// Return Type: HRESULT->LONG->int
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        ///iX: int
        ///piCh: int*
        ///piTrailing: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptStringXtoCP", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptStringXtoCP(
          IntPtr ssa,
          int iX,
          out int piCh,
          out int piTrailing);

        /// Return Type: HRESULT->LONG->int
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        ///piDx: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptStringGetLogicalWidths", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptStringGetLogicalWidths(
          IntPtr ssa,
          [Out] int[] piDx);

        /// Return Type: HRESULT->LONG->int
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptStringValidate", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptStringValidate(IntPtr ssa);

        /// Return Type: HRESULT->LONG->int
        ///ssa: SCRIPT_STRING_ANALYSIS->void*
        ///iX: int
        ///iY: int
        ///uOptions: UINT->unsigned int
        ///prc: RECT*
        ///iMinSel: int
        ///iMaxSel: int
        ///fDisabled: BOOL->int
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptStringOut", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptStringOut(
          IntPtr ssa,
          int iX,
          int iY,
          uint uOptions,
          [In] tagRECT[] prc,
          int iMinSel,
          int iMaxSel,
          [MarshalAs(UnmanagedType.Bool)] bool fDisabled);

        /// Return Type: HRESULT->LONG->int
        ///pwcInChars: WCHAR*
        ///cInChars: int
        ///dwFlags: DWORD->unsigned int
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptIsComplex", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptIsComplex(
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcInChars,
          int cInChars,
          uint dwFlags);

        /// Return Type: HRESULT->LONG->int
        ///Locale: LCID->DWORD->unsigned int
        ///psds: SCRIPT_DIGITSUBSTITUTE*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptRecordDigitSubstitution", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptRecordDigitSubstitution(
          uint Locale,
          out SCRIPT_DIGITSUBSTITUTE psds);

        /// Return Type: HRESULT->LONG->int
        ///psds: SCRIPT_DIGITSUBSTITUTE*
        ///psc: SCRIPT_CONTROL*
        ///pss: SCRIPT_STATE*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptApplyDigitSubstitution", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptApplyDigitSubstitution(
          ref SCRIPT_DIGITSUBSTITUTE psds,
          out SCRIPT_CONTROL psc,
          out SCRIPT_STATE pss);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///psa: SCRIPT_ANALYSIS*
        ///tagScript: OPENTYPE_TAG->ULONG->unsigned int
        ///tagLangSys: OPENTYPE_TAG->ULONG->unsigned int
        ///rcRangeChars: int*
        ///rpRangeProperties: TEXTRANGE_PROPERTIES**
        ///cRanges: int
        ///pwcChars: WCHAR*
        ///cChars: int
        ///cMaxGlyphs: int
        ///pwLogClust: WORD*
        ///pCharProps: SCRIPT_CHARPROP*
        ///pwOutGlyphs: WORD*
        ///pOutGlyphProps: SCRIPT_GLYPHPROP*
        ///pcGlyphs: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptShapeOpenType", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptShapeOpenType(
          IntPtr hdc,
          ref IntPtr psc,
          ref SCRIPT_ANALYSIS psa,
          uint tagScript,
          uint tagLangSys,
          [In] int[] rcRangeChars,
          ref IntPtr rpRangeProperties,
          int cRanges,
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcChars,
          int cChars,
          int cMaxGlyphs,
          [Out] ushort[] pwLogClust,
          [Out] script_charprop[] pCharProps,
          [Out] ushort[] pwOutGlyphs,
          [Out] script_glyphprop[] pOutGlyphProps,
          out int pcGlyphs);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///psa: SCRIPT_ANALYSIS*
        ///tagScript: OPENTYPE_TAG->ULONG->unsigned int
        ///tagLangSys: OPENTYPE_TAG->ULONG->unsigned int
        ///rcRangeChars: int*
        ///rpRangeProperties: TEXTRANGE_PROPERTIES**
        ///cRanges: int
        ///pwcChars: WCHAR*
        ///pwLogClust: WORD*
        ///pCharProps: SCRIPT_CHARPROP*
        ///cChars: int
        ///pwGlyphs: WORD*
        ///pGlyphProps: SCRIPT_GLYPHPROP*
        ///cGlyphs: int
        ///piAdvance: int*
        ///pGoffset: GOFFSET*
        ///pABC: ABC*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptPlaceOpenType", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptPlaceOpenType(
          IntPtr hdc,
          ref IntPtr psc,
          ref SCRIPT_ANALYSIS psa,
          uint tagScript,
          uint tagLangSys,
          [In] int[] rcRangeChars,
          ref IntPtr rpRangeProperties,
          int cRanges,
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcChars,
          [In] ushort[] pwLogClust,
          [In] script_charprop[] pCharProps,
          int cChars,
          [In] ushort[] pwGlyphs,
          ref script_glyphprop pGlyphProps,
          int cGlyphs,
          [In] int[] piAdvance,
          [Out] tagGOFFSET[] pGoffset,
          out ABC pABC);

        /// Return Type: HRESULT->LONG->int
        ///pwcInChars: WCHAR*
        ///cInChars: int
        ///cMaxItems: int
        ///psControl: SCRIPT_CONTROL*
        ///psState: SCRIPT_STATE*
        ///pItems: SCRIPT_ITEM*
        ///pScriptTags: OPENTYPE_TAG*
        ///pcItems: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptItemizeOpenType", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptItemizeOpenType(
          [In][MarshalAs(UnmanagedType.LPWStr)] string pwcInChars,
          int cInChars,
          int cMaxItems,
          ref SCRIPT_CONTROL psControl,
          ref SCRIPT_STATE psState,
          [Out] SCRIPT_ITEM[] pItems,
          [Out] uint[] pScriptTags,
          ref int pcItems);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///psa: SCRIPT_ANALYSIS*
        ///cMaxTags: int
        ///pScriptTags: OPENTYPE_TAG*
        ///pcTags: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetFontScriptTags", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetFontScriptTags(
          IntPtr hdc,
          ref IntPtr psc,
          [In] SCRIPT_ANALYSIS[] psa,
          int cMaxTags,
          [Out] uint[] pScriptTags,
          out int pcTags);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///psa: SCRIPT_ANALYSIS*
        ///tagScript: OPENTYPE_TAG->ULONG->unsigned int
        ///cMaxTags: int
        ///pLangsysTags: OPENTYPE_TAG*
        ///pcTags: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetFontLanguageTags", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetFontLanguageTags(
          IntPtr hdc,
          ref IntPtr psc,
          [In] SCRIPT_ANALYSIS[] psa,
          uint tagScript,
          int cMaxTags,
          [Out] uint[] pLangsysTags,
          out int pcTags);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///psa: SCRIPT_ANALYSIS*
        ///tagScript: OPENTYPE_TAG->ULONG->unsigned int
        ///tagLangSys: OPENTYPE_TAG->ULONG->unsigned int
        ///cMaxTags: int
        ///pFeatureTags: OPENTYPE_TAG*
        ///pcTags: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetFontFeatureTags", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetFontFeatureTags(
          IntPtr hdc,
          ref IntPtr psc,
          [In] SCRIPT_ANALYSIS[] psa,
          uint tagScript,
          uint tagLangSys,
          int cMaxTags,
          [Out] uint[] pFeatureTags,
          out int pcTags);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///psa: SCRIPT_ANALYSIS*
        ///tagScript: OPENTYPE_TAG->ULONG->unsigned int
        ///tagLangSys: OPENTYPE_TAG->ULONG->unsigned int
        ///tagFeature: OPENTYPE_TAG->ULONG->unsigned int
        ///wGlyphId: WORD->unsigned short
        ///cMaxAlternates: int
        ///pAlternateGlyphs: WORD*
        ///pcAlternates: int*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptGetFontAlternateGlyphs", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptGetFontAlternateGlyphs(
          IntPtr hdc,
          ref IntPtr psc,
          [In] SCRIPT_ANALYSIS[] psa,
          uint tagScript,
          uint tagLangSys,
          uint tagFeature,
          ushort wGlyphId,
          int cMaxAlternates,
          [Out] ushort[] pAlternateGlyphs,
          out int pcAlternates);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///psa: SCRIPT_ANALYSIS*
        ///tagScript: OPENTYPE_TAG->ULONG->unsigned int
        ///tagLangSys: OPENTYPE_TAG->ULONG->unsigned int
        ///tagFeature: OPENTYPE_TAG->ULONG->unsigned int
        ///lParameter: LONG->int
        ///wGlyphId: WORD->unsigned short
        ///pwOutGlyphId: WORD*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptSubstituteSingleGlyph", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptSubstituteSingleGlyph(
          IntPtr hdc,
          ref IntPtr psc,
          [In] SCRIPT_ANALYSIS[] psa,
          uint tagScript,
          uint tagLangSys,
          uint tagFeature,
          int lParameter,
          ushort wGlyphId,
          out ushort pwOutGlyphId);

        /// Return Type: HRESULT->LONG->int
        ///hdc: HDC->HDC__*
        ///psc: SCRIPT_CACHE*
        ///psa: SCRIPT_ANALYSIS*
        ///tagScript: OPENTYPE_TAG->ULONG->unsigned int
        ///tagLangSys: OPENTYPE_TAG->ULONG->unsigned int
        ///tagFeature: OPENTYPE_TAG->ULONG->unsigned int
        ///lParameter: LONG->int
        ///wGlyphId: WORD->unsigned short
        ///iAdvance: int
        ///GOffset: GOFFSET->tagGOFFSET
        ///piOutAdvance: int*
        ///pOutGoffset: GOFFSET*
        [DllImportAttribute("usp10.dll", EntryPoint = "ScriptPositionSingleGlyph", CallingConvention = CallingConvention.StdCall)]
        public static extern int ScriptPositionSingleGlyph(
          IntPtr hdc,
          ref IntPtr psc,
          [In] SCRIPT_ANALYSIS[] psa,
          uint tagScript,
          uint tagLangSys,
          uint tagFeature,
          int lParameter,
          ushort wGlyphId,
          int iAdvance,
          tagGOFFSET GOffset,
          out int piOutAdvance,
          out tagGOFFSET pOutGoffset);
    }

    public static class Constants
    {
        public const int SCRIPT_UNDEFINED = 0;
        //
        //p     SCRIPT_UNDEFINED: This is the only public script ordinal. May be
        //      forced into the eScript field of a SCRIPT_ANALYSIS to disable shaping.
        //      SCRIPT_UNDEFINED is supported by all fonts - ScriptShape will display
        //      whatever glyph is defined in the font CMAP table, or, if none, the
        //      missing glyph.

        public const int SGCM_RTL = 0x00000001;     // Return mirrored glyph for mirrorable Unicode codepoints


        public const int SSA_PASSWORD = 0x00000001; // Input string contains a single character to be duplicated iLength times
        public const int SSA_TAB = 0x00000002; // Expand tabs
        public const int SSA_CLIP = 0x00000004; // Clip string at iReqWidth
        public const int SSA_FIT = 0x00000008; // Justify string to iReqWidth
        public const int SSA_DZWG = 0x00000010; // Provide representation glyphs for control characters
        public const int SSA_FALLBACK = 0x00000020; // Use fallback fonts
        public const int SSA_BREAK = 0x00000040; // Return break flags (character and word stops)
        public const int SSA_GLYPHS = 0x00000080; // Generate glyphs, positions and attributes
        public const int SSA_RTL = 0x00000100; // Base embedding level 1
        public const int SSA_GCP = 0x00000200; // Return missing glyphs and LogCLust with GetCharacterPlacement conventions
        public const int SSA_HOTKEY = 0x00000400; // Replace '&' with underline on subsequent codepoint
        public const int SSA_METAFILE = 0x00000800; // Write items with ExtTextOutW Unicode calls, not glyphs
        public const int SSA_LINK = 0x00001000; // Apply FE font linking/association to non-complex text
        public const int SSA_HIDEHOTKEY = 0x00002000; // Remove first '&' from displayed string
        public const int SSA_HOTKEYONLY = 0x00002400; // Display underline only.

        public const int SSA_FULLMEASURE = 0x04000000; // Internal - calculate full width and out the number of chars can fit in iReqWidth.
        public const int SSA_LPKANSIFALLBACK = 0x08000000; // Internal - enable FallBack for all LPK Ansi calls Except BiDi hDC calls
        public const int SSA_PIDX = 0x10000000; // Internal
        public const int SSA_LAYOUTRTL = 0x20000000; // Internal - Used when DC is mirrored
        public const int SSA_DONTGLYPH = 0x40000000; // Internal - Used only by GDI during metafiling - Use ExtTextOutA for positioning
        public const int SSA_NOKASHIDA = unchecked((int)0x80000000); // Internal - Used by GCP to justify the non Arabic glyphs only.

        public const int SIC_COMPLEX = 1;  // Treat complex script letters as complex
        public const int SIC_ASCIIDIGIT = 2;  // Treat digits U+0030 through U+0039 as complex
        public const int SIC_NEUTRAL = 4;  // Treat neutrals as complex

        public const int SCRIPT_DIGITSUBSTITUTE_CONTEXT = 0; // Substitute to match preceeding letters
        public const int SCRIPT_DIGITSUBSTITUTE_NONE = 1; // No substitution
        public const int SCRIPT_DIGITSUBSTITUTE_NATIONAL = 2; // Substitute with official national digits
        public const int SCRIPT_DIGITSUBSTITUTE_TRADITIONAL = 3; // Substitute with traditional digits of the locale

        // Undefined script tag.
        public const int SCRIPT_TAG_UNKNOWN = 0x00000000;

        public const int S_OK = 0;
    }
}