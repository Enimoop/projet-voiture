public class Promotion
{
    public string Name { get; }
    public double DiscountPercent { get; }

    public Promotion(string name, double discountPercent)
    {
        Name = name;
        DiscountPercent = discountPercent;
    }

    public double Appliquer(double amount)
    {
        return amount * (1 - DiscountPercent);
    }
}
