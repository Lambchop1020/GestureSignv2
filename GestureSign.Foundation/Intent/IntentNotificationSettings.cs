namespace GestureSign.Foundation.Intent;

public sealed class IntentNotificationSettings
{
    public bool Enabled { get; set; } = true;
    public static IntentNotificationSettings Read() { try { return IntentFiles.Read<IntentNotificationSettings>(SettingsPath) ?? new(); } catch { return new(); } }
    private static string SettingsPath => Path.Combine(IntentFiles.Root, "notifications.json");
    public static bool ReadEnabled()
    {
        try { return IntentFiles.Read<IntentNotificationSettings>(SettingsPath)?.Enabled ?? true; }
        catch (FileNotFoundException) { return true; }
        catch (DirectoryNotFoundException) { return true; }
        catch { return false; }
    }
    public static void Save(bool enabled) { var settings = Read(); settings.Enabled = enabled; IntentFiles.Write(SettingsPath, settings); }
}
