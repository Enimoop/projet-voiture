public class Rental
{
    public int Id { get; set; }
    public Client Locataire { get; set; }
    public Vehicle Vehicle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool GPS { get; set; }
    public bool ChildSeat { get; set; }
    public bool ExtraInsurance { get; set; }

    public double Deposit { get; }
    public bool IsFinished { get; private set; }

    public Rental(int id, Client locataire, Vehicle vehicle, DateTime startDate, DateTime endDate)
    {
        int duration = (endDate - startDate).Days;

        if (duration <= 0 || duration > 30)
            throw new Exception("Durée de location invalide (max 30 jours).");

        if (!locataire.PeutLouer())
            throw new Exception("Le client a atteint la limite de locations.");

        if (vehicle.State != Vehicle.VehicleState.Available)
            throw new Exception("Le véhicule n'est pas disponible.");

        this.Id = id;
        this.Locataire = locataire;
        this.Vehicle = vehicle;
        this.StartDate = startDate;
        this.EndDate = endDate;

         Deposit = locataire.GetDepotGarantie();

        vehicle.State = Vehicle.VehicleState.Rented;
        locataire.LocationsActives.Add(this);
    }

    public void AfficherDetails()
    {
        Console.WriteLine($"\nLocation ID: {Id}");
        Console.WriteLine($"Locataire: {Locataire.name} {Locataire.surname}");
        Console.WriteLine($"Véhicule: {Vehicle.Brand} {Vehicle.Model}");
        Console.WriteLine($"Date de début: {StartDate.ToShortDateString()}");
        Console.WriteLine($"Date de fin: {EndDate.ToShortDateString()}");
    } 

    public double CalculerPrix()
    {
        int duration = (EndDate - StartDate).Days;

        double total = 100 * duration;

        if (GPS) total += 5 * duration;
        if (ChildSeat) total += 3 * duration;
        if (ExtraInsurance) total += 50;

        double remise = Locataire.CalculerRemise(duration);
        total *= 1 - remise;

        return total;
    }

    public void TerminerLocation()
    {
        if (IsFinished)
            throw new Exception("La location est déjà terminée.");

        IsFinished = true;
        Vehicle.State = Vehicle.VehicleState.Available;
        Locataire.LocationsActives.Remove(this);
        Locataire.HistoriqueLocations.Add(this);
    }

    public string GenererFacture()
    {
        return $"FACTURE Location#{this.Id} - Client {this.Locataire.name} {this.Locataire.surname} - Total: {this.CalculerPrix():0.00}€ - Dépôt: {this.Deposit:0.00}€";
    }


}