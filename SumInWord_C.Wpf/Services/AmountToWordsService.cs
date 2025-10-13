using SumInWord_C.Wpf.BusinessLogic;

namespace SumInWord_C.Wpf.Services
{
    public class AmountToWordsService : IAmountToWordsService
    {
        public string ConvertAmountToWords(decimal amount, byte lang)
        {
            // Використовуємо ваш існуючий клас
            return AmountConverter.ConvertAmountToWords(amount, lang);
        }
    }
}