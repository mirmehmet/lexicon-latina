using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using lexicon_latina.Models;

namespace lexicon_latina.Services;

public class FavoritesService
{
    private static readonly FavoritesService _instance = new();
    public static FavoritesService Instance => _instance;

    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };
    private readonly string _filePath;

    public ObservableCollection<LatinEntry> Entries { get; } = new();

    private FavoritesService()
    {
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LexiconLatina");
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }
        _filePath = Path.Combine(appDataPath, "favorites.json");
        LoadFavorites();
    }

    private void LoadFavorites()
    {
        try
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                var list = JsonSerializer.Deserialize<List<LatinEntry>>(json);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        item.IsFavorited = true;
                        Entries.Add(item);
                    }
                }
            }
        }
        catch (Exception) { }
    }

    private void SaveFavorites()
    {
        try
        {
            string json = JsonSerializer.Serialize(Entries.ToList(), _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch (Exception) { }
    }

    public bool IsFavorited(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return false;
        return Entries.Any(e => e.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
    }

    public void Add(LatinEntry entry)
    {
        if (entry == null || string.IsNullOrWhiteSpace(entry.Word)) return;
        if (IsFavorited(entry.Word)) return;

        var newEntry = new LatinEntry
        {
            Word = entry.Word,
            PartOfSpeech = entry.PartOfSpeech,
            ShortMeaning = entry.ShortMeaning,
            Source = entry.Source,
            IsFavorited = true
        };

        Entries.Add(newEntry);
        SaveFavorites();
    }

    public void Remove(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return;
        var existing = Entries.FirstOrDefault(e => e.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            Entries.Remove(existing);
            SaveFavorites();
        }
    }
}
