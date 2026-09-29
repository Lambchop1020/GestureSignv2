using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using GestureSign.Foundation.Localization;
var assembly=typeof(IntentLocalization).Assembly;
var files=assembly.GetManifestResourceNames().Where(n=>n.StartsWith("Intent.")).ToArray();
if(files.Length!=90)throw new Exception($"Expected 90 cultures, found {files.Length}");
Dictionary<string,string> Load(string name) { using var s=assembly.GetManifestResourceStream(name)!;return JsonSerializer.Deserialize<Dictionary<string,string>>(s)!; }
var baseline=Load("Intent.en-US.json");int checks=0;
foreach(var file in files){var culture=file[7..^5]; var entries=Load(file);
if(!entries.Keys.Order().SequenceEqual(baseline.Keys.Order()))throw new Exception("Missing keys: "+culture);
foreach(var (key,value) in entries){
 if(string.IsNullOrWhiteSpace(value)||value.Contains('\uFFFD')||value.Contains("ZXQ",StringComparison.OrdinalIgnoreCase))throw new Exception("Invalid text: "+culture+" "+key);
 string[] Items(string text)=>Regex.Matches(text,@"\{\d+[^{}]*\}").Select(m=>m.Value).Order().ToArray();
 if(!Items(key).SequenceEqual(Items(value)))throw new Exception("Placeholder mismatch: "+culture+" "+key);
 var argsForFormat=Enumerable.Repeat<object>(427,12).ToArray();
 string.Format(CultureInfo.GetCultureInfo(culture),value,argsForFormat);
 if(IntentLocalization.Format(culture,key)!=value)throw new Exception("Catalog lookup failed: "+culture);
 checks++;
}
var counts=IntentLocalization.Format(culture,"Gesture samples: labeled scrolling {0} · labeled intentional gestures {1} · unlabeled {2}",427,182,1053);
if(!counts.Contains("427")||!counts.Contains("182")||!counts.Contains("1053"))throw new Exception("Counts lost: "+culture);
var notice=IntentLocalization.Format(culture,"AI blocked {1} drawing gestures in the last {0} seconds. Click to review or correct the decisions.",30,7);
if(!notice.Contains("30")||!notice.Contains("7"))throw new Exception("Toast counts lost: "+culture);
var message=IntentLocalization.Message(culture,"模型认为可能是滚动（评分 0.42）");
if(!message.Contains("0.42"))throw new Exception("Runtime score lost: "+culture);
if(culture is not ("zh-CN" or "zh-TW" or "ja-JP") && Regex.IsMatch(message,"[\\u4e00-\\u9fff]"))throw new Exception("Chinese runtime fallback: "+culture);
}
if(IntentLocalization.Format("ja-JP","On")!="オン")throw new Exception("Japanese on label");
if(IntentLocalization.Format("zh-TW","AI scoring")!="AI 評分")throw new Exception("Traditional Chinese scoring label");
// Regression checks for ambiguous English words mistranslated without UI context.
var terminology = new (string Culture, string Key, string Expected)[] {
 ("de-DE", "Right", "Rechte Maustaste"),
 ("ar-SA", "Left", "الزر الأيسر"),
 ("zh-TW", "Clear label", "撤銷標註"),
 ("ur-PK", "Unlabeled", "بغیر لیبل"),
 ("sr-Cyrl-RS", "On", "Укључено"),
 ("ja-JP", "{0}-finger swipe down", "{0}本指で下にスワイプ")
};
foreach(var (culture,key,expected) in terminology)
 if(IntentLocalization.Format(culture,key)!=expected)throw new Exception("Terminology regression: "+culture+" "+key);
foreach(var file in files) {
 var entries=Load(file);
 if(entries["On"]==entries["Off"] || entries["Left"]==entries["Right"])
  throw new Exception("Opposite controls have identical labels: "+file);
 if(entries.Values.Any(v=>v.Contains("हें नांव\n") || v.Contains("हें नांव\r")))
  throw new Exception("Translation service heading leaked into UI: "+file);
}
Console.WriteLine($"PASS: {files.Length} cultures, {checks} catalog entries, dynamic sample counts, toast counts and legacy runtime messages.");

if(args.Length>0){
var ui=Path.Combine(args[0],"GestureSign.WinUI");
const string literal="\"(?:\\\\.|[^\"\\\\])*\"";
var patterns=new[]{@"IntentFormat\(\s*(?<key>"+literal+@")\s*[,)]",@"IntentText\(\s*"+literal+@"\s*,\s*(?<key>"+literal+@")\s*\)"};
foreach(var path in Directory.GetFiles(ui,"*.cs"))foreach(var pattern in patterns)foreach(Match m in Regex.Matches(Regex.Replace(File.ReadAllText(path), @"(?m)^\s*//[^\r\n]*", ""),pattern)){
var key=JsonSerializer.Deserialize<string>(m.Groups["key"].Value)!;
if(!baseline.ContainsKey(key))throw new Exception("Source text missing from catalog: "+key);
}
Console.WriteLine("PASS: UI source text keys are present in all catalogs.");
}
