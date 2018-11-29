using System.IO;
using System.Linq;
using System.Collections.Generic;
using PdfPlagiarismChecker.Core;

namespace PdfPlagiarismChecker.Comparers._template
{
    internal class Worker: Core.BaseWorker<Document>
    {  
        /// <summary>
        /// [INFO]:     This method can be deleted if default behaviour is enough.
        /// [USE]:      Custom code in order to get the files from the "folderPath" and parse its content into your "Document" objects. 
        /// [WARNING]:  Must be overriden if your "Document" object constructor has new parameters.
        /// </summary>
        /// <param name="folderPath">The folder containing the documents to check.</param>
        /// <param name="fileExtension">The file extension of the documents to check.</param>
        /// <returns></returns>             
        protected override List<Document> Parse(string folderPath, string fileExtension){          
           return base.Parse(folderPath, fileExtension);
        }

        /// <summary>
        /// [INFO]:   This method is mandatory and must be coded in order to compare your "Document" objects with each other.
        /// </summary>
        /// <param name="input">The list of documents to check.</param>
        /// <returns></returns>
        protected override List<ResultHeader> Compare(List<Document> input){
            return null;
        }
       
        /// <summary>
        /// [INFO]:   Send the "ResultHeader" object (wich contains the result of the comparisson between folders) to the selected output. This method can be deleted if default behaviour is enough.
        /// [USE]:    Send the "ResultHeader" object (wich contains the result of the comparisson between folders) to the selected output. This method can be deleted if default behaviour is enough.
        /// </summary>
        /// <param name="results">The results of the check that will be send to the output.</param>
        /// <param name="level">Output detail level.</param>
        protected override void  SendToOutput(List<ResultHeader> results, int detail = 0){
            base.SendToOutput(results, detail);
        }        
    }   
}