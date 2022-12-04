using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Nest;

public class MainClass
{
    //private static ElasticClient _client;
    public static void Main()
    {
        var num;
        do
        {
            Menu.Start();
            switch (num)
            {
                case 0: break;
                case 1: //Add file to index
                    {
                        Menu.FilePath();
                        //CSV.Path = @"..\..\..\posts.csv";
                        CSV.Path = Console.ReadLine();
                        List<Posts> ListFromFile = CSV.CsvLoad();
                        NEST.Index(ListFromFile);
                        break;
                    }
                case 2: //Search Text
                    {
                        Menu.Search();
                        Menu.Print(
                            NEST.GetPosts(
                                Console.ReadLine()
                                )
                            );
                        break;
                    }
            }
        }
        while (num!=0);
    }

}

public class Menu
{
    public static int Start()
    {
        Console.Clear();
        Console.WriteLine("Введите номер действия:");
        Console.WriteLine("1. Добавить файл в индекс");
        Console.WriteLine("2. Найти текст");
        //Console.WriteLine("3. Переиндексировать базу");
        //Console.WriteLine("4. Удалить запись из индекса");
        //Console.WriteLine("5. Tests");
        Console.WriteLine("0. Выход");
        return int.Parse(Console.ReadLine());
    }

    public static void FilePath()
    {
        Console.Clear();
        Console.WriteLine("Введите относительный или абсолютный путь к файлу:");
    }

    public static void Search()
    {
        Console.Clear();
        Console.WriteLine("Введите искомый текст");
    }

    public static void Out()
    {
        Console.WriteLine("Найденные результаты:");
    }

    public static void Delete()
    {
        Console.WriteLine("Запись удалена");
    }

    public static void Print(List<Posts> List)
    {
        Console.WriteLine("Найденные результаты:");
        foreach (var response in List)
        {
            Console.WriteLine(response.Text);
            Console.WriteLine("_______________________________");

        }
    }
}

public static class CSV
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

public static class NEST
{
    private static ElasticClient _client;
    public static ConnectionSettings ConnectionSettings
    {
        get
        {
            return new ConnectionSettings(new Uri("https://localhost:9200"))
                .CertificateFingerprint("9d4b8ef8c876db75242b403dbab3df1e9dee3fd98a192442785925db65059e1c")
                .BasicAuthentication("elastic", "--qo*kYItsydWmIVl*IG")
                .DefaultIndex("posts");
        }
    }
    static ElasticClient Client { get { return _client ?? (_client = new ElasticClient(ConnectionSettings)); } }
    public static void Index(List<Posts> posts)
    {
        var indexResponse = Client.IndexMany(posts);
    }

    public static List<Posts> GetPosts(string searchText)
    {
        var posts = new List<Posts>();
        var searchResponse = Client.Search<Posts>(s => s
            .From(0)
            .Size(20)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Text)
                    .Query(searchText)
                    )
                )
            );

        foreach (var response in searchResponse.Hits)
        {
            posts.Add(response.Source);
        }
        return posts;
    }
}