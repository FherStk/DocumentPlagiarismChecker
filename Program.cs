using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            //TODO: THOSE SHOULD BE LOADED BY PARAMETER INPUT
            string folderPath = "C:\\test";
            string fileExtension = "pdf";

            LoadFiles(folderPath, fileExtension);

            
            
            
            //Comparers.WordCounter.Worker wc = new Comparers.WordCounter.Worker("File1", "File2");
            //wc.Run("C:\\test", "pdf");
            //WordCounter.Document d = new WordCounter.Document("");
            //d.words.Add(null);
            
            /*    
                CURRENT:
                    The comparisson will be made following a one-to-many pattern so the result will be more ore less as follows (including details level):
                        [Level-1: basic] For each file: File name; Total file % matching.
                            [Level-2: specific] For each file compared with the previous one (one-to-many): File name; Comprarer used; File % matching.
                                [Level-3: full] For each compared used: Left side caption (header) + left side values (lines);  Right side caption (header) + right side values (lines);  Check caption (header) + check values (lines); 

                PENDING:            
                    The main program will run all the comparers one after each other and the matching % will be recalculated.
                    It will be possible to setup some configuration inside an XML file (for example, kind of output and number of check plugins to use (all by default)).                    
             */
        }

        private static void LoadFiles(string folderPath, string fileExtension){
            //Initial Checks
            if(!Directory.Exists(folderPath)) 
                throw new FolderNotFoundException();

            //Initial vars. including the set of files.
            string leftFile = null;
            string rightFile = null;              
            List<string> files = Directory.GetFiles(folderPath).Where(x => Path.GetExtension(x).ToLower().Equals(string.Format(".{0}", fileExtension))).ToList();

            //Loops over each pair of files (the files must be compared between each other in a relation "1 to many").
            for(int i = 0; i < files.Count(); i++){                                
                leftFile = files.ElementAt(i);
             
                for(int j = i+1; j < files.Count(); j++){                                
                    rightFile = files.ElementAt(j);
                    RunComparers(leftFile, rightFile);
                }                    
            }

            WriteOutput();
        }

        private static void RunComparers(string leftFilePath, string rightFilePath){
            //Creates an instance of each comparer and runs them for each pair of files.
            //TODO: pending.

            Comparers.WordCounter.Worker comp = new Comparers.WordCounter.Worker(leftFilePath, rightFilePath);
            comp.Run();
        }

        private static void WriteOutput(){
            //TODO: must be selected by settings
            Outputs.Terminal t = new Outputs.Terminal();
            t.Write(0);
        }
    }
}
