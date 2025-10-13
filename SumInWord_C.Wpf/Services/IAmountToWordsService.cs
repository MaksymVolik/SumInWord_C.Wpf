namespace SumInWord_C.Wpf.Services
{
    public interface IAmountToWordsService
    {
        string ConvertAmountToWords(decimal amount, byte lang);
    }
}