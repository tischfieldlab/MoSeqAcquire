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
    public enum LightDarkMode
    {
        Light,
        Dark
    }
    public class ThemeViewModel : BaseViewModel
    {
        protected PaletteHelper helper;
        protected LightDarkMode lightDarkMode;

        public ThemeViewModel()
        {
            Swatches = new SwatchesProvider().Swatches;
            this.helper = new PaletteHelper();
        }
        public ICommand SetThemeCommand => new ActionCommand((p) => this.LightDarkMode = (LightDarkMode)p);
        public LightDarkMode LightDarkMode
        {
            get => this.lightDarkMode;
            set {
                this.helper.SetLightDark(value == LightDarkMode.Dark ? true : false);
                this.SetField(ref this.lightDarkMode, value);
            }
        }

        public IEnumerable<Swatch> Swatches { get; }
        public ICommand ApplyPrimaryCommand { get; } = new ActionCommand(o => ApplyPrimary((Swatch)o));

        private static void ApplyPrimary(Swatch swatch)
        {
            new PaletteHelper().ReplacePrimaryColor(swatch);
        }

        public ICommand ApplyAccentCommand { get; } = new ActionCommand(o => ApplyAccent((Swatch)o));

        private static void ApplyAccent(Swatch swatch)
        {
            new PaletteHelper().ReplaceAccentColor(swatch);
        }
    }
}
