namespace GestureSign.IntentDlc;

internal sealed partial class HardwareInference
{
    internal static bool IsSupportedGpu(uint vendorId, string? vendor) =>
        vendorId is 0x10de or 0x1002 or 0x8086 or 0x5143 or 0x17cb ||
        new[] { "NVIDIA", "Advanced Micro Devices", "AMD", "Intel", "Qualcomm" }.Any(name => vendor?.Contains(name, StringComparison.OrdinalIgnoreCase) == true);
}
