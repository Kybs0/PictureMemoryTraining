using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PictureMemoryTraining.Utils;

namespace PictureMemoryTraining.Views.Controls
{
    public class LimitedTextBox : TextBox
    {
        public static readonly DependencyProperty LimitTypeProperty = DependencyProperty.Register(
            "LimitType", typeof(LimitType), typeof(LimitedTextBox), new PropertyMetadata(default(LimitType), OnLimitTypePropertyChanged));

        public LimitType LimitType
        {
            get { return (LimitType)GetValue(LimitTypeProperty); }
            set { SetValue(LimitTypeProperty, value); }
        }

        private static void OnLimitTypePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var textbox = (LimitedTextBox)dependencyObject;
            var limitType = (LimitType)e.NewValue;
            switch (limitType)
            {
                case LimitType.FileNameAllowCharacters:
                    {
                        //禁止文件名不合法字符\:/*?\"<>
                        textbox.ForbiddenPattern = RegexConst.IllegalCharactes;
                        break;
                    }
                case LimitType.NumberOnly:
                    {
                        //禁止除数字和小数点之外的字符
                        textbox.ForbiddenPattern = RegexConst.NumbersPointOnly;
                        InputMethod.SetIsInputMethodEnabled(textbox, false);
                        break;
                    }
            }
        }

        /// <summary>
        /// 禁止字符的匹配模式
        /// </summary>
        protected string ForbiddenPattern;
        private bool _isInnerTextChanged;
        private bool _isValid = true;
        private string _lastText;
        public event EventHandler<bool> ValidateChanged;
        protected virtual void OnValidateChanged(bool isValid)
        {
            ValidateChanged?.Invoke(this, isValid);
        }

        #region 仅当限定输入为数值才有效的附加属性

        public static readonly DependencyProperty AllowDecimalProperty = DependencyProperty.Register(
            "AllowDecimal", typeof(bool), typeof(LimitedTextBox), new PropertyMetadata(default(bool)));


        public bool AllowDecimal
        {
            get { return (bool)GetValue(AllowDecimalProperty); }
            set { SetValue(AllowDecimalProperty, value); }
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(int), typeof(LimitedTextBox), new PropertyMetadata(int.MinValue));


        /// <summary>
        /// 最小值，最好不要大于1，否则可能无法输入
        /// </summary>
        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(int), typeof(LimitedTextBox), new PropertyMetadata(int.MaxValue));


        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        #endregion

        #region 获取焦点时，是否全选

        public bool SelectAllWhenKeyBoardFocused
        {
            get { return (bool)GetValue(SelectAllWhenKeyBoardFocusedProperty); }
            set { SetValue(SelectAllWhenKeyBoardFocusedProperty, value); }
        }

        public static readonly DependencyProperty SelectAllWhenKeyBoardFocusedProperty = DependencyProperty.Register("SelectAllWhenKeyBoardFocused", typeof(bool), typeof(LimitedTextBox),
        new PropertyMetadata(true, SelectAllWhenKeyBoardFocusedPropertyChanged));

        private static void SelectAllWhenKeyBoardFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textBox = d as LimitedTextBox;
            textBox?.RemoveHandler(GotKeyboardFocusEvent, new RoutedEventHandler(textBox.SelectAllText));
            if ((bool)e.NewValue)
            {
                textBox?.AddHandler(GotKeyboardFocusEvent, new RoutedEventHandler(textBox.SelectAllText), true);
            }
        }

        #endregion

        public LimitedTextBox()
        {
            TextChanged += LimitedTextBox_TextChanged;

            AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText), true);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Key == Key.Enter)
            {
                Keyboard.ClearFocus();
            }
        }

        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            if (!IsKeyboardFocusWithin)
            {
                Focus();
            }
        }

        private void SelectAllText(object sender, RoutedEventArgs e)
        {
            SelectAll();
        }

        private void LimitedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isInnerTextChanged)
                return;
            if (string.IsNullOrEmpty(ForbiddenPattern))
                return;

            int oldCaretIndex = CaretIndex;
            if (oldCaretIndex < 0)
                return;
            string frontText = Text.Substring(0, oldCaretIndex);
            string backText = Text.Substring(oldCaretIndex);
            string regexString = ForbiddenPattern;
            var regex = new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

            //如果文本为空，则不再处理
            if (string.IsNullOrEmpty(Text))
                return;

            if (regex.IsMatch(frontText))
            {
                var newText = Regex.Replace(frontText, regexString, "");
                _isInnerTextChanged = true;
                Text = newText + backText;
                //恢复光标之前的位置
                Select(newText.Length, 0);
                _isInnerTextChanged = false;

                EnsureNumberOnly(frontText, regexString);

                if (_isValid)
                {
                    _isValid = false;
                    OnValidateChanged(_isValid);
                }
            }
            else
            {
                EnsureNumberOnly(frontText, regexString);
                if (!_isValid)
                {
                    _isValid = true;
                    OnValidateChanged(_isValid);
                }
            }
        }

        private void EnsureNumberOnly(string frontText, string regexString)
        {
            if (LimitType == LimitType.NumberOnly)
            {
                if (AllowDecimal)
                {
                    double value;
                    if (double.TryParse(Text, out value))
                    {
                        _lastText = Text;
                    }
                }
                else
                {
                    long value;
                    if (long.TryParse(Text, out value))
                    {
                        _lastText = Text;
                    }
                }
                _isInnerTextChanged = true;
                Text = _lastText;
                _isInnerTextChanged = false;
                var newText = Regex.Replace(frontText, regexString, "");
                Select(newText.Length, 0);
            }
        }

        private bool CheckValueRange(double value)
        {
            return (MinValue == int.MinValue || value >= MinValue) && (MaxValue == int.MaxValue || value <= MaxValue);
        }
    }

    public enum LimitType
    {
        None,
        NumberOnly,
        FileNameAllowCharacters,
    }
}
