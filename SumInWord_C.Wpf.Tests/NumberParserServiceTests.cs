using Microsoft.VisualStudio.TestTools.UnitTesting;
using SumInWord_C.Wpf.Services;

namespace SumInWord_C.Wpf.Tests
{
    [TestClass]
    public class NumberParserServiceTests
    {
        private readonly NumberParserService _parser = new();

        [TestMethod]
        [DataRow("0", 0.0)]
        [DataRow("1234,56", 1234.56)]
        [DataRow("1 234,56", 1234.56)]
        [DataRow("1234.56", 1234.56)]
        [DataRow("  1000  ", 1000.0)]
        public void TryParse_ValidNumbers_ReturnsTrue(string input, double expected)
        {
            var result = _parser.TryParse(input, out decimal value, out string? error);
            Assert.IsTrue(result);
            Assert.AreEqual((decimal)expected, value);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("abc")]
        [DataRow("12,34,56")]
        [DataRow("")]
        [DataRow(" ")]
        public void TryParse_InvalidNumbers_ReturnsFalse(string input)
        {
            var result = _parser.TryParse(input, out decimal value, out string? error);
            Assert.IsTrue(string.IsNullOrWhiteSpace(input) ? result : !result);
            if (!result)
            {
                Assert.IsNotNull(error);
                Assert.Contains("Некоректний числовий формат", error);
            }
        }
    }
}