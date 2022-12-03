using System;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Nest;

public class MainClass
{
    private static ElasticClient _client;
    public static void Main()
    {
        Menu.Start();
        CSV.Path = @"..\..\..\posts.csv";
        
        var post = new List<Posts>();
        post = CSV.CsvLoad();

        var settings = new ConnectionSettings(new Uri("https://localhost:9200"))
            .CertificateFingerprint("9d4b8ef8c876db75242b403dbab3df1e9dee3fd98a192442785925db65059e1c")
            .BasicAuthentication("elastic", "--qo*kYItsydWmIVl*IG")
            .DefaultIndex("posts");

        _client = new ElasticClient(settings);

        var indexResponse = _client.IndexMany(post);

        var searchResponse = _client.Search<Posts>(s => s
            .From(0)
            .Size(10)
            .Query(q => q
                .Match(m => m
                    .Field(f => f.Text)
                    .Query("Слив")
                    )
                )
            );
        foreach(var response in searchResponse.Hits)
        {
            Console.WriteLine(response.Source.Text);
        }

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