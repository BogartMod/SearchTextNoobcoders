using System;
using CsvHelper;

public class MainClass
{
    public static void Main()
    {
        Menu.Start();


        Menu.Find();

        Menu.Out();
    }
}

public class Menu
{
    public static void Start()
    {
        Console.WriteLine("Введите имя файла:");
    }
    public static void Find()
    {
        Console.WriteLine("Введите искомый текст");
    }

    public static void Out()
    {
        Console.WriteLine("Найденные результаты:");
    }
}

public class CSV
{
    public string Path { get; set; }
    static bool CsvLoad(Path)
    {
        using (var reader = new StreamReader(Path)) 
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<>();
        }
    }
}

public class Listik
{
    public 
}