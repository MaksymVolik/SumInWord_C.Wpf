using System.Globalization;

namespace SumInWord_C.Wpf.BusinessLogic
{
    /// <summary>
    /// Надає функціональність для конвертації числових сум у текстове представлення українською або російською мовами.
    /// Повний код перенесено з оригінального WinForms-проєкту та адаптовано namespace для цього WPF-проєкту.
    /// </summary>
    public static class AmountConverter
    {
        // --- Статичні масиви для конвертації чисел у слова ---
        internal static readonly string[,] _hundredsNames = {
            {"", "сто", "двести", "триста", "четыреста", "пятьсот", "шестсот", "семьсот", "восемьсот", "девятьсот"},
            {"", "сто", "двісті", "триста", "чотириста", "п'ятсот", "шістсот", "сімсот", "вісімсот", "дев'ятсот"}
        };
        internal static readonly string[,] _teensNames = {
            {"", "десять", "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать", "семнадцать", "восемнадцать", "девятнадцать"},
            {"", "десять", "одинадцять", "дванадцять", "тринадцять", "чотирнадцять", "п'ятнадцять", "шістнадцять", "сімнадцять", "вісімнадцять", "дев'ятнадцять"}
        };
        internal static readonly string[,] _tensNames = {
            {"", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто"},
            {"", "двадцять", "тридцять", "сорок", "п'ятдесят", "шістдесят", "сімдесят", "вісімдесят", "дев'яносто"}
        };
        internal static readonly string[,] _unitsNames = { // (0: "", 1-9: звичайні жіночі/спільні, 10: "один"(ч), 11: "два"(ч))
            {"", "одна", "две", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять", "один", "два"},
            {"", "одна", "дві", "три", "чотири", "п'ять", "шість", "сім", "вісім", "дев'ять", "один", "два"}
        };
        internal static readonly string[,] _unitScaleNames = {
            {"", "гривна", "гривны", "гривен", "тысяча", "тысячи", "тысяч", "миллион", "миллиона", "миллионов", "миллиард", "миллиарда", "миллиардов", "триллион", "триллиона", "триллионов"},
            {"", "гривня", "гривні", "гривень", "тисяча", "тисячі", "тисяч", "мільйон", "мільйони", "мільйонів", "мільярд", "мільярди", "мільярдів", "трильйон", "трильйона", "трильйонів"}
        };
        internal static readonly string[,] _kopecksNames = {
            {"", " копейка", " копейки", " копеек"},
            {"", " копійка", " копійки", " копійок"}
        };
        // --- Кінець статичних масивів ---

        internal static string ConvertTripletToString(int tripletValue, byte lang, int requiredUnitGender)
        {
            if (tripletValue == 0) return ""; // Порожній тріплет

            string result = "";
            int hundredsDigit = tripletValue / 100;
            int tensAndUnitsValue = tripletValue % 100;
            int tensDigit = tensAndUnitsValue / 10;
            int unitsDigit = tensAndUnitsValue % 10;

            // Сотні
            if (hundredsDigit > 0)
            {
                result += _hundredsNames[lang, hundredsDigit] + " ";
            }

            // Десятки та одиниці
            if (tensDigit == 1) // Числа 10-19
            {
                result += _teensNames[lang, unitsDigit + 1] + " ";
            }
            else // Числа 0-9 та 20-99
            {
                if (tensDigit != 0)
                {
                    result += _tensNames[lang, tensDigit - 1] + " "; // -1 бо індекс масиву з 0
                }

                if (unitsDigit != 0)
                {
                    int unitIndex;
                    if (requiredUnitGender == 1) // Жіночий рід для гривень/тисяч - використаємо індекси 1 та 2
                    {
                        unitIndex = unitsDigit; // 1 -> "одна", 2 -> "дві" для української
                    }
                    else // Чоловічий рід для мільйонів/мільярдів - використаємо індекси 10 та 11
                    {
                        if (unitsDigit == 1) unitIndex = 10; // -> "один"
                        else if (unitsDigit == 2) unitIndex = 11; // -> "два"
                        else unitIndex = unitsDigit; // 3 -> "три" ...
                    }
                    result += _unitsNames[lang, unitIndex] + " ";
                }
            }
            return result.TrimEnd(); // Прибираємо зайві пробіли
        }

        internal static byte GetCaseIndex(int value)
        {
            int lastTwoDigits = value % 100;
            int lastDigit = value % 10;

            if (lastTwoDigits >= 11 && lastTwoDigits <= 19)
            {
                return 3; // "гривень", "тисяч", "копійок"
            }

            return lastDigit switch
            {
                1 => 1,// "гривня", "тисяча", "копійка"
                2 or 3 or 4 => 2,// "гривні", "тисячі", "копійки"
                _ => 3,// "гривень", "тисяч", "копійок" (0, 5-9)
            };
        }

        public static string ConvertAmountToWords(decimal amountToConvert, byte lang)
        {
            string integerPartString;
            string kopecksString;
            byte tripletCount;

            string result = "";
            try
            {
                // Перевірка на максимальне значення
                if (amountToConvert >= 10000000000000.0M) // 10 трильйонів
                {
                    return lang == 1 ? "занадто велике число" : "слишком большое число";
                }

                long integerPart = (long)Math.Truncate(amountToConvert); // Використовуємо long для більших чисел

                if (integerPart == 0)
                {
                    result = lang == 1 ? "нуль гривень" : "ноль гривен";
                }
                else
                {
                    integerPartString = integerPart.ToString();
                    tripletCount = (byte)((integerPartString.Length + 2) / 3);
                    // Доповнюємо нулями зліва до довжини, кратної 3
                    integerPartString = new string('0', tripletCount * 3 - integerPartString.Length) + integerPartString;

                    // Етапи: від найменшої групи до двох великих
                    for (byte i = tripletCount; i >= 1; i--)
                    {
                        // Витягуємо значення тріплета
                        int tripletValue = int.Parse(integerPartString.Substring((tripletCount - i) * 3, 3));
                        // Обробляємо, якщо тріплет > 0
                        if (tripletValue > 0)
                        {
                            // Визначаємо рід: 1 (жіночий) для гривень(i=1) та тисяч (i=2), 0 (чоловічий) для інших
                            int requiredUnitGender = (i == 1 || i == 2) ? 1 : 0;
                            result += ConvertTripletToString(tripletValue, lang, requiredUnitGender) + " "; // Додаємо пробіл після тріплета

                            // Отримуємо індекс відмінка (1,2,або 3) для назви розряду (гривня, тисяча, мільйон...)
                            byte unitCaseIndex = GetCaseIndex(tripletValue);
                            // Додаємо назву розряду (гривня, тисяча, мільйон...) на основі індексу тріплета 'i' та індексу відмінка
                            // Перевіряємо, чи індекс не виходить за межі масиву _unitScaleNames
                            int scaleIndex = (i - 1) * 3 + unitCaseIndex;
                            if (scaleIndex < _unitScaleNames.GetLength(1))
                            {
                                result += _unitScaleNames[lang, scaleIndex] + " "; // Додаємо пробіл після назви розряду
                            }
                        }
                        // Якщо це останній тріплет (гривні) і він нульовий, але ціла частина не нульова, додаємо "гривень"
                        else if (i == 1 && integerPart > 0)
                        {
                             result += _unitScaleNames[lang, 3] + " "; // Додаємо "гривень" або "рублей"
                        }
                    }
                    result = result.TrimEnd(); // Прибираємо зайвий пробіл в кінці
                }

                // --- Обробка копійок ---
                // Отримуємо останні два знаки після коми як коми
                kopecksString = amountToConvert.ToString("0.00", CultureInfo.InvariantCulture)[^2..];
                byte kopecksValue = byte.Parse(kopecksString);

                // Визначаємо індекс відмінка для копійок
                byte kopecksCaseIndex = GetCaseIndex(kopecksValue);

                // Додаємо копійки до результату
                result += " " + kopecksString + _kopecksNames[lang, kopecksCaseIndex];

                // Перша літера - велика
                if (!string.IsNullOrEmpty(result))
                {
                    result = char.ToUpper(result[0], CultureInfo.CurrentCulture) + result[1..];
                }
            }
            catch (Exception ex) // Залишаєм загальну обробку помилок
            {
                // Можливо, варто логувати помилку замість виводу користувачу
                // Logger.LogError(ex, "Помилка конвертації суми в слова");
                result = lang == 1 ? $"Помилка конвертації: {ex.Message}" : $"Ошибка конвертации: {ex.Message}";
            }

            return result;
        }
    }
}
