using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace GoogleCalculator
{
    public partial class MainWindow : Window
    {
        private string currentInput = "";
        private double? firstOperand;
        private double? secondOperand;
        private string selectedOperator = "";
        private bool isNewCalculation = true;
        private Dictionary<string, Button> buttons;

        public MainWindow()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.ShowInTaskbar = false;
            this.Visibility = Visibility.Hidden;
            this.KeyDown += MainWindow_KeyDown;
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            buttons = new Dictionary<string, Button>();
            foreach (var child in ((Grid)((Border)this.Content).Child).Children)
            {
                if (child is Button button)
                {
                    buttons[button.Content.ToString()] = button;
                }
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D0:
                case Key.NumPad0:
                    NumberButton_Click(GetButtonByContent("0"), null);
                    break;
                case Key.D1:
                case Key.NumPad1:
                    NumberButton_Click(GetButtonByContent("1"), null);
                    break;
                case Key.D2:
                case Key.NumPad2:
                    NumberButton_Click(GetButtonByContent("2"), null);
                    break;
                case Key.D3:
                case Key.NumPad3:
                    NumberButton_Click(GetButtonByContent("3"), null);
                    break;
                case Key.D4:
                case Key.NumPad4:
                    NumberButton_Click(GetButtonByContent("4"), null);
                    break;
                case Key.D5:
                case Key.NumPad5:
                    NumberButton_Click(GetButtonByContent("5"), null);
                    break;
                case Key.D6:
                case Key.NumPad6:
                    NumberButton_Click(GetButtonByContent("6"), null);
                    break;
                case Key.D7:
                case Key.NumPad7:
                    NumberButton_Click(GetButtonByContent("7"), null);
                    break;
                case Key.D8:
                case Key.NumPad8:
                    NumberButton_Click(GetButtonByContent("8"), null);
                    break;
                case Key.D9:
                case Key.NumPad9:
                    NumberButton_Click(GetButtonByContent("9"), null);
                    break;
                case Key.Add:
                    OperatorButton_Click(GetButtonByContent("+"), null);
                    break;
                case Key.Subtract:
                    OperatorButton_Click(GetButtonByContent("-"), null);
                    break;
                case Key.Multiply:
                    OperatorButton_Click(GetButtonByContent("*"), null);
                    break;
                case Key.Divide:
                    OperatorButton_Click(GetButtonByContent("/"), null);
                    break;
                case Key.Decimal:
                    NumberButton_Click(GetButtonByContent("."), null);
                    break;
                case Key.Enter:
                    EqualsButton_Click(null, null);
                    break;
            }
        }

        private Button GetButtonByContent(string content)
        {
            if (buttons.ContainsKey(content))
            {
                return buttons[content];
            }
            return null;
        }


        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            if (isNewCalculation)
            {
                currentInput = "";
                isNewCalculation = false;
            }
            Button button = (Button)sender;
            currentInput += button.Content.ToString();
            Display.Text = currentInput;
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string op = button.Content.ToString();

            if (op == "+/-")
            {
                if (!string.IsNullOrEmpty(currentInput))
                {
                    if (currentInput.StartsWith("-"))
                    {
                        currentInput = currentInput.Substring(1);
                    }
                    else
                    {
                        currentInput = "-" + currentInput;
                    }
                    Display.Text = currentInput;
                }
            }
            else if (op == "%")
            {
                if (!string.IsNullOrEmpty(currentInput))
                {
                    double value = double.Parse(currentInput);
                    currentInput = (value / 100).ToString();
                    Display.Text = currentInput;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(currentInput))
                {
                    if (firstOperand == null)
                    {
                        firstOperand = double.Parse(currentInput);
                    }
                    else
                    {
                        secondOperand = double.Parse(currentInput);
                        Calculate();
                    }
                }
                selectedOperator = op;
                isNewCalculation = true;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            currentInput = "";
            firstOperand = null;
            secondOperand = null;
            selectedOperator = "";
            Display.Text = "0";
            isNewCalculation = true;
        }

        private void EqualsButton_Click(object sender, RoutedEventArgs e)
        {
            if (firstOperand != null && !string.IsNullOrEmpty(currentInput))
            {
                secondOperand = double.Parse(currentInput);
                Calculate();
                isNewCalculation = true;
            }
        }

        private void Calculate()
        {
            if (firstOperand == null || secondOperand == null) return;

            double result = 0;
            try
            {
                switch (selectedOperator)
                {
                    case "+":
                        result = firstOperand.Value + secondOperand.Value;
                        break;
                    case "-":
                        result = firstOperand.Value - secondOperand.Value;
                        break;
                    case "*":
                        result = firstOperand.Value * secondOperand.Value;
                        break;
                    case "/":
                        if (secondOperand.Value == 0)
                        {
                            Display.Text = "Error";
                            return;
                        }
                        result = firstOperand.Value / secondOperand.Value;
                        break;
                }
                Display.Text = result.ToString();
                firstOperand = result;
                secondOperand = null;
                currentInput = result.ToString();
            }
            catch (Exception)
            {
                Display.Text = "Error";
            }
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            var source = HwndSource.FromHwnd(helper.Handle);
            source.AddHook(HwndHook);
            RegisterHotKey(helper.Handle, 9000, (uint)ModifierKeys.Control, (uint)KeyInterop.VirtualKeyFromKey(Key.OemTilde));
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY && wParam.ToInt32() == 9000)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}
