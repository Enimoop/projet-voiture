using System.Diagnostics;
using System.Net;

public abstract class Client
{
    public int id { get; set; }
    public string name { get; set; }
    public string surname { get; set; }
    public List<Location> LocationsActives { get; } = new();
    public List<Location> HistoriqueLocations { get; } = new();
    public Client(int id, string name, string surname)
    {
        this.id = id;
        this.name = name;
        this.surname = surname;
    }
    public void AfficherInfo()
    {
        Console.WriteLine($"ID: {id}, Name: {name}, Surname: {surname}");
    }

    public bool PeutLouer()
    {
        return LocationsActives.Count < GetMaxLocationsSimultanees();
    }

    public int GetMaxLocationsSimultanees()
    {
        return 3;
    }

    public virtual double CalculerRemise(int dureeJours)
    {
        return dureeJours >= 7 ? 0.15 : 0.0;
    }

    public abstract double GetDepotGarantie();

}

public class Particulier : Client
{
    public Particulier(int id, string name, string surname) : base(id, name, surname) { }
   
    public override double GetDepotGarantie()
    {
        return 500.0;
    }
}

public class Premium : Client
{
    public Premium(int id, string name, string surname) : base(id, name, surname) { }
   
    public override double CalculerRemise(int dureeJours)
    {
        double baseRemise = base.CalculerRemise(dureeJours);
        return baseRemise + 0.10;
    }
    public override double GetDepotGarantie()
    {
        return 300.0;
    }
}
