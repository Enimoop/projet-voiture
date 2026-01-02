using System;
using System.Linq;
using System.Collections.Generic;

public class Rental
{
    public int Id { get; set; }
    public Client Locataire { get; set; }
    public Vehicle Vehicle { get; set; }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public List<Option> Options { get; } = new();
    public Promotion? Promotion { get; set; }
    public Facture? Facture { get; private set; }

    public double Deposit { get; }
    public bool IsFinished { get; private set; }
    public bool IsCanceled { get; private set; }

    public int KmParcourus { get; private set; }
    public bool InspectionOK { get; private set; } = true;
    public double FraisDegats { get; private set; } = 0;

    public Rental(int id, Client locataire, Vehicle vehicle, DateTime startDate, DateTime endDate)
    {
        int duration = (endDate - startDate).Days;

        if (duration <= 0 || duration > 30)
            throw new Exception("Durée de location invalide (max 30 jours).");

        if (!locataire.PeutLouer())
            throw new Exception("Le client a atteint la limite de locations.");

        if (vehicle.State == Vehicle.VehicleState.Maintenance)
            throw new Exception("Le véhicule est en maintenance.");

        if (!vehicle.IsAvailable(startDate, endDate))
            throw new Exception("Le véhicule est déjà réservé sur cette période.");

        Id = id;
        Locataire = locataire;
        Vehicle = vehicle;
        StartDate = startDate;
        EndDate = endDate;

        Deposit = locataire.GetDepotGarantie();

        vehicle.Rentals.Add(this);

        if (StartDate.Date <= DateTime.Today && EndDate.Date > DateTime.Today)
            vehicle.State = Vehicle.VehicleState.Rented;

        locataire.LocationsActives.Add(this);
    }

    public void AfficherDetails()
    {
        Console.WriteLine($"\nLocation ID: {Id}");
        Console.WriteLine($"Locataire: {Locataire.name} {Locataire.surname}");
        Console.WriteLine($"Véhicule: {Vehicle.Brand} {Vehicle.Model}");
        Console.WriteLine($"Date de début: {StartDate:dd/MM/yyyy}");
        Console.WriteLine($"Date de fin: {EndDate:dd/MM/yyyy}");

        if (Options.Count > 0)
            Console.WriteLine("Options: " + string.Join(", ", Options.Select(o => o.Name)));
        else
            Console.WriteLine("Options: aucune");

        if (Promotion != null)
            Console.WriteLine($"Promotion: {Promotion.Name} (-{Promotion.DiscountPercent * 100:0}%)");
    }

    public double CalculerPrix()
    {
        if (IsCanceled) throw new Exception("Impossible de calculer le prix : location annulée.");

        int duration = (EndDate - StartDate).Days;

        double total = Vehicle.BasicPrice * duration;

        foreach (var opt in Options)
            total += opt.CalculerPrix(duration);

        double remise = Locataire.CalculerRemise(duration);
        total *= (1 - remise);

        if (Promotion != null)
            total = Promotion.Appliquer(total);

        return total;
    }

    public void TerminerLocation(int kmParcourus, bool inspectionOk, double fraisDegats)
    {
        if (IsCanceled) throw new Exception("La location est annulée.");
        if (IsFinished) throw new Exception("La location est déjà terminée.");
        if (kmParcourus < 0) throw new Exception("Kilométrage invalide.");
        if (fraisDegats < 0) throw new Exception("Frais dégâts invalides.");

        KmParcourus = kmParcourus;
        InspectionOK = inspectionOk;
        FraisDegats = inspectionOk ? 0 : fraisDegats;

        Vehicle.Mileage += kmParcourus;
        Vehicle.UpdateStateAfterReturn();

        double totalFinal = CalculerPrix() + FraisDegats;

        Facture = new Facture(Id, totalFinal, Deposit);

        Console.WriteLine("\n===== FACTURE =====");
        Console.WriteLine(Facture);
        Console.WriteLine($"Inspection: {(InspectionOK ? "OK" : "KO")}  |  Frais dégâts: {FraisDegats:0.00}€");
        Console.WriteLine($"Km parcourus: {KmParcourus}");
        Console.WriteLine($"Etat véhicule après retour: {Vehicle.State}");
        Console.WriteLine("===================\n");

        IsFinished = true;

        Locataire.LocationsActives.Remove(this);
        Locataire.HistoriqueLocations.Add(this);
    }

    public void Annuler()
    {
        if (IsFinished) throw new Exception("Déjà terminée.");
        if (IsCanceled) throw new Exception("Déjà annulée.");

        if (StartDate.Date <= DateTime.Today)
            throw new Exception("Impossible d'annuler : la location a déjà commencé.");

        IsCanceled = true;

        Locataire.LocationsActives.Remove(this);
        Locataire.HistoriqueLocations.Add(this);

        Vehicle.Rentals.Remove(this);

        Vehicle.State = Vehicle.VehicleState.Available;
    }

    public void ResilierMaintenant(int kmParcourus, bool inspectionOk, double fraisDegats)
    {
        if (IsFinished) throw new Exception("Déjà terminée.");
        if (IsCanceled) throw new Exception("Déjà annulée.");

        if (DateTime.Today < StartDate.Date)
            throw new Exception("La location n'a pas encore commencé (annule plutôt).");

        EndDate = DateTime.Today.AddDays(1);
        TerminerLocation(kmParcourus, inspectionOk, fraisDegats);
    }
}
