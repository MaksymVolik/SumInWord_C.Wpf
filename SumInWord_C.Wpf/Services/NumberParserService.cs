using System.Globalization;

namespace SumInWord_C.Wpf.Services
{
    public class NumberParserService : INumberParserService
    {
        public bool TryParse(string value, out decimal result, out string? error)
        {
            error = null;
            result = 0;
            if (string.IsNullOrWhiteSpace(value))
                return true;

            // Видаляємо роздільники тисяч та уніфікуємо десятковий роздільник
            string cleanedValue = value.Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, string.Empty)
                                       .Replace(',', '.');

            if (!decimal.TryParse(cleanedValue, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out result))
            {
                error = "Некоректний числовий формат. Введіть число.";
                return false;
            }
            return true;
        }
    }
}