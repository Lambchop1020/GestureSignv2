namespace GestureSign.Foundation.Intent;

public sealed record IntentGestureCorrection(string SampleId, string Gesture, IntentFrame[] Frames);

// Separate from the binary intent model and historical recognizer output.
public static class IntentGestureCorrections
{
    private static string PathFor(string root) => Path.Combine(root, "gesture-corrections.json");
    public static IntentGestureCorrection[] Read(string root)
    {
        try { return IntentFiles.Read<IntentGestureCorrection[]>(PathFor(root)) ?? []; }
        catch (FileNotFoundException) { return []; }
        catch (DirectoryNotFoundException) { return []; }
    }
    public static int Contacts(IntentFrame[] frames)
    {
        if (frames.Length < 2) throw new ArgumentException("轨迹不完整。");
        var ids = frames[0].Points.Select(p => p.Contact).ToHashSet();
        if (ids.Count is < 1 or > 4 || frames.Any(f => f.Points.Length != ids.Count ||
            !ids.SetEquals(f.Points.Select(p => p.Contact)) || f.Points.Any(p => !double.IsFinite(p.X) || !double.IsFinite(p.Y))))
            throw new ArgumentException("轨迹触点不一致，无法用作手势模板。");
        return ids.Count;
    }
    public static void Save(string root, IntentSample sample, string? gesture)
    {
        if (gesture != null) { ArgumentException.ThrowIfNullOrWhiteSpace(gesture); Contacts(sample.Frames); }
        var entries = Read(root).Where(c => c.SampleId != sample.Id).ToList();
        if (gesture != null) entries.Add(new(sample.Id, gesture, sample.Frames));
        IntentFiles.Write(PathFor(root), entries.ToArray());
    }
}
