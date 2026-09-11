namespace PizzaSplit.Core;

public static class BillCalculator
{
    public static double CalculateOrderPerCustomer(double orderSum, int customersCount)
    {
        return orderSum / customersCount;
    }

    public static double CalculateTipsPerCustomer(double orderSum, int customersCount, double tipsPercent = 10.0)
    {
        var tipsSum = orderSum * tipsPercent / 100;
        return CalculateOrderPerCustomer(tipsSum, customersCount);
    }
}