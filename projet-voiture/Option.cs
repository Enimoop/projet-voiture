public enum OptionType
{
    Journalier,
    Fixe
}

public class Option
{
    public string Name { get; }
    public OptionType Type { get; }
    public double Price { get; }

    public Option(string name, OptionType type, double price)
    {
        Name = name;
        Type = type;
        Price = price;
    }

    public double CalculerPrix(int duration)
    {
        return Type == OptionType.Journalier
            ? Price * duration
            : Price;
    }
}
