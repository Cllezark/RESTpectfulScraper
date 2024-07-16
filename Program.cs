using System;
using CsvHelper;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using CsvHelper.Configuration;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();

Console.WriteLine("Starting scraper");
// Set file path of list.csv

string filePath = @"C:\Users\clifford.lezark\Documents\code\RESTpectfulScraper\input\tcg-legal-list-pre-info-07-09-2024.csv";

Console.WriteLine($"Loading data from: {filePath}");

// set the request endpoint you're going to scrape
string apiEndpoint = @"https://db.ygoprodeck.com/api/v7/cardinfo.php?name=";
string apiEndpointCap = "&misc=yes";

List<CardData> entries = new List<CardData>();

//open a csvhelper StreamReader to ingest the file
using (var reader = new StreamReader(filePath))
using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
{
   // turn the csv into a list of objects you can interact with 
    var records = csv.GetRecords<Card>();
    int errorCount = 0;
    List<Card> set = records.ToList();
    Console.WriteLine("Data loaded!");

    foreach (var record in set)
    {
        Console.WriteLine($"Password: {record.id}, Name: {record.name}");
        Console.WriteLine($"Size: {set.Count()}");
        try
        {
            //// 1) call the api and request the data associated with the given object's primary key
            ///
            using var httpClient = new HttpClient();
            Console.WriteLine($"{apiEndpoint}{record.name}{apiEndpointCap}");
            var response = await httpClient.GetAsync($"{apiEndpoint}{record.name}{apiEndpointCap}");
            CardData entry = new CardData();
            Root root = new Root();
            if (response.IsSuccessStatusCode)
            {
                var stringData = await response.Content.ReadAsStringAsync();
                Console.WriteLine(stringData);
        
                var card = JsonConvert.DeserializeObject<Root>(stringData);
                entry = card.data[0];
                //entry = JsonSerializer.Deserialize<CardData>(stringData, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});
                Console.WriteLine(entry.name + " + " + entry.type);
                Console.WriteLine("Adding entry for " + entry.name);
            entries.Add(entry);

            }

            //// 2) stash the response in a typed list matching the response 


            //// 3) wait a fraction of a second before firing off the next request
            await Task.Delay(75);

        }
        //// 4) if the call fails, write something to that effect in place of the response
        catch
        {
            errorCount++;
            Console.WriteLine($"Error #{errorCount}");
        }


    }

    Console.WriteLine("Finished scraping.");

}


List<YGOTriviaCard> ygoTriviaCards = new List<YGOTriviaCard>();

// populate that list with transformations of the data from the product of the last operation
foreach(var entry in entries)
{

    // in order to access the fields of an object stored within another object
    // initialize the outermost object
    // access the inner object as vars
    // then access the inner object fields normally.

    var ygoTriviaCard = new YGOTriviaCard();
    var ygoTriviaArt = entry.card_images.FirstOrDefault();
    var ygoTriviaDates = entry.misc_info.FirstOrDefault();

    // setting values.
    if(entry.id is not null)
    {
        ygoTriviaCard.id = (int)entry.id;
    }
    else
    {
        ygoTriviaCard.id = 918918918;
    }
    if (entry.name is not null)
    {
        ygoTriviaCard.Name = entry.name;

    }
    else
    {
        ygoTriviaCard.Name = "ERROR-UNKNOWN";
    }
    if (entry.type is not null)
    {
        ygoTriviaCard.Type = entry.type;

    }
    else
    {
        ygoTriviaCard.Type = "ERROR-UNKNOWN";
    }
    if (ygoTriviaArt is not null)
    {
        ygoTriviaCard.Artwork = ygoTriviaArt.image_url_cropped;

    }
    else
    {
        ygoTriviaArt.image_url_cropped = "ERROR-UNKNOWN";
    }
    if (ygoTriviaDates.tcg_date is not null)
    {
        ygoTriviaCard.TcgDate = DateOnly.Parse(ygoTriviaDates.tcg_date);

    }
    else
    {
        ygoTriviaCard.TcgDate = new DateOnly(1990, 01, 01);
    }
    if (ygoTriviaDates.ocg_date is not null)
    {
        ygoTriviaCard.OcgDate = DateOnly.Parse(ygoTriviaDates.ocg_date);

    }
    else
    {
        ygoTriviaCard.OcgDate = new DateOnly(1990, 01, 01);
    }

    ygoTriviaCards.Add(ygoTriviaCard);
}

Console.WriteLine("Creating output.csv - please wait.");
// create a new csv containing the contents of your final product list 

using (var writer = new StreamWriter("C:\\Users\\clifford.lezark\\Documents\\code\\RESTpectfulScraper\\output\\output.csv"))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{

    csv.WriteRecords(ygoTriviaCards);
    Console.WriteLine("Flushing StreamWriter");
    writer.Flush();
}

Console.WriteLine("All done!");

// Data Types, categorized

/// <summary>
/// Data I have on hand
/// </summary>

public class Card
{
    public int id { get; set; }
    public string name { get; set; }
}



/// <summary>
/// Data I can get access to
/// </summary>

public class Root
{
    public List<CardData> data { get; set; }
}

public class CardImages
{
    public int? id { get; set; }
    public string image_url { get; set; }
    public string image_url_small { get; set; }
    public string image_url_cropped { get; set; }
}

public class CardPrice
{
    public string cardmarket_price { get; set; }
    public string tcgplayer_price { get; set; }
    public string ebay_price { get; set; }
    public string amazon_price { get; set; }
    public string coolstuffinc_price { get; set; }
}

public class CardSet
{
    public string set_name { get; set; }
    public string set_code { get; set; }
    public string set_rarity { get; set; }
    public string set_rarity_code { get; set; }
    public string set_price { get; set; }
}

public class CardData
{
    public int? id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string frameType { get; set; }
    public string desc { get; set; }
    public string race { get; set; }
    public string archetype { get; set; }
    public string ygoprodeck_url { get; set; }
    public List<CardSet> card_sets { get; set; }
    public List<CardImages> card_images { get; set; }
    public List<CardPrice> card_prices { get; set; }
    public List<MiscInfo> misc_info { get; set; }
}

public class MiscInfo
{
    public int? views { get; set; }
    public int? viewsweek { get; set; }
    public int? upvotes { get; set; }
    public int? downvotes { get; set; }
    public string[] formats { get; set; }
    public string tcg_date { get; set; }
    public string ocg_date { get; set; }
    public int? konami_id { get; set; }
    public int? has_effect { get; set; }
    public string md_rarity { get; set; }
}

/// Data I want to create
/// 

public class YGOTriviaCard
{
    public int id { get; set; }
    public string Name { get; set; } 

    public string Type { get; set; }
     
    public DateOnly TcgDate { get; set; }

    public DateOnly OcgDate { get; set; }

    public string Artwork { get; set; }

}
