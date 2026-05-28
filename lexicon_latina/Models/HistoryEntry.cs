namespace lexicon_latina.Models;

public class HistoryEntry
{
    public string InputTurkish { get; set; } = string.Empty;

    public string TranslatedWord { get; set; } = string.Empty;

    public DateTime SearchedAt { get; set; } = DateTime.Now;

    public string SearchedAtDisplay => SearchedAt.ToString("HH:mm  dd MMM");
}
