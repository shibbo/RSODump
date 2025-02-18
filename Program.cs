
namespace RSODump
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            string primaryCommand = args[0].ToLower();
            string filename = args[1];

            switch (primaryCommand)
            {
                case "info":
                    EndianBinaryReader reader = new(File.Open(filename, FileMode.Open));
                    RSO rso = new(reader);
                    rso.PrintInfo();
                    break;
                case "elf":
                    Console.WriteLine("not supported yet :(");
                    break;
            }
        }

        static void ShowHelp()
        {
            Console.WriteLine("RSODump.exe <command> file.rso");
            Console.WriteLine("Possible commands: info elf");
        }
    }
}
