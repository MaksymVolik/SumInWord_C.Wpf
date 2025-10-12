using Microsoft.VisualStudio.TestTools.UnitTesting;
using SumInWord_C.Wpf.ViewModels;
using SumInWord_C.Wpf.Services;
using System;

namespace SumInWord_C.Wpf.Tests
{
    [TestClass]
    public class SumViewModelTests
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
            var fake = new FakeClipboardService();
            var vm = new SumViewModel(fake);
            vm.ConvertedText1 = "тестовий текст";

            // Act
            vm.CopyTextCommand.Execute(vm.ConvertedText1);

            // Assert
            Assert.AreEqual("тестовий текст", fake.LastText);
        }
    }
}
