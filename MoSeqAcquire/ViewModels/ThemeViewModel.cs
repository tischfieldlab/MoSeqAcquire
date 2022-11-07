using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MoSeqAcquire.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MoSeqAcquire.ViewModels
{
    public class ThemeViewModel : BaseViewModel
    {
        protected PaletteHelper helper;
        protected bool isDarkMode;
        protected Swatch primarySwatch;
        protected Swatch accentSwatch;

        public ThemeViewModel()
        {
            this.Initialize();
            this.IsDarkMode = Properties.Settings.Default.ThemeIsDarkMode;
            this.PrimarySwatch = this.AvailableSwatches.First(s => s.Name == Properties.Settings.Default.ThemePrimaryColor);
            this.AccentSwatch = this.AvailableSwatches.First(s => s.Name == Properties.Settings.Default.ThemeAccentColor);
            this.PropertyChanged += (s, e) => this.PushThemeToSettings();
        }
        public ThemeViewModel(bool isDarkMode, string Primary, string Accent)
        {
            this.Initialize();
            this.IsDarkMode = isDarkMode;
            this.PrimarySwatch = this.AvailableSwatches.First(s => s.Name == Primary);
            this.AccentSwatch = this.AvailableSwatches.First(s => s.Name == Accent);
            this.PropertyChanged += (s, e) => this.PushThemeToSettings();
        }
        protected void Initialize()
        {
            this.helper = new PaletteHelper();
            this.AvailableSwatches = new SwatchesProvider().Swatches;            
        }
        protected void PushThemeToSettings()
        {
            Properties.Settings.Default.ThemeIsDarkMode = this.IsDarkMode;
            Properties.Settings.Default.ThemePrimaryColor = this.PrimarySwatch.Name;
            Properties.Settings.Default.ThemeAccentColor = this.AccentSwatch.Name;
            Properties.Settings.Default.Save();
        }
        
        public bool IsDarkMode
        {
            get => this.isDarkMode;
            set
            {
                ITheme theme = this.helper.GetTheme();
                theme.SetBaseTheme(value ? Theme.Dark : Theme.Light);
                this.helper.SetTheme(theme);
                this.SetField(ref this.isDarkMode, value);
            }
        }
        public Swatch PrimarySwatch
        {
            get => this.primarySwatch;
            set
            {
                ITheme theme = this.helper.GetTheme();
                theme.SetPrimaryColor(value.PrimaryHues.First().Color);
                this.helper.SetTheme(theme);
                this.SetField(ref this.primarySwatch, value);
            }
        }
        public Swatch AccentSwatch
        {
            get => this.accentSwatch;
            set
            {
                ITheme theme = this.helper.GetTheme();
                theme.SetPrimaryColor(value.AccentHues.First().Color);
                this.helper.SetTheme(theme);
                this.SetField(ref this.accentSwatch, value);
            }
        }

        public IEnumerable<Swatch> AvailableSwatches { get; protected set; }
    }
}
