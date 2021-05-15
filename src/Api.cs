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
using DocumentPlagiarismChecker.Scores;
using DocumentPlagiarismChecker.Outputs;

namespace DocumentPlagiarismChecker
{
    /// <summary>
    /// This object provides access to the functionalities for the Document Plagiarism Checker library. 
    /// </summary>
    public class Api: IDisposable{
        private long _total;
        private long _computed;          
        private bool disposed = false;
        public List<ComparatorMatchingScore> MatchingResults {get; private set;}
        public Settings Settings {get; private set;}
        public float Progress {
            get{
                if(_total == 0 || _computed == 0) return 0f;
                else return MathF.Round((float)_computed / (float)_total, 2);
            }            
        }
    
        public Api(): this("settings.yaml"){
        }

        public Api(string settingsFilePath): this(new Settings(settingsFilePath)){
        }

        public Api(Settings settings){
            this.Settings = settings;
        }

        /// <summary>
        /// Uses the settings values for comparing a set of files between each other. 
        /// </summary>
        public void CompareFiles(){
            //Initial Checks
            if(!Directory.Exists(this.Settings.Folder)) 
                throw new Exceptions.FolderNotFoundException();

            //Initial vars. including the set of files.            
            Dictionary<string, ComparatorMatchingScore> results = new Dictionary<string, ComparatorMatchingScore>();
            List<string> files = Directory.GetFiles(this.Settings.Folder, string.Format("*.{0}", this.Settings.Extension), (this.Settings.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)).Where(x => !x.Equals(this.Settings.Sample)).ToList();
            List<Type> comparatorTypes = GetComparatorTypes().ToList();
            
            _total = files.Count() * files.Count() * comparatorTypes.Count;
            _computed = 0;

            //Loops over each pair of files (the files must be compared between each other in a relation "1 to many").
            for(int i = 0; i < files.Count(); i++){                                
                string leftFilePath = files.ElementAt(i);
                            
                for(int j = 0; j < files.Count(); j++){                                
                    string rightFilePath = files.ElementAt(j);                                        
                                                                
                    //Instantiate and run every Comparator avoiding already computed ones and comparing a file with itself            
                    if(rightFilePath != leftFilePath){
                        foreach(Type t in comparatorTypes){                                                        
                            ComparatorMatchingScore cms = null;
                            string key = GetComparatorKey(rightFilePath, leftFilePath, t);

                            if(results.ContainsKey(key)){                            
                                //The existing results will be copied swapping the left and right files and reusing the already computed data.
                                ComparatorMatchingScore old = results[key];                        
                                cms = old.Copy(old.RightFileName, old.LeftFileName);                            }
                            else{
                                //New comparissons for left and right files must be performed using the current comparer.
                                var comp = Activator.CreateInstance(t, leftFilePath, rightFilePath, this.Settings);
                                MethodInfo method = comp.GetType().GetMethod("Run");
                                cms = (ComparatorMatchingScore)method.Invoke(comp, null);                                                            
                            }   
                            
                            _computed++;        
                            results.Add(GetComparatorKey(leftFilePath, rightFilePath, t), cms);                     
                        }
                    }
                }                                    
            }

            _computed = _total;
            this.MatchingResults = results.Values.ToList();            
        }               

        /// <summary>
        /// Writes the gioven scores to the configured outputs.
        /// </summary>
        /// <param name="results">A set of file matching scores</param>
        public void WriteOutput(){
            //TODO: must be selected by settings
            TerminalCopia t = new TerminalCopia(this.Settings);
            t.Write(this.MatchingResults);
        }

        /// <summary>
        /// Gets all the available Comparators.
        /// </summary>
        /// <returns>A set of Comparator's object types</returns>
        private static IEnumerable<Type> GetComparatorTypes()
        {   
            //TODO: Select plugins using a configuration file.
            return typeof(App).Assembly.GetTypes().Where(x => x.BaseType.Name.Contains("BaseComparator") && !x.FullName.Contains("_template")).ToList();
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

        private string GetComparatorKey(string leftFilePath, string rightFilePath, Type comparator){
            return string.Format("{0}#{1}@{2}", comparator.ToString(), leftFilePath, rightFilePath);
        }

         // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.                    
                }               

                // Note disposing has been done.
                disposed = true;

            }
        }
    }
}