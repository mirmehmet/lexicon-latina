using System.Net.Http;
using System.Text.Json;

namespace lexicon_latina.Services;

public class TranslationService
{
    private static readonly HttpClient _httpClient = new();

    public TranslationService()
    {
    }

    public async Task<string> TranslateAsync(string turkishText)
    {
        if (string.IsNullOrWhiteSpace(turkishText)) return string.Empty;

        string escapedText = Uri.EscapeDataString(turkishText);
        string url = $"https://clients5.google.com/translate_a/t?client=dict-chrome-ex&sl=tr&tl=en&q={escapedText}";

        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseJson);
        var root = doc.RootElement;

        if (root.ValueKind == JsonValueKind.Array && root.GetArrayLength() > 0)
        {
            var firstElement = root[0];
            if (firstElement.ValueKind == JsonValueKind.Array && firstElement.GetArrayLength() > 0)
            {
                return firstElement[0].GetString() ?? string.Empty;
            }
            else if (firstElement.ValueKind == JsonValueKind.String)
            {
                return firstElement.GetString() ?? string.Empty;
            }
        }

        return string.Empty;
    }
}