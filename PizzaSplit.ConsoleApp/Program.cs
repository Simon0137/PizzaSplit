using PizzaSplit.Core;

namespace PizzaSplit.ConsoleApp;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.Write("Введите сумму заказа: ");
        float orderSum;
        while (true)
        {
            var orderStr = Console.ReadLine();

            if (!float.TryParse(orderStr, out orderSum))
            {
                Console.Write("Сумма заказа должна быть числом: ");
                continue;
            }

            if (orderSum <= 0)
            {
                Console.Write("Cумма заказа должна быть больше 0: ");
                continue;
            }

            if (orderSum > 10000)
            {
                Console.Write("Cумма заказа не должна превышать 10000: ");
                continue;
            }

            break;
        }
        
        Console.Write("Введите количество посетителей: ");
        int customerCount;
        while (true)
        {
            var customerStr = Console.ReadLine();

            if (!int.TryParse(customerStr, out customerCount))
            {
                Console.Write("Введите количество посетителей в виде числа: ");
                continue;
            }

            if (customerCount <= 0)
            {
                Console.Write("Количесто посетителей должно быть больше 0: ");
                continue;
            }

            break;
        }

        var orderResult = BillCalculator.CalculateOrderPerCustomer(orderSum, customerCount);
        var tipsResult = BillCalculator.CalculateTipsPerCustomer(orderSum, customerCount);
        Console.WriteLine($"Счет для одного посетителя: {orderResult:F}");
        Console.WriteLine($"(учитывая чаевые): {(orderResult + tipsResult):F}");
    }
}