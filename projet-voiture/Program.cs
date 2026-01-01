using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

        List<Vehicle> fleet = new List<Vehicle>
        {
            new Vehicle { Id = 1, Brand = "Ford", Model = "Ranger", YearModel = 2025, Color = "Rouge", BasicPrice = 120, State = Vehicle.VehicleState.Available },
            new Vehicle { Id = 2, Brand = "Peugeot", Model = "208", YearModel = 2022, Color = "Blanc", BasicPrice = 90, State = Vehicle.VehicleState.Available },
            new Vehicle { Id = 3, Brand = "Tesla", Model = "Model 3", YearModel = 2023, Color = "Noir", BasicPrice = 150, State = Vehicle.VehicleState.Maintenance },
        };

        Client? currentClient = null;
        List<Rental> rentals = new List<Rental>();
        int nextRentalId = 1;

        while (true)
        {
            DateTime today = DateTime.Today;
            Console.WriteLine("=== Date actuelle : " + today.ToString("dd/MM/yyyy") + " ===");
            Console.WriteLine("=== LOCATION DE VEHICULES ===");
            Console.WriteLine("\n1) S'enregistrer / Se connecter (client)");
            Console.WriteLine("2) Lister les véhicules");
            Console.WriteLine("3) Créer une location");
            Console.WriteLine("4) Mes locations actives");
            Console.WriteLine("5) Terminer une location et afficher sa facture");
            Console.WriteLine("6) Annuler une location (future)");
            Console.WriteLine("7) Résilier une location (en cours)");
            Console.WriteLine("8) Historique de mes locations");
            Console.WriteLine("0) Quitter");
            Console.Write("Choix : ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    currentClient = RegisterClient();
                    break;

                case "2":
                    DisplayVehicles(fleet);
                    break;

                case "3":
                    if (currentClient == null)
                    {
                        Console.WriteLine("Vous devez d'abord vous enregistrer (menu 1).");
                        break;
                    }

                    CreateRental(currentClient, fleet, rentals, ref nextRentalId);
                    break;

                case "4":
                    if (currentClient == null)
                    {
                        Console.WriteLine("Vous devez d'abord vous enregistrer (menu 1).");
                        break;
                    }

                    DisplayActiveRentals(currentClient);
                    break;

                case "5":
                    if (currentClient == null)
                    {
                        Console.WriteLine("Vous devez d'abord vous enregistrer (menu 1).");
                        break;
                    }

                    FinishRental(currentClient);
                    break;
                case "6":
                    if (currentClient == null)
                    {
                        Console.WriteLine("Vous devez d'abord vous enregistrer (menu 1).");
                        break;
                    }
                    CancelFutureRental(currentClient);
                    break;

                case "7":
                    if (currentClient == null)
                    {
                        Console.WriteLine("Vous devez d'abord vous enregistrer (menu 1).");
                        break;
                    }
                    EarlyEndRental(currentClient);
                    break;
                case "8":
                if (currentClient == null)
                {
                    Console.WriteLine("Vous devez d'abord vous enregistrer (menu 1).");
                    break;
                }
                DisplayRentalHistory(currentClient);
                break;


                case "0":
                    Console.WriteLine("Au revoir !");
                    return;

                default:
                    Console.WriteLine("Choix invalide.");
                    break;
            }
        }
    }

    static Client RegisterClient()
    {
        Console.WriteLine("\n--- Enregistrement client ---");
        int id = ReadInt("Id client : ");
        Console.Write("Nom : ");
        string name = Console.ReadLine() ?? "";
        Console.Write("Prénom : ");
        string surname = Console.ReadLine() ?? "";

        Console.WriteLine("Type : 1) Particulier  2) Premium");
        Console.Write("Choix : ");
        string? type = Console.ReadLine();

        Client client = type == "2"
            ? new Premium(id, name, surname)
            : new Particulier(id, name, surname);

        Console.WriteLine("\nClient connecté :");
        client.AfficherInfo();
        Console.WriteLine($"Dépôt de garantie : {client.GetDepotGarantie():0.00}€");
        return client;
    }

    static void DisplayVehicles(List<Vehicle> fleet)
    {
        Console.WriteLine("\n--- Véhicules ---");
        foreach (var v in fleet)
        {
            Console.WriteLine($"[{v.Id}] {v.Brand} {v.Model} ({v.YearModel}) - {v.Color} - Etat: {v.State}");
        }
    }

    static void CreateRental(Client client, List<Vehicle> fleet, List<Rental> rentals, ref int nextRentalId)
    {
        Console.WriteLine("\n--- Création location ---");
        DisplayVehicles(fleet);

        int vehicleId = ReadInt("Id du véhicule : ");
        Vehicle? selected = fleet.FirstOrDefault(v => v.Id == vehicleId);

        if (selected == null)
        {
            Console.WriteLine("Véhicule introuvable.");
            return;
        }

        DateTime start = ReadDate("Date début (ex: 16/12/2025) : ");
        DateTime end = ReadDate("Date fin (ex: 20/12/2025) : ");

        bool gps = ReadYesNo("GPS (5€/jour) ? (o/n) : ");
        bool childSeat = ReadYesNo("Siège enfant (3€/jour) ? (o/n) : ");
        bool extraInsurance = ReadYesNo("Assurance supplémentaire (50€ fixe) ? (o/n) : ");

        bool usePromo = ReadYesNo("Appliquer une promotion ? (o/n) : ");
        Promotion? promo = null;
        if (usePromo)
        {
            Console.WriteLine("Promos disponibles :");
            Console.WriteLine("1) NOEL10 (-10%)");
            int promoChoice = ReadInt("Choix promo (1/2) : ");

            promo = promoChoice == 1
                ? new Promotion("NOEL10", 0.10)
                : new Promotion("WELCOME5", 0.05);
        }

        try
        {
            Rental rental = new Rental(nextRentalId, client, selected, start, end);

            if (gps) rental.Options.Add(new Option("GPS", OptionType.Journalier, 5));
            if (childSeat) rental.Options.Add(new Option("Siège enfant", OptionType.Journalier, 3));
            if (extraInsurance) rental.Options.Add(new Option("Assurance+", OptionType.Fixe, 50));

            rental.Promotion = promo;

            double price = rental.CalculerPrix();

            Console.WriteLine("\n--- Récapitulatif ---");
            rental.AfficherDetails();
            Console.WriteLine($"Dépôt de garantie à verser : {rental.Deposit:0.00}€");
            Console.WriteLine($"Prix total : {price:0.00}€");

            bool confirm = ReadYesNo("Valider et créer la location ? (o/n) : ");
            if (!confirm)
            {
                selected.State = Vehicle.VehicleState.Available;
                rental.Vehicle.Rentals.Remove(rental);
                client.LocationsActives.Remove(rental);

                Console.WriteLine("Location annulée.");
                return;
            }

            rentals.Add(rental);
            nextRentalId++;

            Console.WriteLine("✅ Location créée !");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Impossible de créer la location : {ex.Message}");
        }
    }
    static void DisplayActiveRentals(Client client)
    {
        Console.WriteLine("\n--- Locations actives ---");
        if (client.LocationsActives.Count == 0)
        {
            Console.WriteLine("Aucune location en cours.");
            return;
        }

        foreach (var r in client.LocationsActives)
        {
            Console.WriteLine($"Location #{r.Id} - {r.Vehicle.Brand} {r.Vehicle.Model} - {r.StartDate:dd/MM/yyyy} -> {r.EndDate:dd/MM/yyyy}");
        }
    }

   static void FinishRental(Client client)
{
    Console.WriteLine("\n--- Terminer une location ---");
    DisplayActiveRentals(client);

    if (client.LocationsActives.Count == 0)
        return;

    int rentalId = ReadInt("Id de la location à terminer : ");
    Rental? rental = client.LocationsActives.FirstOrDefault(r => r.Id == rentalId);

    if (rental == null)
    {
        Console.WriteLine("Location introuvable.");
        return;
    }

    int km = ReadInt("Km parcourus : ");
    bool ok = ReadYesNo("Inspection OK ? (o/n) : ");
    double frais = 0;

    if (!ok)
    {
        Console.Write("Frais de dégâts (€) : ");
        while (!double.TryParse(Console.ReadLine(), out frais) || frais < 0)
            Console.Write("Entrée invalide. Recommence : ");
    }

    rental.TerminerLocation(km, ok, frais);
}

    static void CancelFutureRental(Client client)
    {
        Console.WriteLine("\n--- Annuler une location future ---");

        var actives = client.LocationsActives
            .Where(r => !r.IsFinished && !r.IsCanceled)
            .ToList();

        if (actives.Count == 0)
        {
            Console.WriteLine("Aucune location active.");
            return;
        }

        var futures = actives.Where(r => r.StartDate.Date > DateTime.Today).ToList();
        if (futures.Count == 0)
        {
            Console.WriteLine("Aucune location future à annuler.");
            Console.WriteLine("(Les locations qui ont déjà commencé ne peuvent pas être annulées)");
            return;
        }

        foreach (var r in futures)
        {
            Console.WriteLine($"Location #{r.Id} - {r.Vehicle.Brand} {r.Vehicle.Model} - {r.StartDate:dd/MM/yyyy} -> {r.EndDate:dd/MM/yyyy}");
        }

        int rentalId = ReadInt("Id de la location à annuler : ");
        Rental? rental = futures.FirstOrDefault(r => r.Id == rentalId);

        if (rental == null)
        {
            Console.WriteLine("Location introuvable.");
            return;
        }

        try
        {
            rental.Annuler();
            Console.WriteLine("Location future annulée.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Impossible d'annuler : {ex.Message}");
        }
    }

    static void EarlyEndRental(Client client)
    {
        Console.WriteLine("\n--- Résilier une location (en cours) ---");

        var actives = client.LocationsActives
            .Where(r => !r.IsFinished && !r.IsCanceled)
            .ToList();

        if (actives.Count == 0)
        {
            Console.WriteLine("Aucune location active.");
            return;
        }

        var inProgress = actives.Where(r => r.StartDate.Date <= DateTime.Today).ToList();
        if (inProgress.Count == 0)
        {
            Console.WriteLine("Aucune location en cours à résilier.");
            Console.WriteLine("(Les locations futures doivent être annulées, pas résiliées)");
            return;
        }

        foreach (var r in inProgress)
        {
            Console.WriteLine($"Location #{r.Id} - {r.Vehicle.Brand} {r.Vehicle.Model} - {r.StartDate:dd/MM/yyyy} -> {r.EndDate:dd/MM/yyyy}");
        }

        int rentalId = ReadInt("Id de la location à résilier maintenant : ");
        Rental? rental = inProgress.FirstOrDefault(r => r.Id == rentalId);

        if (rental == null)
        {
            Console.WriteLine("Location introuvable.");
            return;
        }

        try
        {
            int km = ReadInt("Km parcourus : ");
            bool ok = ReadYesNo("Inspection OK ? (o/n) : ");
            double frais = 0;

            if (!ok)
            {
                Console.Write("Frais de dégâts (€) : ");
                while (!double.TryParse(Console.ReadLine(), out frais) || frais < 0)
                    Console.Write("Entrée invalide. Recommence : ");
            }

            rental.ResilierMaintenant(km, ok, frais);
            Console.WriteLine("Location résiliée (retour anticipé).");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Impossible de résilier : {ex.Message}");
        }

    }

    static void DisplayRentalHistory(Client client)
    {
        Console.WriteLine("\n--- Historique des locations ---");

        if (client.HistoriqueLocations.Count == 0)
        {
            Console.WriteLine("Aucune location dans l'historique.");
            return;
        }

        foreach (var r in client.HistoriqueLocations)
        {
            Console.WriteLine($"Location #{r.Id}");
            Console.WriteLine($"Véhicule : {r.Vehicle.Brand} {r.Vehicle.Model}");

            Console.WriteLine($"Période : {r.StartDate:dd/MM/yyyy} -> {r.EndDate:dd/MM/yyyy}");
            Console.WriteLine($"Terminée : {(r.IsFinished ? "Oui" : "Non")}");
            Console.WriteLine($"Annulée : {(r.IsCanceled ? "Oui" : "Non")}");

            if (r.Facture != null)
            {
                Console.WriteLine($"Montant : {r.Facture.Total:0.00}€");
                Console.WriteLine($"Date facture : {r.Facture.Date:dd/MM/yyyy}");
            }

            Console.WriteLine("-----------------------------");
        }
    }



    static int ReadInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();
            if (int.TryParse(s, out int value))
                return value;
            Console.WriteLine("Entrée invalide. Recommence.");
        }
    }

    static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine();

            if (DateTime.TryParse(s, out DateTime dt))
                return dt;

            Console.WriteLine("Date invalide. Format attendu : jj/mm/aaaa (ex: 16/12/2025).");
        }
    }

    static bool ReadYesNo(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? s = Console.ReadLine()?.Trim().ToLower();

            if (s == "o" || s == "oui" || s == "y" || s == "yes") return true;
            if (s == "n" || s == "non" || s == "no") return false;

            Console.WriteLine("Réponds par o/n.");
        }
    }

}