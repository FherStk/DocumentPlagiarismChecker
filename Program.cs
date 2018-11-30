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
            //TODO: THOSE SHOULD BE LOADED BY PARAMETER INPUT
            string folderPath = "C:\\test";
            string fileExtension = "pdf";

            CompareFiles(folderPath, fileExtension);                    
            
            /*                      
                PENDING:                                
                    It will be possible to setup some configuration inside an XML file (for example, kind of output and number of check plugins to use (all by default)).                    
                    It will be possible to send a layout file in order to exclude its content from the comparisson.
                    It will be possible to set the threshold for each comparer in order to alert if its exceeded.
             */
        }

        private static void CompareFiles(string folderPath, string fileExtension){
            //Initial Checks
            if(!Directory.Exists(folderPath)) 
                throw new FolderNotFoundException();

            //Initial vars. including the set of files.
            string leftFilePath = null;
            string rightFilePath = null;                   
            List<FileMatchingScore> results = new List<FileMatchingScore>();
            List<string> files = Directory.GetFiles(folderPath).Where(x => Path.GetExtension(x).ToLower().Equals(string.Format(".{0}", fileExtension))).ToList();

            //Loops over each pair of files (the files must be compared between each other in a relation "1 to many").
            for(int i = 0; i < files.Count(); i++){                                
                leftFilePath = files.ElementAt(i);
             
                for(int j = i+1; j < files.Count(); j++){                                
                    rightFilePath = files.ElementAt(j);

                    //Create the score for the given file pair
                    FileMatchingScore fpr = new FileMatchingScore(leftFilePath, rightFilePath);

                    //Instantiate and run every comparer
                    foreach(Type t in GetComparerTypes()){
                        var comp = Activator.CreateInstance(t, leftFilePath, rightFilePath);
                        MethodInfo method = comp.GetType().GetMethod("Run");
                        
                        //Once the object is instantiated, the Run method is invoked.
                        ComparerMatchingScore cms = (ComparerMatchingScore)method.Invoke(comp, null);
                        fpr.ComparerResults.Add(cms);
                    }
                   
                    results.Add(fpr);
                }                    
            }

            WriteOutput(results);
        }       

        private static void WriteOutput(List<FileMatchingScore> results){
            //TODO: must be selected by settings
            Outputs.Terminal t = new Outputs.Terminal();
            t.Write(results);
        }

        private static IEnumerable<Type> GetComparerTypes()
        {   
            //TODO: Select plugins using a configuration file.
            return typeof(Program).Assembly.GetTypes().Where(x => x.BaseType.Name.Contains("BaseComparer") && !x.FullName.Contains("_template")).ToList();
        }
    }
}
