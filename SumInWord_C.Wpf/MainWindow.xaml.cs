using Microsoft.Extensions.DependencyInjection;
using SumInWord_C.Wpf.Properties;
using SumInWord_C.Wpf.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace SumInWord_C.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                var settings = Properties.Settings.Default;

                // Перевірка: чи були збережені дійсні розміри?
                // Використовуємо MinWidth як поріг
                if (settings.WindowWidth >= this.MinWidth && settings.WindowHeight >= this.MinHeight)
                {
                    this.Width = settings.WindowWidth;
                    this.Height = settings.WindowHeight;
                    this.Left = settings.WindowLeft;
                    this.Top = settings.WindowTop;
                    if (settings.WindowState != WindowState.Minimized) this.WindowState = settings.WindowState;
                }

                InitializeComponent();

                // 2. Перевірка видимості
                if (!IsWindowVisibleOnAnyScreen(settings))
                {
                    // Якщо поза екраном, скидаємо позицію
                    this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            catch (Exception ex)
            {
                // Якщо сталася помилка (наприклад, пошкоджений файл налаштувань),
                // ініціалізуємо компоненти та дозволяємо WPF використовувати XAML-налаштування.
                InitializeComponent();
                MessageBox.Show($"Помилка завантаження налаштувань вікна. Використовуються стандартні параметри. Помилка: {ex.Message}", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                DataContext = App.ServiceProvider.GetRequiredService<SumViewModel>();
            }
        }

        private static bool IsWindowVisibleOnAnyScreen(Settings settings)
        {
            // 1. Створюємо Rect, що описує збережену позицію вікна
            var windowRect = new Rect(
                settings.WindowLeft,
                settings.WindowTop,
                settings.WindowWidth,
                settings.WindowHeight);

            // 2. Створюємо Rect, що описує віртуальний екран (охоплює всі монітори)
            var virtualScreen = new Rect(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight);

            // 3. Перевіряємо, чи перетинаються прямокутники (чи видима хоча б частина вікна)
            // Якщо прямокутники перетинаються, повертаємо true.
            return windowRect.IntersectsWith(virtualScreen);
        }

        // Збереження стану вікна при закритті
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var settings = Properties.Settings.Default;

                // Зберігаємо стан вікна (Maximized, Normal, etc.)
                settings.WindowState = this.WindowState;

                // Зберігаємо розмір та позицію, лише якщо вікно не згорнуте або розгорнуте
                if (this.WindowState == WindowState.Normal)
                {
                    settings.WindowLeft = this.Left;
                    settings.WindowTop = this.Top;
                    settings.WindowWidth = this.Width;
                    settings.WindowHeight = this.Height;
                }

                // Зберігаємо зміни
                settings.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження налаштувань вікна: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                MaximizeButton.Content = "⬜";
            }
            else
            {
                WindowState = WindowState.Maximized;
                MaximizeButton.Content = "❐";
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

    }
}