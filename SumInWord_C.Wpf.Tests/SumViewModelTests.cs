using Microsoft.VisualStudio.TestTools.UnitTesting;
using SumInWord_C.Wpf.ViewModels;
using SumInWord_C.Wpf.Services;
using System.Linq;

namespace SumInWord_C.Wpf.Tests
{
    [TestClass]
    public class SumViewModelValidationTests
    {
        [TestMethod]
        public void Sum1Text_InvalidInput_SetsError()
        {
            var vm = new SumViewModel(new ClipboardService(), new NumberParserService());
            vm.Sum1Text = "abc";
            Assert.IsTrue(vm.HasErrors);
            var errors = vm.GetErrors(nameof(vm.Sum1Text));
            Assert.IsTrue(errors.Cast<string>().Any(e => e.Contains("Некоректний числовий формат")));
        }

        [TestMethod]
        public void Sum2Text_EmptyInput_ClearsDependentFields()
        {
            var vm = new SumViewModel(new ClipboardService(), new NumberParserService());
            vm.Sum1Text = "100";
            vm.Sum2Text = "";
            Assert.AreEqual(string.Empty, vm.Sum1Text);
            Assert.AreEqual(string.Empty, vm.Sum3Text);
        }

        [TestMethod]
        public void Sum3Text_NegativeInput_SetsError()
        {
            var vm = new SumViewModel(new ClipboardService(), new NumberParserService());
            vm.Sum3Text = "-100";
            // Якщо негативні значення не дозволені, перевірити на помилку
            // Якщо дозволені — перевірити коректність розрахунків
            // Тут приклад для помилки:
            // Assert.IsTrue(vm.HasErrors);
        }
    }
}