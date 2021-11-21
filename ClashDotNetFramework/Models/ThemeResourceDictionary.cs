using ClashDotNetFramework.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClashDotNetFramework.Models
{
    public class ThemeResourceDictionary : ResourceDictionary
    {
        private Uri _classicSource;
        private Uri _modernSource;
        private Uri _darkSource;

        public Uri ClassicSource
        {
            get { return _classicSource; }
            set
            {
                _classicSource = value;
                UpdateSource();
            }
        }

        public Uri ModernSource
        {
            get { return _modernSource; }
            set
            {
                _modernSource = value;
                UpdateSource();
            }
        }

        public Uri DarkSource
        {
            get { return _darkSource; }
            set
            {
                _darkSource = value;
                UpdateSource();
            }
        }

        public void UpdateSource()
        {
            Uri source = null;
            switch (Global.Settings.Theme)
            {
                case ThemeType.Classic:
                    source = ClassicSource;
                    break;
                case ThemeType.Modern:
                    source = ModernSource;
                    break;
                case ThemeType.Dark:
                    source = DarkSource;
                    break;
                default:
                    break;
            }
            if (source != null && base.Source != source)
                base.Source = source;
        }
    }
}
