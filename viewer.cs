namespace SYNCORE
{
    public class viewer
    {
        static string t = "";
        public static void displayMenu(List<String> menu)
        {
            while (Globals.SYNPROFILER.displaying)
            {
                List<String> pm = [];
                Console.Clear();
                int n = 1;
                string input = "";
                t = "";
                Console.WriteLine("#### Select an Entry ####\n");

                foreach (string i in menu)
                {
                    bool sm = false;
                    if (i.Split(':')[0].Split('_').Length > 2)
                    {
                        sm = true;
                    }
                    if (!sm)
                    {
                        pm.Add(i);
                        Console.WriteLine($"{n}. {pm[n - 1].Split(':')[1]}");
                        n++;
                    }
                }
                Console.WriteLine("0. Back\n");
                Console.WriteLine("#### Select an Entry ####");
                while (string.IsNullOrWhiteSpace(input))
                {
                    int i;
                    Console.Write("> ");
                    input = Console.ReadLine();
                    int.TryParse(input, out i);
                    if (i > n - 1) input = null;
                }
               
                Globals.SYNPROFILER.displaying = false;
                if (input == "0") { return; }
                t = pm[int.Parse(input) - 1].Split(':')[1].Trim();
                displayMenu(menu, pm[int.Parse(input) - 1].Split(':')[0].Split('_')[1]);
            }
        }
        public static void displayMenu(List<String> menu, string parent)
        {

            Globals.SYNPROFILER.displaying = true;
            Console.Clear();
            while (Globals.SYNPROFILER.displaying)
            {
                string input = "";
                List<String> tc = [];
                int n = 1;

                Console.WriteLine($"#### Select an Entry ({t}) ####\n");
                foreach (string i in menu)
                {
                    bool v = true;
                    if (i.Split(':')[0].Split('_')[1] != parent || i.Split(':')[0].Split('_', 2)[1] == parent) // Filters children.
                    {
                        v = false;
                    }
                    if (v)
                    {
                        tc.Add(i);
                        Console.WriteLine($"{n}. {i.Split(':')[1]}");
                        n++;
                    }
                }
                Console.WriteLine("0. Back\n");
                Console.WriteLine($"#### Select an Entry ({t}) ####\n");
                while (String.IsNullOrWhiteSpace(input))
                {
                    int i;
                    Console.Write("> ");
                    input = Console.ReadLine();
                    int.TryParse(input, out i);
                    if (i > n - 1) input = null;
                }

                if (input == "0")
                {
                    displayMenu(menu);
                    break;
                }
                Console.Clear();
                string o = parser.parseSubmenu(File.ReadAllLines(Path.Combine(Globals.SYNPROFILER.tempPath, tc[int.Parse(input) - 1].Split(':')[0]))).Result;
                if (o.Split(':')[0] == "ERROR") Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(o);
                Console.ForegroundColor = ConsoleColor.White;

            }
        }
        public static void sortMenu() // The 'profile' list's contents now appear in an order that mirrors the manifest
        {
            List<string> newProfile = [];
            foreach (string s in Globals.SYNPROFILER.manifest)
            {
                if (!string.IsNullOrWhiteSpace(s) && s[0] == 'M')
                {
                    int c = 0;
                    foreach (string p in Globals.SYNPROFILER.profile)
                    {
                        c++;
                        if (p.Split(':')[0] == s.Split(":")[0]) newProfile.Add(p);
                    }
                }
            }
            Globals.SYNPROFILER.profile = newProfile;
        }
    }
}