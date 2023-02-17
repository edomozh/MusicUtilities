using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

var rawtrackids = Get("https://api.music.yandex.ru/users/egor.domozhirov.ya/likes/tracks");
var jsonoftrackids = (JObject)JsonConvert.DeserializeObject(rawtrackids);
var arrayoftrackids = jsonoftrackids?["result"]?["library"]?["tracks"]?.Select(t => t["id"]?.ToString()).ToList().ToArray();
Console.WriteLine($"{arrayoftrackids.Length} ids found");

//var trackinfos = Post("https://api.music.yandex.ru/tracks/", arrayoftrackids);
//var parsedtrackinfos = (JObject)JsonConvert.DeserializeObject(trackinfos);

foreach (var id in arrayoftrackids)
{
    try
    {
        var track = Get($"https://api.music.yandex.ru/tracks/{id}");
        var parsedtrack = (JObject)JsonConvert.DeserializeObject(track);
        var title = parsedtrack?["result"]?[0]?["title"]?.ToString();
        var artists = parsedtrack["result"][0]["artists"].Select(a => a["name"]?.ToString());

        var fullname = $"{title} - {string.Join(',', artists.ToArray())}";
        File.AppendAllText(@"C:\Users\egord\Desktop\tracks.txt", $"{fullname}\n");
    }
    catch { }
}

string Get(string url)
{
    var client = new HttpClient();
    using var response = client.GetAsync(url).GetAwaiter().GetResult();
    response.EnsureSuccessStatusCode();
    var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    return responseString;

}

string Post(string url, string[] trackids)
{
    var client = new HttpClient();
    var rawcontent = $"{{track-ids:[{string.Join(',', trackids)}]}}";
    var content = new StringContent(rawcontent, Encoding.UTF8, "application/json");
    var response = client.PostAsync(url, content).GetAwaiter().GetResult(); ;
    response.EnsureSuccessStatusCode();
    var responseString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    return responseString;
}