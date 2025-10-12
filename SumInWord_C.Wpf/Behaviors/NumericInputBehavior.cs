using Microsoft.Xaml.Behaviors;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;

namespace SumInWord_C.Wpf.Behaviors
{
    public class NumericInputBehavior : Behavior<TextBox>
    {
        private const int MaxDecimalPlaces = 2;

        // Оскільки ViewModel обробляє обидва роздільники (крапку та кому), 
        // Behavior повинен дозволяти обидва, але обмежити їх кількість.
        private readonly string _decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        private readonly string _alternateSeparator = (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ",") ? "." : ",";
        private readonly string _negativeSign = CultureInfo.CurrentCulture.NumberFormat.NegativeSign;

        // Зберігаємо обидва роздільники в одному рядку для спрощення перевірок
        private string AllowedSeparators => _decimalSeparator + _alternateSeparator;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewTextInput += OnPreviewTextInput;
            this.AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.PreviewTextInput -= OnPreviewTextInput;
            this.AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
            base.OnDetaching();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Дозволяємо керуючі символи
            if (e.Key == Key.Back || e.Key == Key.Delete || e.Key == Key.Tab ||
                e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = false;
                return;
            }

            if (e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;

                // 1. Примусово оновлюємо джерело (ViewModel)
                textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

                // 2. Дозволяємо системі обробити Enter, щоб спрацював InputBinding
                e.Handled = false;
                return;
            }

            if (e.Key == Key.Space)
            {
                e.Handled = true; // Блокуємо введення пробілу
                return;
            }

        }

        private void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string input = e.Text;
            e.Handled = true; // Блокуємо за замовчуванням

            if (input.Length != 1) return; // Обробляємо лише одиночні символи

            char inputChar = input[0];
            string currentText = textBox.Text;

            // Якщо довжина більша за 1 (вставка), ми повинні її заблокувати,
            // оскільки наша логіка не може коректно перевірити вставку
            if (e.Text.Length > 1)
            {
                e.Handled = true;
                return;
            }

            // 1. Дозволяємо цифри (0-9)
            if (char.IsDigit(inputChar))
            {
                // ЛОГІКА ОБМЕЖЕННЯ ДВОХ ЗНАКІВ

                // Знаходимо індекс першого роздільника (крапка або кома)
                int separatorIndex = -1;
                int commaIndex = currentText.IndexOf(',');
                int dotIndex = currentText.IndexOf('.');

                // Визначаємо, який роздільник був використаний першим
                if (commaIndex != -1 && (dotIndex == -1 || commaIndex < dotIndex))
                {
                    separatorIndex = commaIndex;
                }
                else if (dotIndex != -1)
                {
                    separatorIndex = dotIndex;
                }

                // Перевірка: якщо роздільник вже є, курсор після нього, і вже 2 знаки (або більше), і нічого не виділено
                if (separatorIndex != -1 &&
                    textBox.CaretIndex > separatorIndex &&
                    (currentText.Length - separatorIndex) > MaxDecimalPlaces &&
                    textBox.SelectionLength == 0)
                {
                    e.Handled = true; // Забороняємо
                    return;
                }

                e.Handled = false; // Дозволяємо цифру
                return;
            }

            // 2. Обробка десяткового роздільника (крапка або кома)
            if (inputChar.ToString() == _decimalSeparator || inputChar.ToString() == _alternateSeparator)
            {
                // Моделюємо текст, який залишиться після видалення виділеного
                string textWithoutSelection = currentText;
                if (textBox.SelectionLength > 0)
                {
                    textWithoutSelection = currentText.Remove(textBox.SelectionStart, textBox.SelectionLength);
                }

                // Перевіряємо, чи міститиме текст роздільник після введення
                if (!textWithoutSelection.Contains(_decimalSeparator) && !textWithoutSelection.Contains(_alternateSeparator))
                {
                    e.Handled = false; // Дозволяємо, якщо роздільника (крапки чи коми) ще немає
                }
                return;
            }

            // 3. Дозволяємо знак мінуса (-) (якщо це потрібно)
            //if (inputChar.ToString() == _negativeSign &&
            //    textBox.CaretIndex == 0 &&
            //    !currentText.Contains(_negativeSign))
            //{
            //    e.Handled = false;
            //    return;
            //}

            // Усі інші символи блокуються (e.Handled = true)
        }
    }
}