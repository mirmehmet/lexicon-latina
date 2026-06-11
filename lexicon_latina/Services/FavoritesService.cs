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
    private readonly string _sentencesFilePath;

    public ObservableCollection<LatinEntry> Entries { get; } = new();
    public ObservableCollection<TatoebaSentence> Sentences { get; } = new();

    private FavoritesService()
    {
        string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LexiconLatina");
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }
        _filePath = Path.Combine(appDataPath, "favorites.json");
        _sentencesFilePath = Path.Combine(appDataPath, "favorite_sentences.json");
        LoadFavorites();
        LoadSentences();
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

    private void LoadSentences()
    {
        try
        {
            if (File.Exists(_sentencesFilePath))
            {
                string json = File.ReadAllText(_sentencesFilePath);
                var list = JsonSerializer.Deserialize<List<TatoebaSentence>>(json);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        item.IsFavorited = true;
                        Sentences.Add(item);
                    }
                }
            }
        }
        catch (Exception) { }
    }

    private void SaveSentences()
    {
        try
        {
            string json = JsonSerializer.Serialize(Sentences.ToList(), _jsonOptions);
            File.WriteAllText(_sentencesFilePath, json);
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

    public bool IsSentenceFavorited(string latinText)
    {
        if (string.IsNullOrWhiteSpace(latinText)) return false;
        return Sentences.Any(s => s.LatinText.Equals(latinText, StringComparison.OrdinalIgnoreCase));
    }

    public void AddSentence(TatoebaSentence sentence)
    {
        if (sentence == null || string.IsNullOrWhiteSpace(sentence.LatinText)) return;
        if (IsSentenceFavorited(sentence.LatinText)) return;

        var newSentence = new TatoebaSentence
        {
            LatinText = sentence.LatinText,
            TranslationText = sentence.TranslationText,
            IsFavorited = true
        };

        Sentences.Add(newSentence);
        SaveSentences();
    }

    public void RemoveSentence(string latinText)
    {
        if (string.IsNullOrWhiteSpace(latinText)) return;
        var existing = Sentences.FirstOrDefault(s => s.LatinText.Equals(latinText, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
            Sentences.Remove(existing);
            SaveSentences();
        }
    }
}
