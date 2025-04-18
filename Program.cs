// See https://aka.ms/new-console-template for more information

public class Program
{
    public static void Main()
    {
        List<TradeDetails> batchTrades = new();

        Console.Write("Load trades from CSV file? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            Console.Write("Enter path to CSV file: ");
            var csvPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(csvPath))
            {
                Console.WriteLine("Invalid file path. Exiting.");
                Environment.Exit(1); // Exit with error code
            }
            batchTrades = TradeCsvImporter.Import(csvPath);
        }
        else
        {
            Console.WriteLine("Existing from Application...");
            Environment.Exit(0);
        }

        var allResults = batchTrades.Select(t => new RMSCalcultor(t).GetResults()).ToList();

        var summary = new Dictionary<string, decimal>();
        foreach (var result in allResults)
        {
            foreach (var kvp in result)
            {
                if (!summary.ContainsKey(kvp.Key)) summary[kvp.Key] = 0;
                summary[kvp.Key] += kvp.Value;
            }
        }

        Console.WriteLine("\n=== Batch Summary ===");
        foreach (var kvp in summary)
        {
            Console.WriteLine($"{kvp.Key}: ₹{kvp.Value:N2}");
        }

    }
}
