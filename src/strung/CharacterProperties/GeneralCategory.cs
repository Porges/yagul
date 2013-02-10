using System;

namespace Strung.CharacterProperties
{
    [Flags]
    public enum GeneralCategory
    {
        UppercaseLetter = 1 << 0,
        LowercaseLetter = 1 << 1,
        TitlecaseLetter = 1 << 2,
        ModifierLetter = 1 << 3,
        OtherLetter = 1 << 4,
        Letter = UppercaseLetter | LowercaseLetter | TitlecaseLetter | ModifierLetter | OtherLetter,

        NonspacingMark = 1 << 5,
        SpacingCombiningMark = 1 << 6,
        EnclosingMark = 1 << 7,
        Mark = NonspacingMark | SpacingCombiningMark | EnclosingMark,

        DecimalDigitNumber = 1 << 8,
        LetterNumber = 1 << 9,
        OtherNumber = 1 << 10,
        Number = DecimalDigitNumber | LetterNumber | OtherNumber,

        ConnectorPunctuation = 1 << 11,
        DashPunctuation = 1 << 12,
        OpenPunctuation = 1 << 13,
        ClosePunctuation = 1 << 14,
        InitialQuotePunctuation = 1 << 15,
        FinalQuotePunctuation = 1 << 16,
        OtherPunctuation = 1 << 17,
        Punctuation = ConnectorPunctuation | DashPunctuation | OpenPunctuation | ClosePunctuation | InitialQuotePunctuation | FinalQuotePunctuation | OtherPunctuation,

        MathSymbol = 1 << 18,
        CurrencySymbol = 1 << 20,
        ModifierSymbol = 1 << 21,
        OtherSymbol = 1 << 22,
        Symbol = MathSymbol | CurrencySymbol | ModifierSymbol | OtherSymbol,

        SpaceSeparator = 1 << 23,
        LineSeparator = 1 << 24,
        ParagraphSeparator = 1 << 25,
        Separator = SpaceSeparator | LineSeparator | ParagraphSeparator,

        Control = 1 << 26,
        Format = 1 << 27,
        Surrogate = 1 << 28,
        PrivateUse = 1 << 29,
        Unassigned = 1 << 30,
        Other = Control | Format | Surrogate | PrivateUse | Unassigned,
    }

    internal enum CompactGeneralCategory
    {
        UppercaseLetter,
        LowercaseLetter,
        TitlecaseLetter,
        ModifierLetter,
        OtherLetter,
        NonspacingMark,
        SpacingCombiningMark,
        EnclosingMark,
        DecimalDigitNumber,
        LetterNumber,
        OtherNumber,
        ConnectorPunctuation,
        DashPunctuation,
        OpenPunctuation,
        ClosePunctuation,
        InitialQuotePunctuation,
        FinalQuotePunctuation,
        OtherPunctuation,
        MathSymbol,
        CurrencySymbol,
        ModifierSymbol,
        OtherSymbol,
        SpaceSeparator,
        LineSeparator,
        ParagraphSeparator,
        Control,
        Format,
        Surrogate,
        PrivateUse,
        Unassigned,
    }
}
