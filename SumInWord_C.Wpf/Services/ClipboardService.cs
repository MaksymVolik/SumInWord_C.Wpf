using System;
using System.Windows;

namespace SumInWord_C.Wpf.Services
{
    public class ClipboardService : IClipboardService
    {
        public void SetText(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            const int maxRetries = 3;
            const int delayMilliseconds = 100;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    Clipboard.SetText(text);
                    return;
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    if (i < maxRetries - 1) System.Threading.Thread.Sleep(delayMilliseconds);
                }
            }
            throw new InvalidOperationException("Не вдалося записати текст у буфер обміну.");
        }
    }
}

