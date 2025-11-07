using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using YamlDotNet.Serialization;

namespace CustomRetoolFilter
{
    internal class Program
    {
        private class AppConfig
        {
            public string RetoolPath { get; set; } = @"F:\Xogos\Retool\retool.py";
            public string DatOutputDirectory { get; set; } = @"F:\Xogos\Dats\Filtrados";
            public string ExcludedDatDirectoryName { get; set; } = @"Borrar";
        }

        private static AppConfig Config { get; set; } = new();

        private static string ConfigFilePath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml");

        public static void Main(string[] args)
        {
            Config = GetAppConfigFromFile();

            while (true)
            {
                Console.WriteLine("Menú Principal:");
                Console.WriteLine("1. Filtrar arquivo dat");
                Console.WriteLine("2. Actualizar clone lists de Retool");
                Console.WriteLine("3. Abrir arquivo de configuración do programa");
                Console.WriteLine("4. Abrir arquivo de configuración xeral de Retool");
                Console.WriteLine("5. Abrir interfaz de usuario de Retool");
                Console.WriteLine("6. Saír");
                Console.WriteLine("");
                Console.Write("Escolle unha opción: ");
                string? option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        FiltrarArquivoDat();
                        break;
                    case "2":
                        UpdateRetoolCloneLists();
                        break;
                    case "3":
                        OpenAppConfigFile();
                        break;
                    case "4":
                        OpenRetoolConfigFile();
                        break;
                    case "5":
                        OpenRetoolGUI();
                        break;
                    case "6":
                        Console.WriteLine("Pois ata logo.");
                        return;
                    default:
                        Console.WriteLine("Opción non válida. Introduce unha opción válida.");
                        break;
                }

                Console.WriteLine();
            }
        }

        private static void FiltrarArquivoDat()
        {
            Config = GetAppConfigFromFile();

            Console.Write("Introduce a ruta ao arquivo dat: ");
            string? datFilePath = Console.ReadLine();

            string defaultSystemName = "";
            if (!string.IsNullOrEmpty(datFilePath) && File.Exists(datFilePath))
            {
                XmlDocument xmlDoc = new();
                xmlDoc.Load(datFilePath);
                defaultSystemName = xmlDoc.SelectSingleNode("//header/name")?.InnerText ?? "";
            }

            Console.Write($"Introduce o nome do sistema{(string.IsNullOrEmpty(defaultSystemName) ? "" : $" ou déixao en branco para aceptar o nome proposto [{defaultSystemName}]")}: ");
            string? systemName = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(systemName))
                systemName = defaultSystemName;

            ProcessStartInfo retoolStartInfo = new("python")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                WorkingDirectory = Path.GetDirectoryName(Config.RetoolPath),
                Arguments = $"\"{Config.RetoolPath}\" \"{datFilePath}\" --removesdat"
                    + $" --originalheader --output \"{Path.Combine(Config.DatOutputDirectory, systemName)}\""
            };

            using var retool = new Process();

            retool.StartInfo = retoolStartInfo;

            var retoolOutput = "";
            retool.OutputDataReceived += (sender, args) => retoolOutput += args.Data;

            retool.Start();
            retool.BeginOutputReadLine();
            retool.WaitForExit();
            
            int firstDatEndIndex = retoolOutput.IndexOf("</datafile>");
            
            if (firstDatEndIndex >= 0)
            {
                firstDatEndIndex += "</datafile>".Length;

                string includedTitlesDat = retoolOutput[..firstDatEndIndex];
                string excludedTitlesTempDat = retoolOutput[firstDatEndIndex..];

                string excludedDatDirectory = Path.Combine(Config.DatOutputDirectory, systemName, Config.ExcludedDatDirectoryName);
                Directory.CreateDirectory(excludedDatDirectory);

                string excludedTempDatFileName = $"{systemName} (excluídos temp) ({DateTime.Now:yyyyMMddHHmmssffff}).dat";

                File.WriteAllText(Path.Combine(excludedDatDirectory, excludedTempDatFileName), excludedTitlesTempDat);

                using var retool2 = new Process();

                retoolStartInfo.Arguments = $"\"{Config.RetoolPath}\" \"{Path.Combine(excludedDatDirectory, excludedTempDatFileName)}\" --removesdat"
                    + $" --originalheader --output \"{Path.Combine(Config.DatOutputDirectory, systemName)}\"";

                retool2.StartInfo = retoolStartInfo;

                retoolOutput = "";
                retool2.OutputDataReceived += (sender, args) => retoolOutput += args.Data;

                retool2.Start();
                retool2.BeginOutputReadLine();
                retool2.WaitForExit();

                File.Delete(Path.Combine(excludedDatDirectory, excludedTempDatFileName));

                firstDatEndIndex = retoolOutput.IndexOf("</datafile>");

                if (firstDatEndIndex >= 0)
                {
                    firstDatEndIndex += "</datafile>".Length;

                    string includedTitlesDat2 = retoolOutput[..firstDatEndIndex];
                    string excludedTitlesDat2 = retoolOutput[firstDatEndIndex..];

                    string excludedDatFileName = $"{systemName} (excluídos) ({DateTime.Now:yyyyMMddHHmmssffff}).dat";

                    File.WriteAllText(Path.Combine(excludedDatDirectory, excludedDatFileName), excludedTitlesDat2);

                    XmlDocument includedTitlesXmlDoc = new();
                    includedTitlesXmlDoc.LoadXml(includedTitlesDat);

                    XmlDocument includedTitlesXmlDoc2 = new();
                    includedTitlesXmlDoc2.LoadXml(includedTitlesDat2);

                    XmlNode includedTitlesRoot = includedTitlesXmlDoc.DocumentElement;
                    XmlNode includedTitles2Root = includedTitlesXmlDoc2.DocumentElement;

                    foreach (XmlNode gameNode in includedTitles2Root.SelectNodes("//game"))
                    {
                        includedTitlesRoot.AppendChild(gameNode.CloneNode(true));
                    }
                    
                    File.WriteAllText(Path.Combine(Config.DatOutputDirectory, systemName,
                        $"{systemName} (incluídos) ({DateTime.Now:yyyyMMddHHmmssffff}).dat"), includedTitlesXmlDoc.OuterXml);
                }
            }
        }

        private static void UpdateRetoolCloneLists()
        {
            Config = GetAppConfigFromFile();

            using var retool = new Process();

            retool.StartInfo.UseShellExecute = false;

            retool.StartInfo.WorkingDirectory = Path.GetDirectoryName(Config.RetoolPath);
            retool.StartInfo.FileName = "python";
            retool.StartInfo.Arguments = $"\"{Config.RetoolPath}\" --update";
            
            retool.Start();
            retool.WaitForExit();
        }

        private static void OpenAppConfigFile()
        {
            if (!File.Exists(ConfigFilePath))
                CreateDefaultAppConfigFile();

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = ConfigFilePath;
            process.Start();
        }

        private static void CreateDefaultAppConfigFile()
        {
            var defaultText = $"# Ruta ao executable (non GUI) de Retool, por exemplo: \"F:\\Xogos\\Retool\\retool.py\"\n"
                + $"RetoolPath: {Config.RetoolPath}\n\n"
                + $"# Ruta ao cartafol xeral de saída dos dats que se van filtrar en Retool"
                + $", por exemplo: \"F:\\Xogos\\Dats\\Filtrados\"\n"
                + $"DatOutputDirectory: {Config.DatOutputDirectory}\n\n"
                + $"# Nome dos cartafoles de descartes xerados en Retool, por exemplo: \"Borrar\"\n"
                + $"ExcludedDatDirectoryName: {Config.ExcludedDatDirectoryName}\n\n";

            File.WriteAllText(ConfigFilePath, defaultText.Trim());
        }

        private static AppConfig GetAppConfigFromFile()
        {
            if (File.Exists(ConfigFilePath))
            {                
                string serializedConfig = File.ReadAllText(ConfigFilePath);

                var deserializer = new DeserializerBuilder().Build();

                return deserializer.Deserialize<AppConfig>(serializedConfig);
            }

            return new AppConfig();
        }

        private static void OpenRetoolConfigFile()
        {
            Config = GetAppConfigFromFile();

            string? retoolDirectory = Path.GetDirectoryName(Config.RetoolPath);

            if (string.IsNullOrEmpty(retoolDirectory))
            {
                return;
            }

            string retoolConfigFilePath = Path.Combine(retoolDirectory, "config\\user-config.yaml");

            if (!File.Exists(retoolConfigFilePath))
            {
                Console.WriteLine("O arquivo de configuración de Retool non existe.");
                return;
            }

            var process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = retoolConfigFilePath;
            process.Start();
        }

        private static void OpenRetoolGUI()
        {
            Config = GetAppConfigFromFile();

            string retoolGUIPath = Config.RetoolPath.Replace("retool.py", "retoolgui.py");

            if (!File.Exists(retoolGUIPath))
            {
                Console.WriteLine("O arquivo RetoolGUI.py non existe.");
                return;
            }

            using var retool = new Process();

            retool.StartInfo.UseShellExecute = true;
            retool.StartInfo.FileName = retoolGUIPath;
            retool.Start();
        }
    }
}