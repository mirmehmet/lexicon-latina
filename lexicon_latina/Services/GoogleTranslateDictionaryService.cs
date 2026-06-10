using System.Net.Http;
using System.Text.Json;
using lexicon_latina.Models;

namespace lexicon_latina.Services;

public class GoogleTranslateDictionaryService
{
    private static readonly HttpClient _httpClient = new()
    {
        DefaultRequestHeaders = { { "User-Agent", "Mozilla/5.0" } }
    };

    private const string BaseUrl = "https://clients5.google.com/translate_a/single?client=gtx&sl=en&tl=la&dt=t&dt=bd&dj=1";

    public GoogleTranslateDictionaryService()
    {
    }

    public async Task<List<LatinEntry>> SearchAsync(string englishWord)
    {
        var entries = new List<LatinEntry>();
        if (string.IsNullOrWhiteSpace(englishWord)) return entries;

        try
        {
            var url = $"{BaseUrl}&q={Uri.EscapeDataString(englishWord)}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return entries;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("dict", out var dictProp) && dictProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var dictItem in dictProp.EnumerateArray())
                {
                    var pos = dictItem.TryGetProperty("pos", out var posProp) ? posProp.GetString() ?? "" : "";
                    
                    if (dictItem.TryGetProperty("entry", out var entryProp) && entryProp.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var entryItem in entryProp.EnumerateArray())
                        {
                            var word = entryItem.TryGetProperty("word", out var wordProp) ? wordProp.GetString() ?? "" : "";
                            
                            var meaningsList = new List<string>();
                            if (entryItem.TryGetProperty("reverse_translation", out var revProp) && revProp.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var meaningVal in revProp.EnumerateArray())
                                {
                                    var meaningStr = meaningVal.GetString();
                                    if (!string.IsNullOrEmpty(meaningStr))
                                    {
                                        meaningsList.Add(meaningStr);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(word))
                            {
                                entries.Add(new LatinEntry
                                {
                                    Word = word,
                                    PartOfSpeech = TranslatePos(pos),
                                    ShortMeaning = string.Join("; ", meaningsList),
                                    Source = "Google"
                                });
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // Fail silently or handle accordingly
        }

        return entries;
    }

    private static string TranslatePos(string pos) => pos.ToLowerInvariant() switch
    {
        "noun"      => "isim",
        "verb"      => "fiil",
        "adjective" => "sıfat",
        "adverb"    => "zarf",
        "phrase"    => "deyim",
        "conjunction" => "bağlaç",
        "preposition" => "edat",
        _           => pos
    };
}
