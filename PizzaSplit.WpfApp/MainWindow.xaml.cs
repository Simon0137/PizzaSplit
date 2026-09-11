using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PizzaSplit.Core;

namespace PizzaSplit.WpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public double? OrderSum { get; private set; }
    public int? CustomersCount { get; private set; }

    public double Result { get; private set; }
    public double TipsResult { get; private set; }

    private static readonly Regex RegexMatch = new(@"^\d+([,.]\d{0,2})?$");
    private static readonly Color ForegroundColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x00);
    private static readonly Color DefaultForegroundColor = Color.FromArgb(0xBB, 0xAB, 0xAB, 0xAB);
    private static readonly Color ErrorColor = Color.FromArgb(0xFF, 0xD8, 0x34, 0x34);
    private static readonly Color BorderColor = Color.FromArgb(0xFF, 0xAB, 0xAD, 0xB3);

    private bool _orderBoxIsEmpty = true;
    private bool _customerBoxIsEmpty = true;

    public MainWindow()
    {
        InitializeComponent();

        OrderBox.Text = "до 10000 €";
        OrderBox.Foreground = new SolidColorBrush(Color.FromArgb(0xBB, 0xAB, 0xAB, 0xAB));

        OrderErrorLabel.Foreground = new SolidColorBrush(ErrorColor);
        OrderErrorLabel.Visibility = Visibility.Hidden;

        CustomerBox.Text = ">0";
        CustomerBox.Foreground = new SolidColorBrush(Color.FromArgb(0xBB, 0xAB, 0xAB, 0xAB));

        CustomersErrorLabel.Foreground = new SolidColorBrush(ErrorColor);
        CustomersErrorLabel.Visibility = Visibility.Hidden;

        ResultBlock.Text = string.Empty;
        ResultBlock.Visibility = Visibility.Hidden;
    }

    private void OrderBox_GotFocus(object sender, RoutedEventArgs e)
    {
        OrderBox.Foreground = new SolidColorBrush(ForegroundColor);

        if (_orderBoxIsEmpty) OrderBox.Text = string.Empty;
        else OrderBox.Text = OrderBox.Text[..^2];
    }

    private void OrderBox_LostFocus(object sender, RoutedEventArgs e)
    {
        var boxText = OrderBox.Text;
        OrderSum = null;

        if (boxText == string.Empty)
        {
            ClearTextBoxError(OrderBox, OrderErrorLabel);
            OrderBox.Text = "до 10000 €";
            OrderBox.Foreground = new SolidColorBrush(Color.FromArgb(0xBB, 0xAB, 0xAB, 0xAB));
            _orderBoxIsEmpty = true;
            return;
        }

        OrderBox.Text += " €";
        _orderBoxIsEmpty = false;

        if (!double.TryParse(boxText.Replace(',', '.'), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var orderSum))
        {
            VisualizeTextBoxError(OrderBox, OrderErrorLabel, "Сумма заказа должна быть числом");
            return;
        }

        switch (orderSum)
        {
            case <= 0:
                VisualizeTextBoxError(OrderBox, OrderErrorLabel, "Cумма заказа должна быть больше 0");
                return;
            case > 10000:
                VisualizeTextBoxError(OrderBox, OrderErrorLabel, "Cумма заказа не должна превышать 10000");
                return;
            default:
                OrderSum = Math.Round(orderSum, 2);
                ClearTextBoxError(OrderBox, OrderErrorLabel);
                break;
        }
    }

    private void OrderBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var boxText = OrderBox.Text;

        var input = e.Text.Replace('.', ',');
        var isValid = ValidateInput(input, ref boxText);
        if (isValid)
        {
            if (input[0] is not ',')
            {
                e.Handled = !isValid;
                return;
            }

            OrderBox.Text = boxText;
            OrderBox.CaretIndex = OrderBox.Text.Length;
        }
        e.Handled = true;
    }

    private void OrderBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (OrderErrorLabel is { IsVisible: true }) OrderErrorLabel.Visibility = Visibility.Hidden;
    }

    private void CustomerBox_GotFocus(object sender, RoutedEventArgs e)
    {
        CustomerBox.Foreground = new SolidColorBrush(ForegroundColor);

        if (_customerBoxIsEmpty) CustomerBox.Text = string.Empty;
    }

    private void CustomerBox_LostFocus(object sender, RoutedEventArgs e)
    {
        var boxText = CustomerBox.Text;
        CustomersCount = null;

        if (boxText == string.Empty)
        {
            ClearTextBoxError(CustomerBox, CustomersErrorLabel);
            CustomerBox.Text = ">0";
            CustomerBox.Foreground = new SolidColorBrush(Color.FromArgb(0xBB, 0xAB, 0xAB, 0xAB));
            _customerBoxIsEmpty = true;
            return;
        }
        _customerBoxIsEmpty = false;

        if (!int.TryParse(CustomerBox.Text, out var customersCount))
        {
            VisualizeTextBoxError(CustomerBox, CustomersErrorLabel, "Количество посетителей должно быть целым числом");
            return;
        }

        switch (customersCount)
        {
            case <= 0:
                VisualizeTextBoxError(CustomerBox, CustomersErrorLabel, "Количество посетителей должно быть больше 0");
                return;
            default:
                CustomersCount = customersCount;
                ClearTextBoxError(CustomerBox, CustomersErrorLabel);
                break;
        }
    }

    private void CustomerBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var boxText = CustomerBox.Text;

        if (byte.TryParse(e.Text, out _))
        {
            CustomerBox.Text = boxText + e.Text;
            CustomerBox.CaretIndex = CustomerBox.Text.Length;
        }
        e.Handled = true;
    }

    private void CustomerBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CustomersErrorLabel is { IsVisible: true }) CustomersErrorLabel.Visibility = Visibility.Hidden;
    }

    private static void VisualizeTextBoxError(TextBox box, Label errorLabel, string errorLabelText)
    {
        box.BorderBrush = new SolidColorBrush(ErrorColor);
        box.Foreground = new SolidColorBrush(ErrorColor);

        errorLabel.Content = errorLabelText;
        errorLabel.Visibility = Visibility.Visible;
    }

    private static void ClearTextBoxError(TextBox box, Label errorLabel)
    {
        box.BorderBrush = new SolidColorBrush(BorderColor);
        box.Foreground = new SolidColorBrush(ForegroundColor);

        errorLabel.Visibility = Visibility.Hidden;
    }

    private static void ResetTextBox(TextBox box, string defaultText = "")
    {
        box.BorderBrush = new SolidColorBrush(BorderColor);
        box.Foreground = new SolidColorBrush(DefaultForegroundColor);
        box.Text = defaultText;
    }

    private void CalculateButton_Click(object sender, RoutedEventArgs e)
    {
        if (OrderSum != null && CustomersCount != null)
        {
            Result = BillCalculator.CalculateOrderPerCustomer(OrderSum.Value, CustomersCount.Value);
            TipsResult = Result + BillCalculator.CalculateTipsPerCustomer(OrderSum.Value, CustomersCount.Value);

            ResultBlock.Text = $"{(TipsCheck.IsChecked ?? false ? TipsResult : Result):F2} €";
            ResultBlock.Visibility = Visibility.Visible;
        }
        else
        {
            ResultBlock.Text = string.Empty;
            ResultBlock.Visibility = Visibility.Hidden;
        }
    }

    private bool ValidateInput(string newInput, ref string currentNumber)
    {
        if (newInput[0] is ',' && !currentNumber.Contains(','))
        {
            currentNumber += newInput;
            return true;
        }

        if (!byte.TryParse(newInput, out _)) return false;
        if ((currentNumber.Contains(',') && currentNumber[(currentNumber.IndexOf(',') + 1)..].Length < 2)
            || (!currentNumber.Contains(',') && currentNumber.Length < 5))
        {
            currentNumber += newInput;
            return true;
        }

        return false;
    }

    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        ResultBlock.Text = $"{TipsResult:F2} €";
    }

    private void TipsCheck_Unchecked(object sender, RoutedEventArgs e)
    {
        ResultBlock.Text = $"{Result:F2} €";
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        OrderSum = null;
        CustomersCount = null;

        ResultBlock.Visibility = Visibility.Hidden;
        ResultBlock.Text = string.Empty;

        TipsCheck.IsChecked = false;

        ResetTextBox(CustomerBox, ">0");
        CustomersErrorLabel.Visibility = Visibility.Hidden;
        _customerBoxIsEmpty = true;

        ResetTextBox(OrderBox, "до 10000 €");
        OrderErrorLabel.Visibility = Visibility.Hidden;
        _orderBoxIsEmpty = true;
    }
}