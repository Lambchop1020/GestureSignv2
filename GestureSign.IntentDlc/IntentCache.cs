namespace GestureSign.IntentDlc;

internal static class IntentCache
{
    // Only generated 16-digit hash directories are eligible. Keep the active
    // entry plus one previous version; never traverse junctions or symlinks.
    internal static void Prune(string root, string activeKey)
    {
        try
        {
            var parent = new DirectoryInfo(root);
            if (!parent.Exists || (parent.Attributes & FileAttributes.ReparsePoint) != 0) return;
            var old = parent.GetDirectories().Where(d => d.Name.Length == 16 && d.Name.All(Uri.IsHexDigit)
                && !d.Name.Equals(activeKey, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(d => d.LastWriteTimeUtc).Skip(1);
            foreach (var directory in old)
            {
                try
                {
                    if (ContainsLink(directory)) continue;
                    directory.Delete(true);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { }
    }
    private static bool ContainsLink(DirectoryInfo directory) =>
        (directory.Attributes & FileAttributes.ReparsePoint) != 0 ||
        directory.EnumerateFileSystemInfos().Any(item => (item.Attributes & FileAttributes.ReparsePoint) != 0 ||
            item is DirectoryInfo child && ContainsLink(child));
}
