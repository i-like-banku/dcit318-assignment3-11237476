using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public interface IInventoryEntity { int Id { get; } }

public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

public class InventoryLogger<T> where T : IInventoryEntity
{
    private readonly List<T> _log = new();
    private readonly string _filePath;

    public InventoryLogger(string filePath) { _filePath = filePath; }

    public void Add(T item) => _log.Add(item);

    public List<T> GetAll() => new(_log);

    public void SaveToFile()
    {
        try
        {
            File.WriteAllText(_filePath, JsonSerializer.Serialize(_log));
        }
        catch (Exception e) { Console.WriteLine("Save error: " + e.Message); }
    }

    public void LoadFromFile()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var data = JsonSerializer.Deserialize<List<T>>(json);
                _log.Clear();
                if (data != null) _log.AddRange(data);
            }
        }
        catch (Exception e) { Console.WriteLine("Load error: " + e.Message); }
    }
}

public class InventoryApp
{
    public InventoryLogger<InventoryItem> logger;
    private int idCounter = 1;

    public InventoryApp(string path) { logger = new InventoryLogger<InventoryItem>(path); }

    public void Run()
    {
        Console.Write("How many inventory items do you want to enter? (e.g. 3): ");
        if (!int.TryParse(Console.ReadLine(), out int count) || count < 0)
        {
            Console.WriteLine("Invalid number.");
            return;
        }

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine($"\nEnter details for Item {i + 1}:");

            Console.Write("Name (e.g. Laptop, Phone, Printer): ");
            string name = Console.ReadLine() ?? string.Empty;

            int qty;
            while (true)
            {
                Console.Write("Quantity (non-negative integer, e.g. 5): ");
                if (int.TryParse(Console.ReadLine(), out qty) && qty >= 0) break;
                Console.WriteLine("Invalid quantity, enter non-negative integer.");
            }

            var item = new InventoryItem(idCounter++, name, qty, DateTime.Now);
            logger.Add(item);
        }

        logger.SaveToFile();
        Console.WriteLine("\nData saved to file.");

        Console.WriteLine("\nClearing current data from memory and reloading from file...");
        logger.LoadFromFile();

        PrintAllItems();
    }

    public void PrintAllItems()
    {
        Console.WriteLine("\nInventory Items:");
        foreach (var item in logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

class Program
{
    static void Main()
    {
        Console.Write("Enter file path to save/load inventory data (e.g. C:\\Users\\YourName\\Documents\\inventory.json): ");
        string filePath = Console.ReadLine() ?? string.Empty;

        var app = new InventoryApp(filePath);
        app.Run();
    }
}
