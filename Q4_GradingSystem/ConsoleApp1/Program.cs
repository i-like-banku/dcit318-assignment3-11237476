using System;
using System.Collections.Generic;
using System.IO;

public class Student
{
    public int Id; 
    public string FullName; 
    public int Score;

    public Student(int id, string name, int score) 
    { 
        Id = id; 
        FullName = name; 
        Score = score; 
    }

    public string GetGrade() =>
        Score >= 80 ? "A" :
        Score >= 70 ? "B" :
        Score >= 60 ? "C" :
        Score >= 50 ? "D" : "F";
}

public class InvalidScoreFormatException : Exception 
{ 
    public InvalidScoreFormatException(string msg) : base(msg) { } 
}

public class MissingFieldException : Exception 
{ 
    public MissingFieldException(string msg) : base(msg) { } 
}

public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string path)
    {
        var list = new List<Student>();
        using var sr = new StreamReader(path);
        string line;
        int lineNumber = 0;

        while ((line = sr.ReadLine()) != null)
        {
            lineNumber++;
            var parts = line.Split(',');
            if (parts.Length != 3)
                throw new MissingFieldException($"Line {lineNumber}: Missing fields.");

            if (!int.TryParse(parts[0].Trim(), out int id))
                throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid student ID.");

            if (!int.TryParse(parts[2].Trim(), out int score))
                throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score.");

            list.Add(new Student(id, parts[1].Trim(), score));
        }
        return list;
    }

    public void WriteReportToFile(List<Student> students, string outputPath)
    {
        using var sw = new StreamWriter(outputPath);
        foreach (var s in students)
        {
            sw.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
        }
    }
}

class Program
{
    static void Main()
    {
        var processor = new StudentResultProcessor();

        Console.WriteLine("Do you want to:");
        Console.WriteLine("1. Read students from an existing input file");
        Console.WriteLine("2. Enter students manually and save to file first");
        Console.Write("Your choice (1 or 2): ");
        string choice = (Console.ReadLine() ?? "").Trim();

        string inputFilePath = "";
        if (choice == "1")
        {
            Console.Write("Enter input file path (e.g. C:\\Users\\YourName\\Documents\\students.txt): ");
            inputFilePath = Console.ReadLine() ?? "";
        }
        else if (choice == "2")
        {
            Console.Write("Enter file path where student data will be saved (e.g. C:\\Users\\YourName\\Documents\\students.txt): ");
            inputFilePath = Console.ReadLine() ?? "";

            Console.WriteLine("\nExample data format in file: ");
            Console.WriteLine("101, Alice Smith, 84");
            Console.WriteLine("102, Bob Adams, 73");
            Console.WriteLine("103, Charlie Chang, 59");
            Console.WriteLine("Type 'done' when finished entering students.\n");

            using var sw = new StreamWriter(inputFilePath);
            int lineNo = 1;
            while (true)
            {
                Console.Write($"Enter student {lineNo} data: ");
                string line = Console.ReadLine() ?? "";
                if (line.ToLower() == "done") break;
                sw.WriteLine(line);
                lineNo++;
            }
        }

        Console.Write("Enter output file path for the report (e.g. C:\\Users\\YourName\\Documents\\report.txt): ");
        string outputFilePath = Console.ReadLine() ?? "";

        try
        {
            var students = processor.ReadStudentsFromFile(inputFilePath);
            processor.WriteReportToFile(students, outputFilePath);
            Console.WriteLine($"\nReport generated successfully to: {outputFilePath}");
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("Input file not found.");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine("Invalid score format: " + ex.Message);
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine("Missing field error: " + ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
