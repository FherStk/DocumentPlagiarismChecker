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
            if(args.Length == 0){
                Console.WriteLine("Please, specify the required arguments, for further details use 'dotnet run --info'.");
                return;
            }

            string folder = null, ext = null, sample = null;
            for(int i = 0; i < args.Length; i++){
                switch(args[i]){
                    case "--info":
                        Help();
                        return;
                    
                    case "--folder":
                        folder = args[i+1];
                        i++;
                        break;

                    case "--extension":
                        ext = args[i+1];
                        i++;
                        break;
                    
                    case "--sample":
                        sample = args[i+1];
                        i++;
                        break;
                }
            }

            if(string.IsNullOrEmpty(folder)) throw new FolderNotSpecifiedException();
            if(string.IsNullOrEmpty(ext)) ext = "pdf"; //throw new FileExtensionNotSpecifiedException();  //TODO: this will be mandatory when more file types become available for comparing.            
            API.WriteOutput(API.CompareFiles(folder, ext, sample), DisplayLevel.COMPARATOR);        
        }

        private static void Help(){            
            WriteSeparator('#');

            Console.WriteLine(typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyProductAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine();
            Console.WriteLine(typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyDescriptionAttribute)).SingleOrDefault().ConstructorArguments[0].Value);
            Console.WriteLine();
            Console.WriteLine(string.Format("  Copyright: {0}", typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyCompanyAttribute)).SingleOrDefault().ConstructorArguments[0].Value));
            Console.WriteLine(string.Format("  License: {0}", typeof(Program).Assembly.GetCustomAttributesData().Where(x => x.AttributeType == typeof(AssemblyCopyrightAttribute)).SingleOrDefault().ConstructorArguments[0].Value));
            Console.WriteLine(string.Format("  Version: {0}",typeof(Program).Assembly.GetName().Version.ToString()));
            
            WriteSeparator('-');

            Console.WriteLine("Usage: Run the application with 'dotnet run' with the following arguments:");
            Console.WriteLine();
            Console.WriteLine("  --folder: the absolute path to the folder containing the documents that must be compared.");
            Console.WriteLine("  --sample: the absolute path to the sample file, results matching the content of this file will be ommited (like parts of homework statements).");
            Console.WriteLine("  --extension [OPTIONAL]: files with other extensions inside the folder will be omited.");

            WriteSeparator('-');

            Console.WriteLine("Example (Windows):");
            Console.WriteLine("  dotnet run --folder \"C:\\test\" --sample \"C:\\test\\sample.pdf\"");
            Console.WriteLine();
            Console.WriteLine("Example (Linux):");
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
