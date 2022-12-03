using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Nest;

public class MainClass
{
    public static void Main()
    {
        Menu.Start();
        CSV.Path = @"..\..\..\posts.csv";
        
        var post = new List<Posts>();
        post = CSV.CsvLoad();


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
    public static string Path { get; set; }
    public static List<Posts> CsvLoad()
    {
        var records = new List<Posts>();
        using (var reader = new StreamReader(Path)) 
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            records = csv.GetRecords<Posts>().ToList();
        }
        return records;
    }
}

public class Posts
{
    [Name("text")]
    public string? Text { get; set; }
    [Name("created_date")]
    public DateTime? Created_date { get; set; }
    [Name("rubrics")]
    public string? Rubrics { get; set; }
}