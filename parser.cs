namespace SYNCORE
{
    using System.IO.Compression;

    public class parser
    {
        public void parseProfile(string file)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Parsing...");
            Console.WriteLine("Removing old files...");
            try
            {
                Directory.Delete(Globals.SYNPROFILER.tempPath, true);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                Console.WriteLine("Nothing to remove, proceeding...");
            }
            Console.WriteLine("Extracting profile...");
            ZipFile.ExtractToDirectory(file, Globals.SYNPROFILER.tempPath);
            Console.WriteLine("Scanning files...");
            foreach (String f in Directory.EnumerateFiles(Globals.SYNPROFILER.tempPath))
            {

                if (Path.GetFileName(f) == "MF")
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Manifest found!\nParsing Manifest...");
                    Globals.SYNPROFILER.manifest = File.ReadAllLines(f);
                    foreach (string l in Globals.SYNPROFILER.manifest)
                    {
                        if (l.Split(':')[0] == "NAME") Globals.SYNPROFILER.profileName = l.Split(':')[1].Trim();
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                if (Path.GetFileName(f).StartsWith("MU_")) // Might need to wrestle with regex so submenus aren't snorted by this
                {
                    string n = Path.GetFileName(f);
                    string l = lookup(n).Result;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\"{l}\" found!");
                    Globals.SYNPROFILER.profile.Add(($"{n}:{l}"));
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
            Console.WriteLine("\nDone! Press any key to continue..."); // "Where's the any key?" -Homer Simpson
            Console.ReadKey();
        }
        public static Task<string> lookup(string lookup)
        {
            string t = null;
            foreach (string l in Globals.SYNPROFILER.manifest)
            {
                if (l.Split(':')[0] == lookup)
                {
                    t = l.Split(':')[1].Trim(); // If a translation exists, temporarily assign it to 't'
                }
            }
            if (String.IsNullOrWhiteSpace(t))
            {
                t = lookup; // Return the lookup if it can't find a translation
            }
            return Task.FromResult(t); // Return the translation
        }


        public static Task<string> parseSubmenu(string[] submenu)
        {
            string menu = "";
            string title = "";
            bool EOF = false;
            foreach (string l in submenu)
            {
                switch (l.Split(':')[0])
                {
                    case "TITLE": // Should eventually make the header and footer center and change depending on the content's line length
                        title = l.Split(':')[1].Trim();
                        menu += $"#### {l.Split(':')[1].Trim()} ####\n\n";
                        break;
                    case "CONTENT":
                        menu += $"{l.Split(':')[1].Trim()}\n";
                        EOF = true;
                        break;
                    default:
                        if (EOF) menu += $"{l}\n";
                        break;
                }
            }
            if (!String.IsNullOrWhiteSpace(title)) menu += $"\n#### {title} ####\n\n";
            if (String.IsNullOrWhiteSpace(menu)) menu = "ERROR: Empty or malformed submenu";

            return Task<string>.FromResult(menu);
        }
    }
}
