
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using lexicon_latina.Models;

namespace lexicon_latina.Services
{

    public class LatinIsSimpleService
    {
        private readonly HttpClient _httpClient;

        private const string BaseUrl = "https://www.latin-is-simple.com/api/vocabulary/search/";

        public LatinIsSimpleService()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "LexiconLatina/1.0");
        }

        public async Task<List<LatinEntry>> SearchAsync(string englishWord)
        {

            var url = $"{BaseUrl}?query={Uri.EscapeDataString(englishWord)}&forms_only=false&format=json";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<LatinEntry>();

            var json = await response.Content.ReadAsStringAsync();

            return ParseResults(json);
        }

        private List<LatinEntry> ParseResults(string json)
        {
            var entries = new List<LatinEntry>();

            try
            {

                var root = JsonSerializer.Deserialize<JsonElement>(json);

                if (root.ValueKind != JsonValueKind.Array)
                    return entries;

                foreach (var item in root.EnumerateArray())
                {

                    var shortName = item.TryGetProperty("short_name", out var sn)
                        ? sn.GetString() ?? "" : "";

                    var pos = "";
                    if (item.TryGetProperty("type", out var type)
                        && type.TryGetProperty("label", out var label))
                        pos = label.GetString() ?? "";

                    var meaning = "";
                    if (item.TryGetProperty("translations_unstructured", out var trans)
                        && trans.TryGetProperty("en", out var en))
                        meaning = en.GetString() ?? "";

                    if (meaning.Length > 200)
                        meaning = meaning[..200] + "...";

                    if (meaning.Contains("not yet translated"))
                        continue;

                    if (!string.IsNullOrWhiteSpace(shortName))
                    {
                        entries.Add(new LatinEntry
                        {
                            Word         = shortName,
                            PartOfSpeech = TranslatePos(pos),
                            ShortMeaning = meaning,
                            Source       = "Latin is Simple"
                        });
                    }
                }
            }
            catch
            {

            }

            return entries;
        }

        private static string TranslatePos(string pos) => pos switch
        {
            "Noun"      => "isim",
            "Verb"      => "fiil",
            "Adjective" => "sıfat",
            "Adverb"    => "zarf",
            "Phrase"    => "deyim",
            _           => pos
        };
    }
}