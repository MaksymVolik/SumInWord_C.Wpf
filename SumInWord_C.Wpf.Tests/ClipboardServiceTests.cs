using Microsoft.VisualStudio.TestTools.UnitTesting;
using SumInWord_C.Wpf.ViewModels;
using SumInWord_C.Wpf.Services;
using SumInWord_C.Wpf.Interfaces;

namespace SumInWord_C.Wpf.Tests
{
    [TestClass]
    public class ClipboardServiceTests
    {
        private class FakeClipboardService : IClipboardService
        {
            public string? LastText { get; private set; }
            public void SetText(string text)
            {
                LastText = text;
            }
        }

        [TestMethod]
        public void CopyConverted1Command_InvokesClipboardService()
        {
            // Arrange
            var fakeClipboard = new FakeClipboardService();
            var parser = new NumberParserService();
            var amountToWords = new AmountToWordsService();
            var vm = new SumViewModel(fakeClipboard, parser, amountToWords);
            vm.ConvertedText1 = "тестовий текст";

            // Act
            vm.CopyTextCommand.Execute(vm.ConvertedText1);

            // Assert
            Assert.AreEqual("тестовий текст", fakeClipboard.LastText);
        }
    }
}
