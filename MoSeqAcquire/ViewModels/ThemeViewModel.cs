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
        protected Swatch primarySwatch;
        protected Swatch accentSwatch;

        public ThemeViewModel()
        {
            this.helper = new PaletteHelper();

            this.SetThemeCommand = new ActionCommand((p) => this.LightDarkMode = (LightDarkMode)p);
            this.ApplyPrimaryCommand = new ActionCommand((o) => this.PrimarySwatch = (Swatch)o);
            this.ApplyAccentCommand = new ActionCommand((o) => this.AccentSwatch = (Swatch)o);

            this.PrimarySwatches = new SwatchesProvider().Swatches.Select(s => new SwatchViewModel(s, this.ApplyPrimaryCommand));
            this.AccentSwatches = new SwatchesProvider().Swatches.Select(s => new SwatchViewModel(s, this.ApplyAccentCommand));
        }
        
        public LightDarkMode LightDarkMode
        {
            get => this.lightDarkMode;
            set {
                this.helper.SetLightDark(value == LightDarkMode.Dark ? true : false);
                this.SetField(ref this.lightDarkMode, value);
            }
        }
        public Swatch PrimarySwatch
        {
            get => this.primarySwatch;
            set
            {
                this.helper.ReplacePrimaryColor(value);
                this.PrimarySwatches.ForEach(s => s.IsSelected = false);
                this.PrimarySwatches.Where(s => s.Swatch.Equals(value)).ForEach(s => s.IsSelected = true);
                this.SetField(ref this.primarySwatch, value);
            }
        }
        public Swatch AccentSwatch
        {
            get => this.accentSwatch;
            set
            {
                this.helper.ReplaceAccentColor(value);
                this.AccentSwatches.ForEach(s => s.IsSelected = false);
                this.AccentSwatches.Where(s => s.Swatch.Equals(value)).ForEach(s => s.IsSelected = true);
                this.SetField(ref this.accentSwatch, value);
            }
        }

        public IEnumerable<SwatchViewModel> PrimarySwatches { get; }
        public IEnumerable<SwatchViewModel> AccentSwatches { get; }
        public ICommand SetThemeCommand { get; protected set; }
        public ICommand ApplyPrimaryCommand { get; protected set; }
        public ICommand ApplyAccentCommand { get; protected set; }
    }

    public class SwatchViewModel : BaseViewModel
    {
        protected Swatch swatch;
        protected bool isSelected;
        public SwatchViewModel(Swatch swatch)
        {
            this.swatch = swatch;
        }
        public SwatchViewModel(Swatch swatch, ICommand selectionCommand)
        {
            this.swatch = swatch;
            this.SelectionCommand = selectionCommand;
        }
        public Swatch Swatch { get => this.swatch; }
        public string Name { get => this.swatch.Name; }
        public bool IsSelected
        {
            get => this.isSelected;
            set => this.SetField(ref this.isSelected, value);
        }
        public ICommand SelectionCommand { get; set; }
    }
}
