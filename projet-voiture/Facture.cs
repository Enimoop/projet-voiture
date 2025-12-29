public class Facture
{
    public int RentalId { get; }
    public DateTime Date { get; }
    public double Total { get; }
    public double Deposit { get; }

    public Facture(int rentalId, double total, double deposit)
    {
        RentalId = rentalId;
        Total = total;
        Deposit = deposit;
        Date = DateTime.Now;
    }

    public override string ToString()
    {
        return
            $"FACTURE\n" +
            $"Location #{RentalId}\n" +
            $"Date : {Date:dd/MM/yyyy}\n" +
            $"Total : {Total:0.00} €\n" +
            $"Dépôt de garantie : {Deposit:0.00} €";
    }
}
