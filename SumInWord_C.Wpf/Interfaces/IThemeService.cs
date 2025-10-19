namespace SumInWord_C.Wpf.Interfaces
{
    public interface IThemeService
    {
        void ApplyTheme(Services.Theme theme);
        Services.Theme GetCurrentTheme();
        void LoadSavedTheme();
    }
}