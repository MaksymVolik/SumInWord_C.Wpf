namespace SumInWord_C.Wpf.Interfaces
{
    public interface IAmountToWordsService
    {
        string ConvertAmountToWords(decimal amount, byte lang);
    }
}