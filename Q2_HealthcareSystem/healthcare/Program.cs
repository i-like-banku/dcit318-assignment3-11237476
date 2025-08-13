using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private readonly List<T> items = new();
    public void Add(T item) => items.Add(item);
    public List<T> GetAll() => new(items);
    public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
    public bool Remove(Func<T, bool> predicate)
    {
        var item = items.FirstOrDefault(predicate);
        if (item == null) return false;
        items.Remove(item);
        return true;
    }
}

public class Patient
{
    public int Id;
    public string Name;
    public int Age;
    public string Gender;
    public Patient(int id, string name, int age, string gender)
    {
        Id = id; Name = name; Age = age; Gender = gender;
    }
}

public class Prescription
{
    public int Id;
    public int PatientId;
    public string MedicationName;
    public DateTime DateIssued;
    public Prescription(int id, int patientId, string med, DateTime date)
    {
        Id = id; PatientId = patientId; MedicationName = med; DateIssued = date;
    }
}

public class HealthSystemApp
{
    private readonly Repository<Patient> _patientRepo = new();
    private readonly Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _map = new();
    private int patientIdCounter = 1;
    private int prescriptionIdCounter = 1;

    public void Run()
    {
        Console.Write("Enter number of patients: ");
        if (!int.TryParse(Console.ReadLine(), out int numPatients) || numPatients <= 0)
        {
            Console.WriteLine("Invalid number. Exiting.");
            return;
        }

        // 🔹 Ask if user wants to continue
        Console.Write($"You entered {numPatients} patient(s). Do you want to continue? (Y/N): ");
        string confirmPatients = (Console.ReadLine() ?? "").Trim().ToUpper();
        if (confirmPatients != "Y")
        {
            Console.WriteLine("Operation cancelled. Exiting...");
            return;
        }

        for (int i = 0; i < numPatients; i++)
        {
            Console.WriteLine($"\nEnter details for Patient {i + 1}:");
            Console.Write("Name: ");
            string name = Console.ReadLine() ?? string.Empty;

            int age;
            while (true)
            {
                Console.Write("Age: ");
                if (int.TryParse(Console.ReadLine(), out age)) break;
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }

            Console.Write("Gender: ");
            string gender = Console.ReadLine() ?? string.Empty;

            _patientRepo.Add(new Patient(patientIdCounter++, name, age, gender));
        }

        Console.Write("\nEnter number of prescriptions: ");
        if (!int.TryParse(Console.ReadLine(), out int numPrescriptions) || numPrescriptions < 0)
        {
            Console.WriteLine("Invalid number. Exiting.");
            return;
        }

        // 🔹 Optional: confirm for prescriptions too
        Console.Write($"You entered {numPrescriptions} prescription(s). Do you want to continue? (Y/N): ");
        string confirmPrescriptions = (Console.ReadLine() ?? "").Trim().ToUpper();
        if (confirmPrescriptions != "Y")
        {
            Console.WriteLine("Prescription entry cancelled.");
            numPrescriptions = 0; // skip adding prescriptions
        }

        for (int i = 0; i < numPrescriptions; i++)
        {
            Console.WriteLine($"\nEnter details for Prescription {i + 1}:");

            int patientId;
            while (true)
            {
                Console.Write("Patient ID: ");
                if (int.TryParse(Console.ReadLine(), out patientId) &&
                    _patientRepo.GetById(p => p.Id == patientId) != null)
                    break;
                Console.WriteLine("Patient ID not found. Please enter a valid existing Patient ID.");
            }

            Console.Write("Medication Name: ");
            string medName = Console.ReadLine() ?? string.Empty;

            DateTime dateIssued;
            while (true)
            {
                Console.Write("Date Issued (YYYY-MM-DD): ");
                if (DateTime.TryParse(Console.ReadLine(), out dateIssued)) break;
                Console.WriteLine("Invalid date format.");
            }

            _prescriptionRepo.Add(new Prescription(prescriptionIdCounter++, patientId, medName, dateIssued));
        }

        // Build map and display data
        BuildPrescriptionMap();
        PrintAllPatients();

        Console.Write("\nEnter a Patient ID to display their prescriptions: ");
        if (int.TryParse(Console.ReadLine(), out int pid))
        {
            PrintPrescriptionsForPatient(pid);
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }

    public void BuildPrescriptionMap()
    {
        _map = _prescriptionRepo.GetAll().GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("\nPatients:");
        foreach (var p in _patientRepo.GetAll())
            Console.WriteLine($"{p.Id}: {p.Name}, Age: {p.Age}, Gender: {p.Gender}");
    }

    public void PrintPrescriptionsForPatient(int id)
    {
        var patient = _patientRepo.GetById(p => p.Id == id);
        if (patient != null && _map.TryGetValue(id, out var list))
        {
            Console.WriteLine($"\nPrescriptions for {patient.Name} (ID {id}):");
            foreach (var pres in list)
                Console.WriteLine($"{pres.MedicationName} issued on {pres.DateIssued:yyyy-MM-dd}");
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }
}

class Program
{
    static void Main()
    {
        var app = new HealthSystemApp();
        app.Run();
    }
}
