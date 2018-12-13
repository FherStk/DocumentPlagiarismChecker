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
    /// <summary>
    /// This object provides access to the functionalities for the Document Plagiarism Checker library. 
    /// </summary>
    public class API{
        private long _total;
        private long _computed;
        public List<ComparatorMatchingScore> MatchingResults {get; private set;}
        public float Progress {
            get{
                if(_total == 0 || _computed == 0) return 0f;
                else return MathF.Round((float)_computed / (float)_total, 2);
            }            
        }
    
        /// <summary>
        /// Uses the settings values for comparing a set of files between each other. 
        /// </summary>
        public void CompareFiles(){
            //Initial Checks
            if(!Directory.Exists(Settings.Instance.Get(Setting.GLOBAL_FOLDER))) 
                throw new FolderNotFoundException();

            //Initial vars. including the set of files.
            string leftFilePath = null;
            string rightFilePath = null;                   
            Dictionary<string, ComparatorMatchingScore> results = new Dictionary<string, ComparatorMatchingScore>();
            List<string> files = Directory.GetFiles(Settings.Instance.Get(Setting.GLOBAL_FOLDER), string.Format("*.{0}", Settings.Instance.Get(Setting.GLOBAL_EXTENSION)), (Settings.Instance.Get(Setting.GLOBAL_RECURSIVE) == "true" ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).Where(x => !x.Equals(Settings.Instance.Get(Setting.GLOBAL_SAMPLE))).ToList();
            List<Type> comparatorTypes = GetComparatorTypes().ToList();

            //The total combinations to calculate are the number of combinations without repetition for 2 elements over a set of N = (n over 2) = (n! / 2! (n-2)!)
            //The that total of combination, there will be performed a check for every comparator. 
            _total = (Factorial(files.Count()) / 2 * Factorial(files.Count() - 2)) * comparatorTypes.Count;
            _computed = 0;

            //Loops over each pair of files (the files must be compared between each other in a relation "1 to many").
            //TODO: the results must be stored even if the values has been computed early. So the FileMatchingScore will be related with
            //(n-1)*x where n is the number of files and x the number of comparers.
            string key = "";
            for(int i = 0; i < files.Count(); i++){                                
                leftFilePath = files.ElementAt(i);
                            
                for(int j = 0; j < files.Count(); j++){                                
                    rightFilePath = files.ElementAt(j);                                        
                    
                    //Instantiate and run every Comparator avoiding already computed ones and comparing a file with itself                    
                    key = string.Format("{0}@{1}", leftFilePath, rightFilePath);
                    if(results.ContainsKey(key)){
                        //TODO: clone the results[key] item and swap the left and right files.
                        //add it again to the results
                        //this doesn't count as computed matchign result.
                    }
                    else if(rightFilePath != leftFilePath){
                        foreach(Type t in comparatorTypes){
                            var comp = Activator.CreateInstance(t, leftFilePath, rightFilePath, Settings.Instance.Get(Setting.GLOBAL_SAMPLE));
                            MethodInfo method = comp.GetType().GetMethod("Run");
                            
                            //Once the object is instantiated, the Run method is invoked.
                            results.Add(key, (ComparatorMatchingScore)method.Invoke(comp, null));
                            _computed++;
                        }                                    
                    }
                }                                    
            }

            _total = 1;            
            this.MatchingResults = results.Values.ToList();            
        }       

        /// <summary>
        /// Writes the gioven scores to the configured outputs.
        /// </summary>
        /// <param name="results">A set of file matching scores</param>
        public void WriteOutput(){
            //TODO: must be selected by settings
            Outputs.Terminal t = new Outputs.Terminal();
            t.Write(this.MatchingResults);
        }

        /// <summary>
        /// Gets all the available Comparators.
        /// </summary>
        /// <returns>A set of Comparator's object types</returns>
        private static IEnumerable<Type> GetComparatorTypes()
        {   
            //TODO: Select plugins using a configuration file.
            return typeof(Program).Assembly.GetTypes().Where(x => x.BaseType.Name.Contains("BaseComparator") && !x.FullName.Contains("_template")).ToList();
        }

        /// <summary>
        /// Calculates the factorial for a number
        /// </summary>
        /// <param name="number">The number which factorial will be calculated.</param>
        /// <returns>The factorial for the given number</returns>
        private long Factorial(long number)
        {
            if (number <= 1) return 1;
            else return number * Factorial(number - 1);
        }
    }
}