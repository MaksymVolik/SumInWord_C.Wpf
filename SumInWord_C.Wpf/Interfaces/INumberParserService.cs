namespace SumInWord_C.Wpf.Interfaces
{
    public interface INumberParserService
    {
        /// <summary>
        /// Парсить рядок у decimal, повертає помилку у error, якщо формат некоректний.
        /// </summary>
        /// <param name="value">Вхідний рядок</param>
        /// <param name="result">Результат парсингу</param>
        /// <param name="error">Текст помилки (або null)</param>
        /// <returns>true, якщо парсинг успішний; false, якщо помилка</returns>
        bool TryParse(string value, out decimal result, out string? error);
    }
}