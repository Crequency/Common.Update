using Common.Update.Checker;
using Common.Update.Manual;
using System.Text.Json;

Console.WriteLine("Common.Update.Manual");
Console.WriteLine("v1.0.0.1");

var ask = (string tip, ConsoleColor? cc) =>
{
    ConsoleColor ncc = Console.ForegroundColor;
    if (cc != null) Console.ForegroundColor = (ConsoleColor)cc;
    Console.Write(tip);
    Console.ForegroundColor = ncc;
    string? result = Console.ReadLine();
    return result;
};

var int4 = (string input, int mean) =>
{
    if (int.TryParse(input, out int x)) return x;
    else return mean;
};

string baseDir = Path.GetFullPath(ask("Root Directory: ", null));

Checker checker = new Checker()
    .SetRootDirectory(baseDir)
    .SetPerThreadFilesCount(int4(ask("Thread Limit: ", ConsoleColor.Yellow), 20))
    .SetTransHash2String(true);

int _ignoreFoldersCount = int4(ask("Ignore Folders Count: ", null), 0);
for (int i = 0; i < _ignoreFoldersCount; i++)
    _ = checker.AppendIgnoreFolder(Path.GetFullPath(ask($"_ignoreFolder{i}:\t", null)));

int _ignoreFormatsCount = int4(ask("Ignore Formats Count: ", null), 0);
for (int i = 0; i < _ignoreFormatsCount; i++)
    _ = checker.AppendIgnoreFormat(ask($"_ignoreFormat{i}:\t", null));

int _includeFilesCount = int4(ask("Include Files Count: ", null), 0);
for (int i = 0; i < _includeFilesCount; i++)
    _ = checker.AppendIncludeFile(Path.GetFullPath($"{baseDir}/{ask($"_includeFile{i}:\t", null)}"));

Console.WriteLine("Ready? Enter to Continue!");
Console.ReadLine();

bool isScanned = false;
bool canCalculate = false;

new Thread(() =>
{
    checker.Scan();
    isScanned = true;
    while (!canCalculate) { }
    checker.Calculate();
}).Start();

while (!isScanned) Console.WriteLine("Scanning ...");

checker.PrintScannedFiles(Console.Out);
Console.WriteLine("Scanning finished!");
Console.WriteLine("Enter to Calculate!");

Console.ReadLine();

canCalculate = true;

while (true)
{
    var pro = checker.GetProgress();
    Console.WriteLine($"Calculating ... ({pro.Item1}/{pro.Item2})");
    if (pro.Item1.Equals(pro.Item2)) break;
}

Console.WriteLine("Calculated!");
Console.WriteLine();

var result = checker.GetCalculateResult();
Dictionary<string, (string, string)> toJson = new();
foreach (var item in result)
{
    toJson.Add(item.Key, (item.Value.Item1.ToUpper(), item.Value.Item2.ToUpper()));
}

JsonSerializerOptions options = new()
{
    WriteIndented = true,
    IncludeFields = true,
    PropertyNamingPolicy = new StringCoupleNamingPolicy(),
};
string json = JsonSerializer.Serialize(toJson, options: options);
Console.WriteLine(json);

File.WriteAllText(Path.GetFullPath($"{baseDir}/Common.Update.Result.json"), json);

Console.WriteLine();
