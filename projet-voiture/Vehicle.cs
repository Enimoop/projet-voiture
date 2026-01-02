using System;
using System.Collections.Generic;

public class Vehicle
{
    public enum VehicleState
    {
        Available,
        Rented,
        Maintenance,
    }

    public int Id { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public int YearModel { get; set; }
    public double BasicPrice { get; set; }
    public string Color { get; set; } = "";
    public VehicleState State { get; set; }

    public List<Rental> Rentals { get; } = new();

    public int Mileage { get; set; }
    public int MileageAtLastMaintenance { get; set; }
    public DateTime LastMaintenance { get; set; } = DateTime.Today;

    public bool BesoinMaintenance()
    {
        bool km = (Mileage - MileageAtLastMaintenance) >= 10000;
        bool time = LastMaintenance.AddMonths(6) <= DateTime.Today;
        return km || time;
    }

    public void UpdateStateAfterReturn()
    {
        State = BesoinMaintenance() ? VehicleState.Maintenance : VehicleState.Available;
    }

    public bool IsAvailable(DateTime start, DateTime end)
    {
        foreach (var r in Rentals)
        {
            if (r.IsCanceled || r.IsFinished) continue;

            bool overlap = start < r.EndDate && end > r.StartDate;
            if (overlap) return false;
        }
        return true;
    }

    public void AfficherDetails()
    {
        Console.WriteLine($"ID: {Id}");
        Console.WriteLine($"Marque: {Brand}");
        Console.WriteLine($"Modèle: {Model}");
        Console.WriteLine($"Année: {YearModel}");
        Console.WriteLine($"Couleur: {Color}");
        Console.WriteLine($"Prix/jour: {BasicPrice:0.00}€");
        Console.WriteLine($"Etat: {State}");
        Console.WriteLine($"Km total: {Mileage}");
        Console.WriteLine($"Besoin maintenance: {BesoinMaintenance()}");
    }
}
