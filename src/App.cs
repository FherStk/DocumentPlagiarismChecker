/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DocumentPlagiarismChecker
{
    class App
    {
        static void Main(string[] args)
        {  
            Settings s = null;
            //Load the settings by path or display the help info.
            for(int i = 0; i < args.Length; i++){                
                if(args[i].StartsWith("--settings")){
                    s = new Settings(args[i].Split("=")[1]);
                    break;
                }
            }
            
            //Load the default settings and check for mandatory fields.            
            if(s == null) s = new Settings("settings.yaml");           
            if(string.IsNullOrEmpty(s.Folder)) throw new Exceptions.FolderNotSpecifiedException();
            if(string.IsNullOrEmpty(s.Extension)) throw new Exceptions.FileExtensionNotSpecifiedException();            

            //Display header info.
            WriteSeparator('#');
            Console.WriteLine(typeof(App).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyProductAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine();
            Console.WriteLine(typeof(App).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyDescriptionAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine();
            Console.WriteLine("  Copyright: {0}", typeof(App).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyCompanyAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine("  License: {0}", typeof(App).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyCopyrightAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine("  Version: {0}", typeof(App).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyInformationalVersionAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine();
            Console.WriteLine("  Third party software:");
            Console.WriteLine("     iTextSharp (https://developers.itextpdf.com/downloads)by iText: under the AGPL v3 license (http://itextpdf.com/terms-of-use/).");
            Console.WriteLine("     ConsoleTables (https://github.com/khalidabuhakmeh/ConsoleTables) by Khalid Abuhakmeh: under the MIT license (https://github.com/khalidabuhakmeh/ConsoleTables/blob/master/LICENSE).");
            Console.WriteLine("     YamlDotNet (https://github.com/aaubry/YamlDotNet) by Antoine Aubry (https://github.com/aaubry): under the MIT license (https://github.com/aaubry/YamlDotNet/blob/master/LICENSE).");
            WriteSeparator('-');

            //Multi-tasking in order to display progress
            using(Api api = new Api(s)){
                Task compare = Task.Run(() => 
                    api.CompareFiles()
                );

                //Polling for progress in order to display the output
                Task progress = Task.Run(() => {
                    do{
                        Console.Write("\rLoading... {0:P2}", api.Progress);
                        System.Threading.Thread.Sleep(1000);
                    }
                    while(api.Progress < 1);                

                    Console.Write("\rLoading... {0:P2}", 1);
                    Console.WriteLine();
                    Console.WriteLine("Done!");
                    Console.WriteLine();
                    Console.WriteLine("Printing results:");
                    Console.WriteLine();
                    api.WriteOutput();
                });

                progress.Wait();
                WriteSeparator('#');
            }           
        }

        private static void WriteSeparator(char separator, bool spacing = true){
             if(spacing) Console.WriteLine();

             for(int i = 0; i < Console.WindowWidth; i++)
                Console.Write(separator);

            if(spacing) Console.WriteLine();
        }
    }
}
