using System;
using System.Collections.Generic;

namespace Fizz.Common
{
    public sealed class FizzLanguageCodes
    {
        public static readonly IFizzLanguageCode Afrikaans = new FizzLanguageCode("Afrikaans", "af");
        public static readonly IFizzLanguageCode Arabic = new FizzLanguageCode("Arabic", "ar");
        public static readonly IFizzLanguageCode Bangla = new FizzLanguageCode("Bangla", "bn");
        public static readonly IFizzLanguageCode BosnianLatin = new FizzLanguageCode("BosnianLatin", "bs");
        public static readonly IFizzLanguageCode Bulgarian = new FizzLanguageCode("Bulgarian", "bg");
        public static readonly IFizzLanguageCode CantoneseTraditional = new FizzLanguageCode("CantoneseTraditional", "yue");
        public static readonly IFizzLanguageCode Catalan = new FizzLanguageCode("Catalan", "ca");
        public static readonly IFizzLanguageCode ChineseSimplified = new FizzLanguageCode("ChineseSimplified", "zh-Hans");
        public static readonly IFizzLanguageCode ChineseTraditional = new FizzLanguageCode("ChineseTraditional", "zh-Hant");
        public static readonly IFizzLanguageCode Croatian = new FizzLanguageCode("Croatian", "hr");
        public static readonly IFizzLanguageCode Czech = new FizzLanguageCode("Czech", "cs");
        public static readonly IFizzLanguageCode Danish = new FizzLanguageCode("Danish", "da");
        public static readonly IFizzLanguageCode Dutch = new FizzLanguageCode("Dutch", "nl");
        public static readonly IFizzLanguageCode English = new FizzLanguageCode("English", "en");
        public static readonly IFizzLanguageCode Estonian = new FizzLanguageCode("Estonian", "et");
        public static readonly IFizzLanguageCode Fijian = new FizzLanguageCode("Fijian", "fj");
        public static readonly IFizzLanguageCode Filipino = new FizzLanguageCode("Filipino", "fil");
        public static readonly IFizzLanguageCode Finnish = new FizzLanguageCode("Finnish", "fi");
        public static readonly IFizzLanguageCode French = new FizzLanguageCode("French", "fr");
        public static readonly IFizzLanguageCode German = new FizzLanguageCode("German", "de");
        public static readonly IFizzLanguageCode Greek = new FizzLanguageCode("Greek", "el");
        public static readonly IFizzLanguageCode HaitianCreole = new FizzLanguageCode("HaitianCreole", "ht");
        public static readonly IFizzLanguageCode Hebrew = new FizzLanguageCode("Hebrew", "he");
        public static readonly IFizzLanguageCode Hindi = new FizzLanguageCode("Hindi", "hi");
        public static readonly IFizzLanguageCode HmongDaw = new FizzLanguageCode("HmongDaw", "mww");
        public static readonly IFizzLanguageCode Hungarian = new FizzLanguageCode("Hungarian", "hu");
        public static readonly IFizzLanguageCode Icelandic = new FizzLanguageCode("Icelandic", "is");
        public static readonly IFizzLanguageCode Indonesian = new FizzLanguageCode("Indonesian", "id");
        public static readonly IFizzLanguageCode Italian = new FizzLanguageCode("Italian", "it");
        public static readonly IFizzLanguageCode Japanese = new FizzLanguageCode("Japanese", "ja");
        public static readonly IFizzLanguageCode Kiswahili = new FizzLanguageCode("Kiswahili", "sw");
        public static readonly IFizzLanguageCode Klingon = new FizzLanguageCode("Klingon", "tlh");
        public static readonly IFizzLanguageCode KlingonPlqaD = new FizzLanguageCode("KlingonPlqaD", "tlh-Qaak");
        public static readonly IFizzLanguageCode Korean = new FizzLanguageCode("Korean", "ko");
        public static readonly IFizzLanguageCode Latvian = new FizzLanguageCode("Latvian", "lv");
        public static readonly IFizzLanguageCode Lithuanian = new FizzLanguageCode("Lithuanian", "lt");
        public static readonly IFizzLanguageCode Malagasy = new FizzLanguageCode("Malagasy", "mg");
        public static readonly IFizzLanguageCode Malay = new FizzLanguageCode("Malay", "ms");
        public static readonly IFizzLanguageCode Maltese = new FizzLanguageCode("Maltese", "mt");
        public static readonly IFizzLanguageCode Norwegian = new FizzLanguageCode("Norwegian", "nb");
        public static readonly IFizzLanguageCode Persian = new FizzLanguageCode("Persian", "fa");
        public static readonly IFizzLanguageCode Polish = new FizzLanguageCode("Polish", "pl");
        public static readonly IFizzLanguageCode Portuguese = new FizzLanguageCode("Portuguese", "pt");
        public static readonly IFizzLanguageCode QueretaroOtomi = new FizzLanguageCode("QueretaroOtomi", "otq");
        public static readonly IFizzLanguageCode Romanian = new FizzLanguageCode("Romanian", "ro");
        public static readonly IFizzLanguageCode Russian = new FizzLanguageCode("Russian", "ru");
        public static readonly IFizzLanguageCode Samoan = new FizzLanguageCode("Samoan", "sm");
        public static readonly IFizzLanguageCode SerbianCyrillic = new FizzLanguageCode("SerbianCyrillic", "sr-Cyrl");
        public static readonly IFizzLanguageCode SerbianLatin = new FizzLanguageCode("SerbianLatin", "sr-Latn");
        public static readonly IFizzLanguageCode Slovak = new FizzLanguageCode("Slovak", "sk");
        public static readonly IFizzLanguageCode Slovenian = new FizzLanguageCode("Slovenian", "sl");
        public static readonly IFizzLanguageCode Spanish = new FizzLanguageCode("Spanish", "es");
        public static readonly IFizzLanguageCode Swedish = new FizzLanguageCode("Swedish", "sv");
        public static readonly IFizzLanguageCode Tahitian = new FizzLanguageCode("Tahitian", "ty");
        public static readonly IFizzLanguageCode Tamil = new FizzLanguageCode("Tamil", "ta");
        public static readonly IFizzLanguageCode Telugu = new FizzLanguageCode("Telugu", "te");
        public static readonly IFizzLanguageCode Thai = new FizzLanguageCode("Thai", "th");
        public static readonly IFizzLanguageCode Tongan = new FizzLanguageCode("Tongan", "to");
        public static readonly IFizzLanguageCode Turkish = new FizzLanguageCode("Turkish", "tr");
        public static readonly IFizzLanguageCode Ukrainian = new FizzLanguageCode("Ukrainian", "uk");
        public static readonly IFizzLanguageCode Urdu = new FizzLanguageCode("Urdu", "ur");
        public static readonly IFizzLanguageCode Vietnamese = new FizzLanguageCode("Vietnamese", "vi");
        public static readonly IFizzLanguageCode Welsh = new FizzLanguageCode("Welsh", "cy");
        public static readonly IFizzLanguageCode YucatecMaya = new FizzLanguageCode("YucatecMaya", "yua");

        public static IList<IFizzLanguageCode> AllLanguages { get { return _languageCodes; } } 

        private static IList<IFizzLanguageCode> _languageCodes = new List<IFizzLanguageCode>()
        {
            Afrikaans,
            Arabic,
            Bangla,
            BosnianLatin,
            Bulgarian,
            CantoneseTraditional,
            Catalan,
            ChineseSimplified,
            ChineseTraditional,
            Croatian,
            Czech,
            Danish,
            Dutch,
            English,
            Estonian,
            Fijian,
            Filipino,
            Finnish,
            French,
            German,
            Greek,
            HaitianCreole,
            Hebrew,
            Hindi,
            HmongDaw,
            Hungarian,
            Icelandic,
            Indonesian,
            Italian,
            Japanese,
            Kiswahili,
            Klingon,
            KlingonPlqaD,
            Korean,
            Latvian,
            Lithuanian,
            Malagasy,
            Malay,
            Maltese,
            Norwegian,
            Persian,
            Polish,
            Portuguese,
            QueretaroOtomi,
            Romanian,
            Russian,
            Samoan,
            SerbianCyrillic,
            SerbianLatin,
            Slovak,
            Slovenian,
            Spanish,
            Swedish,
            Tahitian,
            Tamil,
            Telugu,
            Thai,
            Tongan,
            Turkish,
            Ukrainian,
            Urdu,
            Vietnamese,
            Welsh,
            YucatecMaya
        };

        private FizzLanguageCodes() { }
        
        class FizzLanguageCode : IFizzLanguageCode
        {
            public string Language { get { return _language; } }
            public string Code { get { return _code; } }

            private string _language;
            private string _code;

            internal FizzLanguageCode(string lang, string code)
            {
                _language = lang;
                _code = code;
            }
        }
    }

    public interface IFizzLanguageCode
    {
        string Language { get; }
        string Code { get; }
    }
}
