using GestureSign.Foundation.Intent;
using GestureSign.IntentLearning;

namespace GestureSign.IntentDlc;

// The same model and decision thresholds as the hardware engine, without loading
// ONNX, Windows ML, WinRT or any execution provider into the background process.
internal sealed partial class HardwareInference : IDisposable
{
    internal static Action<string>? DiagnosticTrace { get; set; }
    private IntentModel? _model;
    public string Status => _model == null ? "未训练" : "CPU · 本地分类器";
    public string HardwarePreview => "CPU · 本地分类器";
    public bool Eligible => _model?.EligibleForProtection == true;
    public Task DetectAsync() => Task.CompletedTask;
    public Task LoadAsync(IntentModel model, bool installProviders, string? testVendor = null)
    {
        model.Validate();
        _model = model;
        DiagnosticTrace?.Invoke("Inference: selected CPU local classifier");
        return Task.CompletedTask;
    }
    public IntentPrediction Predict(float[] features)
    {
        var model = _model;
        if (model == null) return new(0, Status, "No trained model");
        if (features.Length != IntentFeatures.Count || features.Any(v => !float.IsFinite(v)))
            return new(0, Status, "Invalid feature vector");
        return new(model.Score(features), Status);
    }
    public void Dispose() { }
}
