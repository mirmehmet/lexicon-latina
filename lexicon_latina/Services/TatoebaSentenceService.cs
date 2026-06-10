using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using lexicon_latina.Models;

namespace lexicon_latina.Services;

public class TatoebaSentenceService
{
    private static readonly HttpClient _httpClient = new()
    {
        DefaultRequestHeaders = { { "User-Agent", "Mozilla/5.0" } }
    };

    public TatoebaSentenceService()
    {
    }

    public async Task<List<TatoebaSentence>> GetSentencesAsync(string latinWord)
    {
        var sentences = new List<TatoebaSentence>();
        if (string.IsNullOrWhiteSpace(latinWord)) return sentences;

        // Clean/sanitize the word (take the first lemma form if there are multiple, e.g. "lascivus, lasciva, lascivum")
        string cleanWord = SanitizeLatinWord(latinWord);
        if (string.IsNullOrEmpty(cleanWord)) return sentences;

        // 1. Try Turkish translations first
        sentences = await QueryTatoebaAsync(cleanWord, "tur");

        // 2. Fall back to English if Turkish translations are not found
        if (sentences.Count == 0)
        {
            sentences = await QueryTatoebaAsync(cleanWord, "eng");
        }

        return sentences.Take(5).ToList();
    }

    private async Task<List<TatoebaSentence>> QueryTatoebaAsync(string word, string transLang)
    {
        var list = new List<TatoebaSentence>();
        try
        {
            string url = $"https://api.tatoeba.org/v1/sentences?q={Uri.EscapeDataString(word)}&lang=lat&sort=relevance&trans:lang={transLang}&showtrans=matching";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return list;

            string json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("data", out var dataProp) && dataProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in dataProp.EnumerateArray())
                {
                    var latinText = item.TryGetProperty("text", out var textProp) ? textProp.GetString() ?? "" : "";
                    var translationText = "";

                    if (item.TryGetProperty("translations", out var transProp) && transProp.ValueKind == JsonValueKind.Array && transProp.GetArrayLength() > 0)
                    {
                        var firstTrans = transProp[0];
                        translationText = firstTrans.TryGetProperty("text", out var transTextProp) ? transTextProp.GetString() ?? "" : "";
                    }

                    if (!string.IsNullOrEmpty(latinText) && !string.IsNullOrEmpty(translationText))
                    {
                        list.Add(new TatoebaSentence
                        {
                            LatinText = latinText,
                            TranslationText = translationText
                        });
                    }
                }
            }
        }
        catch
        {
            // Fail silently
        }

        return list;
    }

    private static string SanitizeLatinWord(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return string.Empty;

        // If the word has comma-separated dictionary forms (e.g. "lascivus, lasciva, lascivum"),
        // take the first lemma form.
        string firstPart = word.Split(',')[0].Trim();

        return firstPart;
    }
}
