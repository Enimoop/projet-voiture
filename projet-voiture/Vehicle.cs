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
    public int BasicPrice { get; set; }
    public string Color { get; set; }
    public VehicleState State { get; set; }

    public int KilometrageTotal { get; set; }
    public int KilometrageActuel { get; set; }
    public DateTime? LastMaintenance { get; set; }

    public bool BesoinMaintenance()
    {
        if (LastMaintenance is not null)
        {
            return DateTime.Now - LastMaintenance > TimeSpan.FromDays(60);
        }
        return KilometrageActuel - KilometrageTotal > 10000;
    }


    public void AfficherDetails()
    {
        Console.WriteLine($"\n\n\n\n\n\nID: {Id}");
        Console.WriteLine($"Marque: {Brand}");
        Console.WriteLine($"Modèle: {Model}");
        Console.WriteLine($"Année: {YearModel}");
        Console.WriteLine($"Couleur: {Color}");
        Console.WriteLine($"Besoin de Maintenance: {BesoinMaintenance()}");
    }
}