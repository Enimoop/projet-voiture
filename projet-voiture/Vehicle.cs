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
    public List<Rental> Rentals { get; } = new();

    public void AfficherDetails()
    {
        Console.WriteLine($"\n\n\n\n\n\nID: {Id}");
        Console.WriteLine($"Marque: {Brand}");
        Console.WriteLine($"Modèle: {Model}");
        Console.WriteLine($"Année: {YearModel}");
        Console.WriteLine($"Couleur: {Color}");
    }

    public bool IsAvailable(DateTime start, DateTime end)
{
    foreach (var r in Rentals)
    {
        bool overlap = start < r.EndDate && end > r.StartDate;
        if (overlap) return false;
    }
    return true;
}

}