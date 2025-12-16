public class Vehicle
{
    public enum VehicleState
    {
        Available,
        Rented,
        Maintenance,
    }
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int YearModel { get; set; }
    public string Color { get; set; }
    public VehicleState State { get; set; }

    public void AfficherDetails()
    {
        Console.WriteLine($"\n\n\n\n\n\nID: {Id}");
        Console.WriteLine($"Marque: {Brand}");
        Console.WriteLine($"Modèle: {Model}");
        Console.WriteLine($"Année: {YearModel}");
        Console.WriteLine($"Couleur: {Color}");
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Vehicle fordRanger = new Vehicle
        {
            Id = 1,
            Brand = "Ford",
            Model = "Ranger",
            YearModel = 2025,
            Color = "Rouge"
        };

        fordRanger.AfficherDetails();
    }
}