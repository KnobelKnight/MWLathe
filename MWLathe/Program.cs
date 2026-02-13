using MWLathe.Helpers;
using MWLathe.Records;
using System.Diagnostics;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

if (args.Length == 0 || args[0] == "-h")
{
    Console.WriteLine("MWLathe v1.1");
    Console.WriteLine("https://github.com/KnobelKnight/MWLathe");
    Console.WriteLine("Usage: mwlathe.exe <input.esm/esp> <output.esm/esp> <id_map>");
    Console.WriteLine("For id_map: <old ID>,<new ID>");
    Console.WriteLine("Make sure id_map is headerless and without quotes!");
    Console.WriteLine("Options:");
    Console.WriteLine("-s <separator> | Set custom separator for id_map. Mandatory for non-csv/tsv files");
    Console.WriteLine("-b | Replace IDs within book texts. Useful for ex. PositionCell markers, but unsafe with plaintext IDs");
    Environment.Exit(0);
}

string? separator = null;
var fileArgs = new List<string>();

for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "-s" && i + 1 < args.Length)
    {
        separator = args[i + 1];
        i++; // Skip the separator value
    }
    else if (args[i] == "-b")
    {
        BOOK.replaceBookText = true;
    }
    else if (!args[i].StartsWith("-"))
    {
        fileArgs.Add(args[i]);
    }
}

if (fileArgs.Count < 3
    || (!fileArgs[0].EndsWith(".esm", StringComparison.OrdinalIgnoreCase) && !fileArgs[0].EndsWith(".esp", StringComparison.OrdinalIgnoreCase))
    || (!fileArgs[1].EndsWith(".esm", StringComparison.OrdinalIgnoreCase) && !fileArgs[1].EndsWith(".esp", StringComparison.OrdinalIgnoreCase)))
{
    Console.WriteLine("Invalid format or file types. mwlathe.exe -h for help");
    Environment.Exit(1);
}

var inputPath = Path.Combine(Directory.GetCurrentDirectory(), fileArgs[0]);
var outputPath = Path.Combine(Directory.GetCurrentDirectory(), fileArgs[1]);
var mapPath = Path.Combine(Directory.GetCurrentDirectory(), fileArgs[2]);


if (separator == null)
{
    if (fileArgs[2].EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
    {
        separator = ",";
    }
    else if (fileArgs[2].EndsWith(".tsv", StringComparison.OrdinalIgnoreCase))
    {
        separator = "\t";
    }
    else
    {
        Console.Error.WriteLine($"Aborting: map file must be csv, tsv, or specify a separator");
        Environment.Exit(2);
    }
}

if (inputPath.Equals(outputPath, StringComparison.OrdinalIgnoreCase))
{
    Console.Error.WriteLine($"Aborting: input and output files identical");
    Environment.Exit(2);
}
else if (!Path.Exists(inputPath))
{
    Console.Error.WriteLine($"Aborting: no such file \"{inputPath}\"");
    Environment.Exit(2);
}
else if (!Path.Exists(mapPath))
{
    Console.Error.WriteLine($"Aborting: no such file \"{mapPath}\"");
    Environment.Exit(2);
}

List<Replacement> Replacements = new List<Replacement>();

foreach (var line in File.ReadLines(mapPath))
{
    string[] lineParts = line.Split(separator);
    if (lineParts.Length == 1 || lineParts[1] == "")
    {
        Console.Error.WriteLine($"Skipping: no new ID for old ID \"{lineParts[0]}\"");
    }
    else if (lineParts[0] == "")
    {
        Console.Error.WriteLine($"Skipping: no old ID for new ID \"{lineParts[1]}\"");
    }
    else if (lineParts[1].Length > 31)
    {
        Console.Error.WriteLine($"Skipping: new ID \"{lineParts[1]}\" is {lineParts[1].Length} characters (max 31)");
    }
    else
    {
        Replacements.Add(new Replacement
        {
            OldID = lineParts[0],
            NewID = lineParts[1]
        });
    }
}

if (Replacements.Count == 0)
{
    Console.Error.WriteLine($"Aborting: no replacement records read from {Path.GetFileName(mapPath)}");
    Environment.Exit(2);
}
var newIDList = new List<string>();
foreach (var idPair in Replacements)
{
    if (newIDList.Contains(idPair.OldID))
    {
        Console.Error.WriteLine($"Warning: new ID {idPair.OldID} is later replaced by ID {idPair.NewID}. This may lead to unexpected results.");
    }
    newIDList.Add(idPair.NewID);
}
Console.WriteLine($"Read {Replacements.Count} replacement record(s) from {Path.GetFileName(mapPath)}. Replacing...");

byte[] buffer = new byte[4];

Stopwatch sw = Stopwatch.StartNew();
List<string> recordsWithoutID = new List<string>(["LAND", "PGRD", "SKIL", "TES3"]);

using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
using (FileStream ts = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
using (BufferedStream bs = new BufferedStream(fs))
{
    while (bs.Read(buffer, 0, buffer.Length) > 0)
    {
        Record newRecord = IdentifyRecord(Record.Encoding.GetString(buffer));
        newRecord.Populate(bs);
        if (!recordsWithoutID.Contains(newRecord.GetType().Name))
        {
            foreach (var replacement in Replacements)
            {
                newRecord.UpdateID(replacement.OldID, replacement.NewID);
            }
        }
        newRecord.Write(ts);
    }
    sw.Stop();
    Console.WriteLine($"Output successfully written to {Path.GetFileName(outputPath)} in {sw}. If scripts were affected, they will need to be recompiled.");
}

static Record IdentifyRecord(string recordType)
{
    switch (recordType)
    {
        case "ACTI":
            return new ACTI();
        case "ALCH":
            return new ALCH();
        case "APPA":
            return new APPA();
        case "ARMO":
            return new ARMO();
        case "BODY":
            return new BODY();
        case "BOOK":
            return new BOOK();
        case "BSGN":
            return new BSGN();
        case "CELL":
            return new CELL();
        case "CLAS":
            return new CLAS();
        case "CLOT":
            return new CLOT();
        case "CONT":
            return new CONT();
        case "CREA":
            return new CREA();
        case "DIAL":
            return new DIAL();
        case "DOOR":
            return new DOOR();
        case "ENCH":
            return new ENCH();
        case "FACT":
            return new FACT();
        case "GLOB":
            return new GLOB();
        case "GMST":
            return new GMST();
        case "INFO":
            return new INFO();
        case "INGR":
            return new INGR();
        case "LAND":
            return new LAND();
        case "LEVC":
            return new LEVC();
        case "LEVI":
            return new LEVI();
        case "LIGH":
            return new LIGH();
        case "LOCK":
            return new LOCK();
        case "LTEX":
            return new LTEX();
        case "MGEF":
            return new MGEF();
        case "MISC":
            return new MISC();
        case "NPC_":
            return new NPC_();
        case "PGRD":
            return new PGRD();
        case "PROB":
            return new PROB();
        case "RACE":
            return new RACE();
        case "REGN":
            return new REGN();
        case "REPA":
            return new REPA();
        case "SCPT":
            return new SCPT();
        case "SKIL":
            return new SKIL();
        case "SNDG":
            return new SNDG();
        case "SOUN":
            return new SOUN();
        case "SPEL":
            return new SPEL();
        case "SSCR":
            return new SSCR();
        case "STAT":
            return new STAT();
        case "TES3":
            return new TES3();
        case "WEAP":
            return new WEAP();
        default:
            throw new NotImplementedException($"Unknown record type {recordType}");
    }
}
