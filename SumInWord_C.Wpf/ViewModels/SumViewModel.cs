using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SumInWord_C.Wpf.Interfaces;
using SumInWord_C.Wpf.Services;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace SumInWord_C.Wpf.ViewModels
{
    public enum ClearMode
    {
        FullClear,    // Очистити все (Виклик з кнопки UI)
        InputOnly     // Очистити лише поля введення та їх залежності (Виклик з OnSumXChanged)
    }
    public partial class SumViewModel(IClipboardService clipboardService, INumberParserService numberParserService, IAmountToWordsService amountToWordsService) : ObservableObject, INotifyDataErrorInfo
    {
        private const decimal VatRate = 0.20m;
        private const byte DefaultLang = 1; // Ukrainian by default (1)
        private const string MoneyFormat = "#,###0.00 'грн.'";

        private readonly CultureInfo _culture = CultureInfo.CurrentCulture;

        private readonly INumberParserService _numberParserService = numberParserService ?? throw new ArgumentNullException(nameof(numberParserService));
        private readonly IClipboardService _clipboardService = clipboardService ?? throw new ArgumentNullException(nameof(clipboardService));
        private readonly IAmountToWordsService _amountToWordsService = amountToWordsService ?? throw new ArgumentNullException(nameof(amountToWordsService));

        // Числові поля для розрахунків
        private decimal _sum1;
        private decimal _sum2;
        private decimal _sum3;

        // Прапорець для блокування рекурсивного виклику
        private bool _isRecalculating = false;

        // Властивості, прив'язані до UI
        [ObservableProperty]
        private string _sum1Text = string.Empty;

        [ObservableProperty]
        private string _sum2Text = string.Empty;

        [ObservableProperty]
        private string _sum3Text = string.Empty;

        [ObservableProperty]
        private string resultText = string.Empty;

        [ObservableProperty]
        private string convertedText1 = string.Empty;

        [ObservableProperty]
        private string convertedText2 = string.Empty;

        [ObservableProperty]
        private string convertedText3 = string.Empty;
        
        private readonly Dictionary<string, List<string>> _errors = [];
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public bool HasErrors => _errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return Array.Empty<string>();
            return _errors.TryGetValue(propertyName, out var list) ? list : Array.Empty<string>();
        }

        // Для сумісності з XAML-дизайнером
        public SumViewModel() : this(new ClipboardService(), new NumberParserService(), new AmountToWordsService()) { }

        partial void OnSum1TextChanged(string value)
        {
            if (HandleEmptyInput(value)) return;

            if (_isRecalculating) return;
            _isRecalculating = true;
            try
            {
                _errors.Remove(nameof(Sum1Text));
                if (_numberParserService.TryParse(value, out decimal parsedSum, out string? error))
                {
                    _sum1 = parsedSum;
                    _sum2 = Math.Round(_sum1 * VatRate, 2);
                    _sum3 = _sum1 + _sum2;

                    string formattedValue = _sum1.ToString("N2", _culture);
                    if (value != formattedValue)
                    {
                        Sum1Text = formattedValue;
                    }

                    Sum2Text = _sum2.ToString("N2", _culture);
                    Sum3Text = _sum3.ToString("N2", _culture);
                }
                else
                {
                    _errors[nameof(Sum1Text)] = [error ?? "Некоректний числовий формат."];
                }
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Sum1Text)));
            }
            finally
            {
                _isRecalculating = false;
            }
        }

        partial void OnSum2TextChanged(string value)
        {
            if (HandleEmptyInput(value)) return;

            if (_isRecalculating) return;
            _isRecalculating = true;
            try
            {
                _errors.Remove(nameof(Sum2Text));
                if (_numberParserService.TryParse(value, out decimal parsedSum, out string? error))
                {
                    _sum2 = parsedSum;
                    _sum3 = _sum1 + _sum2;

                    string formattedValue = parsedSum.ToString("N2", _culture);
                    if (Sum2Text != formattedValue)
                    {
                        Sum2Text = formattedValue;
                    }

                    Sum3Text = _sum3.ToString("N2", _culture);
                }
                else
                {
                    _errors[nameof(Sum2Text)] = [error ?? "Некоректний числовий формат."];
                }
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Sum2Text)));
            }
            finally
            {
                _isRecalculating = false;
            }
        }

        partial void OnSum3TextChanged(string value)
        {
            if (HandleEmptyInput(value)) return;

            if (_isRecalculating) return;
            _isRecalculating = true;
            try
            {
                _errors.Remove(nameof(Sum3Text));
                if (_numberParserService.TryParse(value, out decimal parsedSum, out string? error))
                {
                    _sum3 = parsedSum;
                    _sum1 = Math.Round(_sum3 / (1 + VatRate), 2);
                    _sum2 = _sum3 - _sum1;

                    string formattedValue = parsedSum.ToString("N2", _culture);
                    if (Sum3Text != formattedValue)
                    {
                        Sum3Text = formattedValue;
                    }

                    Sum1Text = _sum1.ToString("N2", _culture);
                    Sum2Text = _sum2.ToString("N2", _culture);
                }
                else
                {
                    _errors[nameof(Sum3Text)] = [error ?? "Некоректний числовий формат."];
                }
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Sum3Text)));
            }
            finally
            {
                _isRecalculating = false;
            }
        }

        [RelayCommand]
        private void Convert()
        {
            if (HasErrors) return;

            var culture = CultureInfo.CurrentCulture;

            var text1 = _amountToWordsService.ConvertAmountToWords(_sum1, DefaultLang);
            var text2 = _amountToWordsService.ConvertAmountToWords(_sum2, DefaultLang);
            var text3 = _amountToWordsService.ConvertAmountToWords(_sum3, DefaultLang);

            ConvertedText1 = text1;
            ConvertedText2 = text2;
            ConvertedText3 = text3;

            ResultText = $"Загальна вартість робіт (послуг) складає без ПДВ {_sum1.ToString(MoneyFormat, culture)} ({text1}), " +
                $"ПДВ {VatRate:P0} {_sum2.ToString(MoneyFormat, culture)} ({text2}), разом {_sum3.ToString(MoneyFormat, culture)} ({text3}).";
        }

        [RelayCommand]
        private void Clear(ClearMode mode = ClearMode.FullClear)
        {
            Sum1Text = Sum2Text = Sum3Text = string.Empty;
            _sum1 = _sum2 = _sum3 = 0m;
            if (mode == ClearMode.FullClear) ResultText = ConvertedText1 = ConvertedText2 = ConvertedText3 = string.Empty;
            _errors.Clear();
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(string.Empty));
        }

        [RelayCommand]
        private void CopyText(string? textToCopy)
        {
            if (string.IsNullOrEmpty(textToCopy))
                return;
            try
            {
                _clipboardService.SetText(textToCopy);
            }
            catch (Exception ex)
            {
                // Можна додати логування через ILogger
                MessageBox.Show("Не вдалося скопіювати у буфер обміну.\n" + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool HandleEmptyInput(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_isRecalculating) return true;
                _isRecalculating = true;
                try
                {
                    Clear(ClearMode.InputOnly);
                }
                finally
                {
                    _isRecalculating = false;
                }
                return true;
            }
            return false;
        }
    }
}