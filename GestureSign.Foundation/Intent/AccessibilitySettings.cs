namespace GestureSign.Foundation.Intent;

public sealed class AccessibilitySettings
{
    public bool LargeControls { get; set; }
    public bool NumberTrails { get; set; } = true;
    public int TouchpadEdgePercent { get; set; } = 8;
    private static AccessibilitySettings _current = new();
    public static AccessibilitySettings Current => System.Threading.Volatile.Read(ref _current);
    private static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GestureSign V2", "accessibility.json");
    public static void Reload()
    {
        try { System.Threading.Volatile.Write(ref _current, IntentFiles.Read<AccessibilitySettings>(SettingsPath) ?? new()); }
        catch { }
    }
    public void Save() { IntentFiles.Write(SettingsPath, this); System.Threading.Volatile.Write(ref _current, this); }
}
