/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            //Settings file must be loaded first.
            for(int i = 0; i < args.Length; i++){
                if(args[i] == "--settings"){
                    Settings.Instance.Load(args[i+1]);
                    break;
                }
            }

            //The settings can be overwriten by input arguments.
            for(int i = 0; i < args.Length; i++){
                switch(args[i]){
                    case "--info":
                        Help();
                        return;
                    
                    case "--folder":
                        Settings.Instance.Set(Setting.GLOBAL_FOLDER, args[i+1]);
                        i++;
                        break;

                    case "--extension":
                        Settings.Instance.Set(Setting.GLOBAL_EXTENSION, args[i+1]);
                        i++;
                        break;
                    
                    case "--sample":
                        Settings.Instance.Set(Setting.GLOBAL_SAMPLE, args[i+1]);
                        i++;
                        break;

                    case "--display":
                        Settings.Instance.Set(Setting.GLOBAL_DISPLAY, args[i+1]);
                        i++;
                        break;
                }
            }

            if(string.IsNullOrEmpty(Settings.Instance.Get(Setting.GLOBAL_FOLDER))) throw new FolderNotSpecifiedException();
            if(string.IsNullOrEmpty(Settings.Instance.Get(Setting.GLOBAL_EXTENSION))) throw new FileExtensionNotSpecifiedException();
            API.WriteOutput(API.CompareFiles(), Enum.Parse<DisplayLevel>(Settings.Instance.Get(Setting.GLOBAL_DISPLAY).ToUpper()));
        }

        private static void Help(){            
            WriteSeparator('#');

            Console.WriteLine(typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyProductAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine();
            Console.WriteLine(typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyDescriptionAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine();
            Console.WriteLine(string.Format("  Copyright: {0}", typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyCompanyAttribute)).SingleOrDefault().ConstructorArguments[0].Value));
            Console.WriteLine(string.Format("  License: {0}", typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyCopyrightAttribute)).SingleOrDefault().ConstructorArguments[0].Value));
            Console.WriteLine(string.Format("  Version: {0}", typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyInformationalVersionAttribute)).SingleOrDefault().ConstructorArguments[0].Value));
            
            WriteSeparator('-');

            Console.WriteLine("Usage: Run the application with 'dotnet run' with the following arguments:");
            Console.WriteLine();
            Console.WriteLine("  --display: stablished how many details will be send to the output. Accepted values are:");
            Console.WriteLine("    - basic: displays only the compared file names and the global matching percentage.");
            Console.WriteLine("    - comparator: displays previous data plus the name of each comparator used with its individual matching percentage.");
            Console.WriteLine("    - detailed: displays previous data plus some details that produced a matching result over the specified threshold value.");
            Console.WriteLine("    - full: displays previous data plus all the details used by the comparator in order to calculate its marching value.");
            Console.WriteLine();
            Console.WriteLine("  --extension: files with other extensions inside the folder will be omited. Accepted values are:");
            Console.WriteLine("    - pdf: for PDF files.");
            Console.WriteLine();
            Console.WriteLine("  --folder: the absolute path to the folder containing the documents that must be compared.");
            Console.WriteLine();
            Console.WriteLine("  --sample: the absolute path to the sample file, results matching the content of this file will be ommited (like parts of homework statements).");            
            
            

            WriteSeparator('-');

            Console.WriteLine("Examples (Windows):");
            Console.WriteLine("  dotnet run");
            Console.WriteLine("  dotnet run --folder \"C:\\test\" --sample \"C:\\test\\sample.pdf\"");
            Console.WriteLine();
            Console.WriteLine("Examples (Linux):");
            Console.WriteLine("  dotnet run");
            Console.WriteLine("  dotnet run --folder \"/home/user/test\" --sample \"/home/user/test/sample.pdf\"");
            
            WriteSeparator('#');
        }

        private static void WriteSeparator(char separator, bool spacing = true){
             if(spacing) Console.WriteLine();

             for(int i = 0; i < Console.WindowWidth; i++)
                Console.Write(separator);

            if(spacing) Console.WriteLine();
        }
    }
}
