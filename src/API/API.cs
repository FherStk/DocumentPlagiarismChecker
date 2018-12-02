using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker
{
    /// <summary>
    /// This object provides access to the functionalities for the Document Plagiarism Checker library. 
    /// </summary>
    public class API{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        public static List<FileMatchingScore> CompareFiles(string folderPath, string fileExtension){
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

            return results;            
        }       

        /// <summary>
        /// Writes the gioven scores to the configured outputs.
        /// </summary>
        /// <param name="results">A set of file matching scores</param>
        public static void WriteOutput(List<FileMatchingScore> results){
            //TODO: must be selected by settings
            Outputs.Terminal t = new Outputs.Terminal();
            t.Write(results);
        }

        /// <summary>
        /// Gets all the available comparers.
        /// </summary>
        /// <returns>A set of comparer's object types</returns>
        private static IEnumerable<Type> GetComparerTypes()
        {   
            //TODO: Select plugins using a configuration file.
            return typeof(Program).Assembly.GetTypes().Where(x => x.BaseType.Name.Contains("BaseComparer") && !x.FullName.Contains("_template")).ToList();
        }
    }
}