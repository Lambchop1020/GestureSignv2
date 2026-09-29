using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GestureSign.Foundation.Localization;

/// <summary>Shared UI and daemon text. Catalog keys are stable English format strings.</summary>
public static class IntentLocalization
{
    private static readonly ConcurrentDictionary<string, Dictionary<string, string>> Catalogs = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Lazy<KeyValuePair<string, string>[]> ChineseMessages = new(() =>
        Catalog("zh-CN").OrderByDescending(p => p.Value.Length).ToArray());
    private static Dictionary<string, string> Catalog(string culture) => Catalogs.GetOrAdd(culture, name =>
    {
        using var stream = typeof(IntentLocalization).Assembly.GetManifestResourceStream("Intent." + name + ".json");
        return stream is null ? new(StringComparer.Ordinal) : JsonSerializer.Deserialize<Dictionary<string, string>>(stream) ?? new(StringComparer.Ordinal);
    });
    public static string Format(string culture, string english, params object[] args)
    {
        if (string.IsNullOrWhiteSpace(culture)) culture = CultureInfo.CurrentUICulture.Name;
        string template = Catalog(culture).TryGetValue(english, out var value) ? value : english;
        if (args.Length == 0) return template;
        try { return string.Format(CultureInfo.GetCultureInfo(culture), template, args); }
        catch (CultureNotFoundException) { return string.Format(CultureInfo.InvariantCulture, template, args); }
    }
    // Older optional components return Chinese status text. Translate known messages
    // at the display boundary without changing sample data, gesture IDs, or diagnostics.
    public static string Message(string culture, string? text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        if (Catalog(culture).TryGetValue(text, out var direct)) return direct;
        if (culture.Equals("zh-CN", StringComparison.OrdinalIgnoreCase)) return text;
        foreach (var entry in ChineseMessages.Value)
        {
            if (text == entry.Value) return Format(culture, entry.Key);
        }
        foreach (var entry in ChineseMessages.Value.Where(e => e.Value.Contains('{')))
        {
            var items = Regex.Matches(entry.Value, @"\{(\d+)[^{}]*\}");
            if (items.Count == 0) continue;
            var pattern = items[0].Index == 0 ? "^" : "";
            int offset = 0;
            foreach (Match item in items)
            {
                pattern += Regex.Escape(entry.Value[offset..item.Index]) + "(.+?)";
                offset = item.Index + item.Length;
            }
            pattern += Regex.Escape(entry.Value[offset..]) + (offset == entry.Value.Length ? "$" : "");
            var match = Regex.Match(text, pattern, RegexOptions.Singleline, TimeSpan.FromMilliseconds(50));
            if (!match.Success) continue;
            var values = new object[items.Max(m => int.Parse(m.Groups[1].Value)) + 1];
            for (int i = 0; i < items.Count; i++) values[int.Parse(items[i].Groups[1].Value)] = match.Groups[i + 1].Value;
            text = text[..match.Index] + Format(culture, entry.Key, values) + text[(match.Index + match.Length)..];
        }
        foreach (var entry in ChineseMessages.Value)
        {
            if (entry.Value.Length < 2 || !Regex.IsMatch(entry.Value, "[\\u4e00-\\u9fff]")) continue;
            if (!entry.Value.Contains('{'))
                text = text.Replace(entry.Value, Format(culture, entry.Key), StringComparison.Ordinal);
        }
        return text;
    }
}
