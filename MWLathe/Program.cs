using MWLathe.Helpers;
using MWLathe.Records;
using System.Diagnostics;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

if (args.Length == 0 || (args[0] == "-h" && (args.Length == 1 || !args[1].StartsWith("--"))))
{
    ShowGenericHelp();
    Environment.Exit(0);
}

if (args[0] == "-h" && args[1].StartsWith("--"))
{
    ShowModeSpecificHelp(args[1]);
    Environment.Exit(0);
}

string mode = "ID";
string? separator = null;
// TODO: implement
bool partialMatching = false;
string? changelogPath = null;
var fileArgs = new List<string>();

if (args[0].StartsWith("--"))
{
    switch (args[0])
    {
        case "--i":
            mode = "ID";
            break;
        case "--c":
            mode = "Cell";
            break;
        default:
            Console.WriteLine($"Unknown mode \"{args[0]}\". mwlathe.exe -h for help");
            Environment.Exit(1);
            break;
    }
}

int startIndex = args[0].StartsWith("--") ? 1 : 0;

for (int i = startIndex; i < args.Length; i++)
{
    if (mode  == "ID")
    {
        // TODO: these should error if there's no extra argument
        if (args[i] == "-s" && i + 1 < args.Length)
        {
            separator = args[i + 1];
            i++; // Skip the separator value
        }
        else if (args[i] == "-b")
        {
            BOOK.replaceBookText = true;
        }
        else if (args[i] == "-p" && i + 1 < args.Length)
        {
            changelogPath = args[i + 1];
            i++; // Skip the path value
        }
        else if (!args[i].StartsWith('-'))
        {
            fileArgs.Add(args[i]);
        }
    }
    else if (mode == "Cell")
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
        else if (args[i] == "-d")
        {
            INFO.replaceDialogue = true;
        }
        else if (args[i] == "-m")
        {
            partialMatching = true;
        }
        else if (args[i] == "-p" && i + 1 < args.Length)
        {
            changelogPath = args[i + 1];
            i++; // Skip the path value
        }
        else if (!args[i].StartsWith('-'))
        {
            fileArgs.Add(args[i]);
        }
    }
    else
    {
        Console.WriteLine($"Internal error: bad mode \"{mode}\"");
        Environment.Exit(1);
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
        Console.Error.WriteLine($"Skipping: no new value for old value \"{lineParts[0]}\"");
    }
    else if (lineParts[0] == "")
    {
        Console.Error.WriteLine($"Skipping: no old value for new value \"{lineParts[1]}\"");
    }
    else if (mode == "ID" && lineParts[1].Length > 31)
    {
        Console.Error.WriteLine($"Skipping: new value \"{lineParts[1]}\" is {lineParts[1].Length} characters (max 31)");
    }
    else
    {
        Replacements.Add(new Replacement
        {
            Old = lineParts[0],
            New = lineParts[1]
        });
    }
}

if (Replacements.Count == 0)
{
    Console.Error.WriteLine($"Aborting: no replacements read from {Path.GetFileName(mapPath)}");
    Environment.Exit(2);
}
var newIDList = new List<string>();
foreach (var idPair in Replacements)
{
    if (newIDList.Contains(idPair.Old))
    {
        Console.Error.WriteLine($"Warning: new ID {idPair.Old} is later replaced by ID {idPair.New}. This may lead to unexpected results.");
    }
    newIDList.Add(idPair.New);
}
Console.WriteLine($"Read {Replacements.Count} replacement(s) from {Path.GetFileName(mapPath)}. Replacing...");

byte[] buffer = new byte[4];

Stopwatch sw = Stopwatch.StartNew();
if (mode == "ID")
{
    List<string> recordsWithoutID = new List<string>(["LAND", "PGRD", "SKIL", "TES3"]);

    FileStream? cs = null;
    StreamWriter? swChangelog = null;
    if (changelogPath != null)
    {
        cs = new FileStream(changelogPath, FileMode.Create, FileAccess.Write);
        swChangelog = new StreamWriter(cs);
        swChangelog.WriteLine("RecordType\tIdentifier");
        swChangelog.Flush();
    }
    using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
    using (FileStream ts = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
    using (BufferedStream bs = new BufferedStream(fs))
    {
        var updatedRecordCount = 0;
        while (bs.Read(buffer, 0, buffer.Length) > 0)
        {
            Record newRecord = IdentifyRecord(Record.Encoding.GetString(buffer));
            newRecord.Populate(bs);
            if (!recordsWithoutID.Contains(newRecord.GetType().Name))
            {
                foreach (var replacement in Replacements)
                {
                    newRecord.UpdateID(replacement.Old, replacement.New);
                }
                if (newRecord.Updated)
                {
                    updatedRecordCount += 1;
                    if (cs is not null)
                    {
                        swChangelog!.WriteLine($"{newRecord.GetType().Name}\t{newRecord.Identifier}");
                    }
                }
            }
            newRecord.Write(ts);
        }
        if (cs is not null)
        {
            swChangelog!.Dispose();
        }
        Console.WriteLine($"Output successfully written to {Path.GetFileName(outputPath)}. {updatedRecordCount} record(s) updated in {sw}. If scripts were affected, they will need to be recompiled.");
    }
}
else if (mode == "Cell")
{
    List<string> recordsWithCell = new List<string>(["BOOK", "CELL", "CREA", "INFO", "NPC_", "PGRD", "SCPT"]);

    FileStream? cs = null;
    StreamWriter? swChangelog = null;
    if (changelogPath != null)
    {
        cs = new FileStream(changelogPath, FileMode.Create, FileAccess.Write);
        swChangelog = new StreamWriter(cs);
        swChangelog.WriteLine("RecordType\tIdentifier");
        swChangelog.Flush();
    }
    using (FileStream fs = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
    using (FileStream ts = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
    using (BufferedStream bs = new BufferedStream(fs))
    {
        var updatedRecordCount = 0;
        while (bs.Read(buffer, 0, buffer.Length) > 0)
        {
            Record newRecord = IdentifyRecord(Record.Encoding.GetString(buffer));
            newRecord.Populate(bs);
            if (recordsWithCell.Contains(newRecord.GetType().Name))
            {
                foreach (var replacement in Replacements)
                {
                    newRecord.UpdateCell(replacement.Old, replacement.New);
                }
                if (newRecord.Updated)
                {
                    updatedRecordCount += 1;
                    if (cs is not null)
                    {
                        swChangelog!.WriteLine($"{newRecord.GetType().Name}\t{newRecord.Identifier}");
                    }
                }
            }
            newRecord.Write(ts);
        }
        if (cs is not null)
        {
            swChangelog!.Dispose();
        }
        Console.WriteLine($"Output successfully written to {Path.GetFileName(outputPath)}. {updatedRecordCount} record(s) updated in {sw}. If scripts were affected, they will need to be recompiled.");
    }
}
else
{
    Console.WriteLine($"Internal error: bad mode \"{mode}\"");
    Environment.Exit(1);
}

static void ShowGenericHelp()
{
    Console.WriteLine("MWLathe v2.0");
    Console.WriteLine("https://github.com/KnobelKnight/MWLathe");
    Console.WriteLine("Modes:");
    Console.WriteLine("--i | ID replacement mode (default)");
    Console.WriteLine("--c | Cell name replacement mode");
    Console.WriteLine("mwlathe.exe -h <mode> for mode-specific options");
}

static void ShowModeSpecificHelp(string mode)
{
    switch (mode)
    {
        case "--i":
            Console.WriteLine("Usage: mwlathe.exe --i <arguments> <input.esm/esp> <output.esm/esp> <ID map file>");
            Console.WriteLine("For ID map: <old ID>,<new ID>");
            Console.WriteLine("Make sure ID map is headerless and without quotes!");
            Console.WriteLine("-s <separator> | Set custom separator for ID map. Mandatory for non-csv/tsv files");
            Console.WriteLine("-b | Replace IDs within book texts. Useful for ex. PositionCell markers, but unsafe with plaintext IDs");
            Console.WriteLine("-p <changelog file> | Print a list of all affected records to changelog file in tab-separated format");
            break;
        case "--c":
            Console.WriteLine("Usage: mwlathe.exe --c <arguments> <input.esm/esp> <output.esm/esp> <cell name map file>");
            Console.WriteLine("For cell name map: <old name>,<new name>");
            Console.WriteLine("Make sure cell name map is headerless and without quotes!");
            Console.WriteLine("-s <separator> | Set custom separator for cell name map. Mandatory for non-csv/tsv files");
            Console.WriteLine("-b | Replace cell names within book texts");
            Console.WriteLine("-d | Replace cell names within dialogue text");
            Console.WriteLine("-m | Enable partial matching. Ex. changing \"Vivec\" to \"Vivace\" will also update \"Vivec, Arena\"");
            Console.WriteLine("-p <changelog file> | Print a list of all affected records to changelog file in tab-separated format");
            break;
        default:
            Console.WriteLine($"Unknown mode \"{mode}\"");
            Console.WriteLine();
            ShowGenericHelp();
            break;
    }
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
