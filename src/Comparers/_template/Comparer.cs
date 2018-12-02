using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparers._template
{
    internal class Comparer: Core.BaseComparer<Document>
    {  
        /// <summary>
        /// Instantiates a new comparer, creating the document objects for the given file paths (left and right).
        /// </summary>
        /// <param name="leftFilePath"></param>
        /// <param name="rightFilePath"></param>
        public Comparer(string fileLeftPath, string fileRightPath): base(fileLeftPath, fileRightPath){
        }  
               
        /// <summary>        
        /// Runs the comparer and check (whatever you want to code) between the left and right files.
        /// <returns>The matching score and its details.</returns>
        /// </summary>
        public override ComparerMatchingScore Run(){          
           return null;
        }          
    }   
}