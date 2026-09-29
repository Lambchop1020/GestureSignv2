namespace GestureSign.Foundation.Intent;

public sealed class IntentReviewSettings
{
    public bool TwoFingers { get; set; } = true;
    public bool ThreeFourFingers { get; set; } = true;
    public bool SingleFingerCustom { get; set; } = true;
    public string[] ExcludedGestures { get; set; } = [];
    public bool Includes(int count, string? candidate) => !string.IsNullOrWhiteSpace(candidate) &&
        !(ExcludedGestures ?? []).Contains(candidate, StringComparer.OrdinalIgnoreCase) &&
        (count == 1 ? SingleFingerCustom : count == 2 ? TwoFingers : count is 3 or 4 && ThreeFourFingers);
    public static string PathFor(string root) => Path.Combine(root, "review-settings.json");
    public static IntentReviewSettings Read(string root)
    {
        try { return IntentFiles.Read<IntentReviewSettings>(PathFor(root)) ?? new(); }
        catch { return new(); }
    }
    public void Save(string root) => IntentFiles.Write(PathFor(root), this);
}
