using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;

namespace TS3Dump
{
    class Config
    {
        public static string cfgfile = "config.ini";
        public static IniData LoadConfig()
        {
            if (File.Exists(cfgfile))
            {
                var parser = new FileIniDataParser();
                return parser.ReadFile(cfgfile);
            }
            else
            {
                CreateConfig();
                Console.WriteLine("Default config created as \"{0}\".", cfgfile);
                Console.WriteLine("Please edit it to your needs and restart this bot.");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.Exit(0);
                return new IniData();
            }
        }
        public static void CreateConfig()
        {
            var parser = new FileIniDataParser();
            var cfg = new IniData();
            cfg["server"]["address"] = "127.0.0.1";
            cfg["server"]["queryport"] = "10011";
            cfg["server"]["voiceport"] = "9987";
            cfg["server"]["login"] = "serveradmin";
            cfg["server"]["password"] = string.Empty;
            cfg["files"]["badges"] = "badges.csv";
            cfg["files"]["versions"] = "versions.csv";
            parser.WriteFile(cfgfile, cfg);
        }
    }
}
