using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using lexicon_latina.Models;

namespace lexicon_latina.Services;

public class HistoryService
{
    private static readonly HistoryService _instance = new();
    public static HistoryService Instance => _instance;

    private const int MaxItems = 10;
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly string _filePath;

    public ObservableCollection<HistoryEntry> Entries { get; } = new();

    public static event Action<HistoryEntry>? SearchRequested;

    public static void RequestSearch(HistoryEntry entry) =>
        SearchRequested?.Invoke(entry);

    private HistoryService()
    {
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LexiconLatina");
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }
        _filePath = Path.Combine(appDataPath, "history.json");
        LoadHistory();
    }

    private void LoadHistory()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                var list = JsonSerializer.Deserialize<List<HistoryEntry>>(json);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        Entries.Add(item);
                    }
                }
            }
        }
        catch (Exception) { }
    }

    private void SaveHistory()
    {
        try
        {
            string json = JsonSerializer.Serialize(Entries.ToList(), _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception) { }
    }

    public void Add(string inputTurkish, string translatedWord)
    {
        if (string.IsNullOrWhiteSpace(inputTurkish)) return;

        if (Entries.Count > 0 &&
            Entries[0].InputTurkish.Equals(inputTurkish, StringComparison.OrdinalIgnoreCase))
            return;

        var entry = new HistoryEntry
        {
            InputTurkish   = inputTurkish,
            TranslatedWord = translatedWord,
            SearchedAt     = DateTime.Now
        };

        Entries.Insert(0, entry);

        while (Entries.Count > MaxItems)
            Entries.RemoveAt(Entries.Count - 1);

        SaveHistory();
    }
}
