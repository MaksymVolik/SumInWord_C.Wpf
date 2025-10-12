using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SumInWord_C.Wpf.BusinessLogic;
using SumInWord_C.Wpf.Services;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows;

namespace SumInWord_C.Wpf.ViewModels
{
    public partial class SumViewModel(IClipboardService clipboardService) : ObservableObject, INotifyDataErrorInfo
    {
        private const decimal VatRate = 0.20m;
        private readonly string _moneyFormat = "#,###0.00 'грн.'";
        private readonly CultureInfo _culture = CultureInfo.CurrentCulture;

        // ПРИХОВАНІ ЧИСЛОВІ ПОЛЯ ДЛЯ РОЗРАХУНКІВ
        private decimal _sum1; // Сума без ПДВ
        private decimal _sum2; // Сума ПДВ
        private decimal _sum3; // Всього з ПДВ

        // Прапорець для блокування рекурсивного виклику
        private bool _isRecalculating = false;

        // 1. Властивості, прив'язані до UI (string)
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

        private const byte currentLang = 1; // Ukrainian by default (1)

        private readonly IClipboardService _clipboardService = clipboardService ?? throw new ArgumentNullException(nameof(clipboardService));

        private readonly Dictionary<string, List<string>> _errors = [];

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return Array.Empty<string>();
            return _errors.TryGetValue(propertyName, out var list) ? list : Array.Empty<string>();
        }

        public SumViewModel() : this(new ClipboardService()) { }

        partial void OnSum1TextChanged(string value)
        {
            // === ЛОГІКА СКИДАННЯ (Якщо поле очищене) ===
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_isRecalculating) return;

                _isRecalculating = true;
                try
                {
                    // Скидаємо числові значення
                    _sum1 = _sum2 = _sum3 = 0;

                    // Скидаємо UI полів, на які впливає, на порожній рядок
                    Sum2Text = Sum3Text = string.Empty;
                }
                finally
                {
                    _isRecalculating = false;
                }
                return;
            }

            // === ЛОГІКА ПЕРЕРАХУНКУ ТА ФОРМАТУВАННЯ ===
            if (_isRecalculating) return; // Блокуємо, якщо це рекурсивний виклик

            _isRecalculating = true;
            try
            {
                if (ValidateAndParse(nameof(Sum1Text), value, out decimal parsedSum))
                {
                    // 1. БІЗНЕС-ЛОГІКА (Має бути першою, щоб _sumX були встановлені)
                    _sum1 = parsedSum;
                    _sum2 = Math.Round(_sum1 * VatRate, 2);
                    _sum3 = _sum1 + _sum2;

                    // 2. ФОРМАТУВАННЯ ВХІДНОГО ПОЛЯ (Sum1Text)
                    // Ми використовуємо _sum1, яке вже встановлено!
                    string formattedValue = _sum1.ToString("N2", _culture);

                    if (value != formattedValue)
                    {
                        Sum1Text = formattedValue; 
                    }

                    // 3. ЗВОРОТНЄ ОНОВЛЕННЯ ДРУГИХ ПОЛІВ
                    Sum2Text = _sum2.ToString("N2", _culture);
                    Sum3Text = _sum3.ToString("N2", _culture);
                }
            }
            finally
            {
                _isRecalculating = false;
            }
        }

        partial void OnSum2TextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_isRecalculating) return;

                _isRecalculating = true;
                try
                {
                    // Скидаємо числові значення
                    _sum1 = _sum2 = _sum3 = 0;

                    // Скидаємо UI полів, на які впливає, на порожній рядок
                    Sum1Text = Sum3Text = string.Empty;
                }
                finally
                {
                    _isRecalculating = false;
                }
                return;
            }

            // === ЛОГІКА ПЕРЕРАХУНКУ ТА ФОРМАТУВАННЯ ===
            if (_isRecalculating) return;

            _isRecalculating = true;
            try
            {
                if (ValidateAndParse(nameof(Sum2Text), value, out decimal parsedSum))
                {
                    // 1. БІЗНЕС-ЛОГІКА: Розрахунок Загальної Суми
                    _sum2 = parsedSum;
                    _sum3 = _sum1 + _sum2; // Сума без ПДВ + Сума ПДВ

                    // 1. Форматування ВХІДНОГО поля (Sum2Text)
                    string formattedValue = parsedSum.ToString("N2", _culture);
                    if (Sum2Text != formattedValue)
                    {
                        // Оновлюємо, щоб застосувати формат (наприклад, 100 -> 100,00)
                        Sum2Text = formattedValue;
                        // Після присвоєння Sum2Text, цей метод буде викликаний знову,
                        // але _isRecalculating заблокує його.
                    }

                    // 3. ЗВОРОТНЄ ОНОВЛЕННЯ ДРУГОГО ПОЛЯ
                    // Оновлюємо Sum3Text
                    Sum3Text = _sum3.ToString("N2", _culture);

                    // Оскільки Sum1Text не змінювався, його не чіпаємо.
                }
            }
            finally
            {
                _isRecalculating = false;
            }
        }

        partial void OnSum3TextChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                if (_isRecalculating) return;

                _isRecalculating = true;
                try
                {
                    // Скидаємо числові значення
                    _sum1 = _sum2 = _sum3 = 0;

                    // Скидаємо UI полів, на які впливає, на порожній рядок
                    Sum1Text = Sum2Text = string.Empty;
                }
                finally
                {
                    _isRecalculating = false;
                }
                return;
            }

            if (_isRecalculating) return;

            _isRecalculating = true;
            try
            {
                if (ValidateAndParse(nameof(Sum3Text), value, out decimal parsedSum))
                {
                    // 1. БІЗНЕС-ЛОГІКА: Розрахунок Sum1 та Sum2
                    _sum3 = parsedSum;

                    // Розрахунок Sum1 (Сума без ПДВ)
                    _sum1 = Math.Round(_sum3 / (1 + VatRate), 2);

                    // Розрахунок Sum2 (Сума ПДВ)
                    _sum2 = _sum3 - _sum1;

                    // 2. Форматування ВХІДНОГО поля (Sum3Text)
                    string formattedValue = parsedSum.ToString("N2", _culture);
                    if (Sum3Text != formattedValue)
                    {
                        // Оновлюємо, щоб застосувати формат
                        Sum3Text = formattedValue;
                    }

                    // 3. ЗВОРОТНЄ ОНОВЛЕННЯ ОБОХ ЗАЛЕЖНИХ ПОЛІВ
                    // Оновлюємо Sum1Text
                    Sum1Text = _sum1.ToString("N2", _culture);

                    // Оновлюємо Sum2Text
                    Sum2Text = _sum2.ToString("N2", _culture);
                }
            }
            finally
            {
                _isRecalculating = false;
            }
        }

        private bool ValidateAndParse(string propertyName, string? value, out decimal result)
        {
            // 1. Очищення та обробка порожнього поля (без змін)
            _errors.Remove(propertyName);
            if (string.IsNullOrWhiteSpace(value))
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                result = 0;
                return true;
            }

            // 2. Уніфікація роздільників (без змін) та вилучення роздільників тисяч
            string thousandSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string cleanedValue = value.Replace(thousandSeparator, string.Empty);
            string invariantValue = cleanedValue.Replace(',', '.');

            // 3. виклик TryParse
            if (!decimal.TryParse(
                invariantValue,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out decimal parsedResult))
            {
                // 4. Логіка помилки
                _errors[propertyName] = ["Некоректний числовий формат. Введіть число."];
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                result = 0;
                return false;
            }
            else
            {
                // 5. Логіка успіху
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
                result = parsedResult;
                return true;
            }
        }

        [RelayCommand]
        private void Convert()
        {
            // 1. Перевірка на помилки валідації (виконується при LostFocus)
            if (HasErrors) return; // Не продовжуємо, якщо є помилки

            // 2. Використання ВНУТРІШНІХ ЧИСЛОВИХ ПОЛІВ для розрахунків
            // _sum1, _sum2, _sum3 вже містять останні успішно спарсені числа.
            var culture = CultureInfo.CurrentCulture;

            // 3. Конвертація в слова (Напряму передаємо приватні поля)
            var text1 = AmountConverter.ConvertAmountToWords(_sum1, currentLang); // Сума без ПДВ
            var text2 = AmountConverter.ConvertAmountToWords(_sum2, currentLang); // Сума ПДВ
            var text3 = AmountConverter.ConvertAmountToWords(_sum3, currentLang); // Всього з ПДВ

            // 4. Оновлення результатів
            ConvertedText1 = text1;
            ConvertedText2 = text2;
            ConvertedText3 = text3;

            // 5. Формування підсумкового рядка
            ResultText = $"Загальна вартість робіт (послуг) складає без ПДВ {_sum1.ToString(_moneyFormat, culture)} ({text1}), " +
                $"ПДВ {VatRate:P0} {_sum2.ToString(_moneyFormat, culture)} ({text2}), разом {_sum3.ToString(_moneyFormat, culture)} ({text3}).";
        }

        [RelayCommand]
        private void Clear()
        {
            Sum1Text = Sum2Text = Sum3Text = ResultText = ConvertedText1 = ConvertedText2 = ConvertedText3 = string.Empty;
        }

        [RelayCommand]
        private void CopyText(string? textToCopy)
        {
            // 1. ПЕРЕВІРКА: Якщо текст порожній або null, виходимо.
            if (string.IsNullOrEmpty(textToCopy))
            {
                return;
            }
            try
            {
                _clipboardService.SetText(textToCopy);
            }
            catch (Exception)
            {
                MessageBox.Show("Не вдалося скопіювати у буфер обміну.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}