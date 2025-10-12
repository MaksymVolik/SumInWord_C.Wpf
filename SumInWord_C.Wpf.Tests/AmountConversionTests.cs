using Microsoft.VisualStudio.TestTools.UnitTesting;
using SumInWord_C.Wpf.BusinessLogic;

namespace SumInWord_C.Wpf.Tests
{
    [TestClass]
    public class AmountConversionTests
    {
        // Тестуємо клас AmountConverter з шару BusinessLogic

        // --- Тести для української мови (lang = 1) ---

        [TestMethod]
        [DataRow(0.00, "Нуль гривень 00 копійок")]
        [DataRow(1.00, "Одна гривня 00 копійок")]
        [DataRow(2.00, "Дві гривні 00 копійок")]
        [DataRow(5.00, "П'ять гривень 00 копійок")]
        [DataRow(10.00, "Десять гривень 00 копійок")]
        [DataRow(11.00, "Одинадцять гривень 00 копійок")]
        [DataRow(19.00, "Дев'ятнадцять гривень 00 копійок")]
        [DataRow(20.00, "Двадцять гривень 00 копійок")]
        [DataRow(21.00, "Двадцять одна гривня 00 копійок")]
        [DataRow(32.00, "Тридцять дві гривні 00 копійок")]
        [DataRow(99.00, "Дев'яносто дев'ять гривень 00 копійок")]
        [DataRow(100.00, "Сто гривень 00 копійок")]
        [DataRow(101.00, "Сто одна гривня 00 копійок")]
        [DataRow(115.00, "Сто п'ятнадцять гривень 00 копійок")]
        [DataRow(258.00, "Двісті п'ятдесят вісім гривень 00 копійок")]
        [DataRow(999.00, "Дев'ятсот дев'яносто дев'ять гривень 00 копійок")]
        [DataRow(777.00, "Сімсот сімдесят сім гривень 00 копійок")]
        public void ConvertAmountToWords_Ukr_IntegerAmounts_ReturnsCorrectString(double amount, string expected)
        {
            // Arrange
            byte lang = 1; // Українська
            decimal input = (decimal)amount;

            // Act
            string actual = AmountConverter.ConvertAmountToWords(input, lang); // Змінено виклик

            // Assert
            Assert.AreEqual(expected, actual, $"Input: {input}");
        }

        [TestMethod]
        [DataRow(0.01, "Нуль гривень 01 копійка")]
        [DataRow(0.02, "Нуль гривень 02 копійки")]
        [DataRow(0.05, "Нуль гривень 05 копійок")]
        [DataRow(0.10, "Нуль гривень 10 копійок")]
        [DataRow(0.11, "Нуль гривень 11 копійок")]
        [DataRow(0.25, "Нуль гривень 25 копійок")]
        [DataRow(0.99, "Нуль гривень 99 копійок")]
        public void ConvertAmountToWords_Ukr_OnlyKopecks_ReturnsCorrectString(double amount, string expected)
        {
            // Arrange
            byte lang = 1; // Українська
            decimal input = (decimal)amount;

            // Act
            string actual = AmountConverter.ConvertAmountToWords(input, lang); // Змінено виклик

            // Assert
            Assert.AreEqual(expected, actual, $"Input: {input}");
        }

        [TestMethod]
        [DataRow(123.45, "Сто двадцять три гривні 45 копійок")]
        [DataRow(1001.01, "Одна тисяча одна гривня 01 копійка")]
        [DataRow(2024.24, "Дві тисячі двадцять чотири гривні 24 копійки")]
        [DataRow(5678.90, "П'ять тисяч шістсот сімдесят вісім гривень 90 копійок")]
        [DataRow(12345.67, "Дванадцять тисяч триста сорок п'ять гривень 67 копійок")]
        [DataRow(1000000.00, "Один мільйон гривень 00 копійок")]
        [DataRow(2543123.50, "Два мільйони п'ятсот сорок три тисячі сто двадцять три гривні 50 копійок")]
        public void ConvertAmountToWords_Ukr_DecimalAmounts_ReturnsCorrectString(double amount, string expected)
        {
            // Arrange
            byte lang = 1; // Українська
            decimal input = (decimal)amount;

            // Act
            string actual = AmountConverter.ConvertAmountToWords(input, lang); // Змінено виклик

            // Assert
            Assert.AreEqual(expected, actual, $"Input: {input}");
        }

        // --- Тести для російської мови (lang = 0) ---
        // TODO: Додати аналогічні тести для російської мови (lang = 0),
        // якщо підтримка цієї мови є важливою.
        // Наприклад:
        /*
        [TestMethod]
        [DataRow(123.45, "Сто двадцать три гривны 45 копеек")]
        public void ConvertAmountToWords_Rus_DecimalAmounts_ReturnsCorrectString(double amount, string expected)
        {
            // Arrange
            byte lang = 0; // Російська
            decimal input = (decimal)amount;

            // Act
            string actual = AmountConverter.ConvertAmountToWords(input, lang); // Змінено виклик

            // Assert
            Assert.AreEqual(expected, actual, $"Input: {input}");
        }
        */

        [TestMethod]
        public void ConvertAmountToWords_LargeNumber_ReturnsErrorString()
        {
            // Arrange
            byte lang = 1;
            decimal input = 10000000000000.0M; // 10 трильйонів
            string expected = "занадто велике число";

            // Act
            string actual = AmountConverter.ConvertAmountToWords(input, lang); // Змінено виклик

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
