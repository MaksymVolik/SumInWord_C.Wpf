using SumInWord_C.Wpf.Interfaces;
using System.Windows;
using System.Runtime.InteropServices;

namespace SumInWord_C.Wpf.Services
{
    public class ClipboardService : IClipboardService
    {
        public void SetText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            const int maxRetries = 3;
            const int delayMilliseconds = 100;

            Exception? lastException = null;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    Exception? threadException = null;
                    
                    var thread = new Thread(() =>
                    {
                        try
                        {
                            Clipboard.SetDataObject(text, true);
                        }
                        catch (Exception ex)
                        {
                            threadException = ex;
                        }
                    });

                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join(5000); // Таймаут 5 секунд
                    
                    // Перевіряємо, чи виник виняток у потоці
                    if (threadException != null)
                    {
                        throw threadException;
                    }
                    
                    // Перевіряємо успішність копіювання
                    if (VerifyClipboardContent(text))
                    {
                        return; // Успіх!
                    }
                }
                catch (ExternalException ex)
                {
                    lastException = ex;
                    
                    // Перевіряємо, чи текст все ж таки скопіювався
                    Thread.Sleep(50);
                    if (VerifyClipboardContent(text))
                    {
                        return; // Копіювання успішне попри помилку
                    }
                    
                    if (i < maxRetries - 1)
                    {
                        Thread.Sleep(delayMilliseconds * (i + 1));
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Неочікувана помилка при записі в буфер обміну (спроба {i + 1}/{maxRetries}): {ex.GetType().Name} - {ex.Message}", 
                        ex);
                }
            }

            throw new InvalidOperationException(
                $"Не вдалося записати текст у буфер обміну після {maxRetries} спроб. " +
                $"Код помилки: 0x{Marshal.GetHRForException(lastException):X8}. " +
                $"Можливо, буфер обміну зайнятий іншою програмою.",
                lastException);
        }

        private bool VerifyClipboardContent(string expectedText)
        {
            try
            {
                if (Clipboard.ContainsText())
                {
                    string clipboardText = Clipboard.GetText();
                    return clipboardText == expectedText;
                }
            }
            catch
            {
                // Якщо не можемо перевірити - вважаємо, що не вдалося
            }
            return false;
        }
    }
}