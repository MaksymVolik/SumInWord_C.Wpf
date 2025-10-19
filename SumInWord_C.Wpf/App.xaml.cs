using Microsoft.Extensions.DependencyInjection;
using SumInWord_C.Wpf.Interfaces;
using SumInWord_C.Wpf.Services;
using SumInWord_C.Wpf.ViewModels;
using System.Windows;

namespace SumInWord_C.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // Реєстрація сервісів
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddSingleton<INumberParserService, NumberParserService>();
            services.AddSingleton<IAmountToWordsService, AmountToWordsService>();
            services.AddSingleton<IThemeService, ThemeService>(); // Додали ThemeService
            services.AddTransient<SumViewModel>();

            ServiceProvider = services.BuildServiceProvider();

            // Завантажуємо збережену тему перед показом головного вікна
            var themeService = ServiceProvider.GetRequiredService<IThemeService>();
            themeService.LoadSavedTheme();

            base.OnStartup(e);
        }
    }
}
