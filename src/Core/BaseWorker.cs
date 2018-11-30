using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PdfPlagiarismChecker.Core
{
    internal abstract class BaseWorker<T> where T: BaseDocument{

        /// <summary>
        /// Runs the comparer and sends the results to the output.
        /// </summary>
        /// <param name="folderPath">The folder containing the files to check (not recursive).</param>
        /// <param name="fileExtension">The file extension of the files to check.</param>
        public void Run(string folderPath, string fileExtension){
            List<T> docs = Parse(folderPath, fileExtension);
            List<Result> res = Compare(docs);
            SendToOutput(res);
        }

        /// <summary>
        /// Goes through all the files stored into the given path (with the given extension) and parses its content into T objects.
        /// </summary>
        /// <param name="folderPath">The folder containing the files to check (not recursive).</param>
        /// <param name="fileExtension">The file extension of the files to check.</param>
        /// <returns></returns>
        protected virtual List<T> Parse(string folderPath, string fileExtension){
            if(!Directory.Exists(folderPath)) 
                throw new FolderNotFoundException();
            
            //Loop over all the PDF files inside the folder
            List<T> res = new List<T>();
            foreach(string filePath in Directory.GetFiles(folderPath).Where(x => Path.GetExtension(x).ToLower().Equals(string.Format(".{0}", fileExtension))))
                res.Add((T)Activator.CreateInstance(typeof(T), filePath));                       

            return res;
        }        
        /// <summary>
        /// Sends the comparisson results to the default terminal output.
        /// </summary>
        /// <param name="results">A set of result objects containing the results of the comparisson.</param>
        /// <param name="detail">The output detail level.</param>
        protected virtual void  SendToOutput(List<Result> results, int detail = 0){
            Outputs.Terminal output = new Outputs.Terminal();
            output.Write(results, detail);
        }
        protected abstract List<Result> Compare(List<T> input);
    }
}