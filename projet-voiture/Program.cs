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