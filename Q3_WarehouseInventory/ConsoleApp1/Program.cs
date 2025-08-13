using System;
using System.Collections.Generic;

public interface IInventoryItem { int Id { get; } string Name { get; } int Quantity { get; set; } }

public class ElectronicItem : IInventoryItem
{
    public int Id { get; } public string Name { get; }
    public int Quantity { get; set; } public string Brand { get; }
    public int WarrantyMonths { get; }
    public ElectronicItem(int id, string name, int qty, string brand, int warranty)
    { Id = id; Name = name; Quantity = qty; Brand = brand; WarrantyMonths = warranty; }
}
public class GroceryItem : IInventoryItem
{
    public int Id { get; } public string Name { get; }
    public int Quantity { get; set; } public DateTime ExpiryDate { get; }
    public GroceryItem(int id, string name, int qty, DateTime expiry)
    { Id = id; Name = name; Quantity = qty; ExpiryDate = expiry; }
}

public class DuplicateItemException : Exception { public DuplicateItemException(string msg) : base(msg) { } }
public class ItemNotFoundException : Exception { public ItemNotFoundException(string msg) : base(msg) { } }
public class InvalidQuantityException : Exception { public InvalidQuantityException(string msg) : base(msg) { } }

public class InventoryRepository<T> where T : IInventoryItem
{
    private readonly Dictionary<int, T> _items = new();
    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id)) throw new DuplicateItemException("Item ID already exists");
        _items[item.Id] = item;
    }
    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id)) throw new ItemNotFoundException("Item not found");
        return _items[id];
    }
    public void RemoveItem(int id)
    {
        if (!_items.Remove(id)) throw new ItemNotFoundException("Item not found");
    }
    public void UpdateQuantity(int id, int quantity)
    {
        if (quantity < 0) throw new InvalidQuantityException("Quantity cannot be negative");
        if (!_items.ContainsKey(id)) throw new ItemNotFoundException("Item not found");
        _items[id].Quantity = quantity;
    }
    public List<T> GetAllItems() => new(_items.Values);
}

public class WareHouseManager
{
    public InventoryRepository<ElectronicItem> electronics = new();
    public InventoryRepository<GroceryItem> groceries = new();

    private int electronicIdCounter = 1;
    private int groceryIdCounter = 1;

    public void Run()
    {
        Console.Write("Enter number of electronic items to add: ");
        if (!int.TryParse(Console.ReadLine(), out int elecCount) || elecCount < 0)
        {
            Console.WriteLine("Invalid number. Exiting.");
            return;
        }

        for (int i = 0; i < elecCount; i++)
        {
            Console.WriteLine($"Enter details for Electronic Item {i + 1}:");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            int quantity;
            while (true)
            {
                Console.Write("Quantity: ");
                if (int.TryParse(Console.ReadLine(), out quantity) && quantity >= 0) break;
                Console.WriteLine("Invalid quantity. Must be non-negative integer.");
            }

            Console.Write("Brand: ");
            string brand = Console.ReadLine();

            int warranty;
            while (true)
            {
                Console.Write("Warranty months: ");
                if (int.TryParse(Console.ReadLine(), out warranty) && warranty >= 0) break;
                Console.WriteLine("Invalid warranty period.");
            }

            try
            {
                electronics.AddItem(new ElectronicItem(electronicIdCounter++, name, quantity, brand, warranty));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        Console.Write("Enter number of grocery items to add: ");
        if (!int.TryParse(Console.ReadLine(), out int groceryCount) || groceryCount < 0)
        {
            Console.WriteLine("Invalid number. Exiting.");
            return;
        }

        for (int i = 0; i < groceryCount; i++)
        {
            Console.WriteLine($"Enter details for Grocery Item {i + 1}:");

            Console.Write("Name: ");
            string name = Console.ReadLine();

            int quantity;
            while (true)
            {
                Console.Write("Quantity: ");
                if (int.TryParse(Console.ReadLine(), out quantity) && quantity >= 0) break;
                Console.WriteLine("Invalid quantity. Must be non-negative integer.");
            }

            DateTime expiry;
            while (true)
            {
                Console.Write("Expiry date (YYYY-MM-DD): ");
                if (DateTime.TryParse(Console.ReadLine(), out expiry)) break;
                Console.WriteLine("Invalid date format.");
            }

            try
            {
                groceries.AddItem(new GroceryItem(groceryIdCounter++, name, quantity, expiry));
            }
            catch (DuplicateItemException ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        PrintAllItems(electronics);
        PrintAllItems(groceries);

        // Optional handling of testing error cases can be added here if desired
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        Console.WriteLine($"\nItems in {typeof(T).Name} repository:");
        foreach (var item in repo.GetAllItems())
            Console.WriteLine($"{item.Id}: {item.Name} - Qty: {item.Quantity}");
    }
}

class Program
{
    static void Main()
    {
        var manager = new WareHouseManager();
        manager.Run();
    }
}
