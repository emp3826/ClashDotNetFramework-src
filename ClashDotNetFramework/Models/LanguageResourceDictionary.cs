using ClashDotNetFramework.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClashDotNetFramework.Models
{
    public class LanguageResourceDictionary : ResourceDictionary
    {
        private Uri _englishSource;
        private Uri _chineseSource;
        private Uri _japaneseSource;

        public Uri EnglishSource
        {
            get { return _englishSource; }
            set
            {
                _englishSource = value;
                UpdateSource();
            }
        }

        public Uri ChineseSource
        {
            get { return _chineseSource; }
            set
            {
                _chineseSource = value;
                UpdateSource();
            }
        }

        public Uri JapaneseSource
        {
            get { return _japaneseSource; }
            set
            {
                _japaneseSource = value;
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            Uri source = null;
            switch (Global.Settings.Language)
            {
                case LanguageType.English:
                    source = EnglishSource;
                    break;
                case LanguageType.Chinese:
                    source = ChineseSource;
                    break;
                case LanguageType.Japanese:
                    source = JapaneseSource;
                    break;
                default:
                    break;
            }
            if (source != null && base.Source != source)
                base.Source = source;
        }
    }
}
