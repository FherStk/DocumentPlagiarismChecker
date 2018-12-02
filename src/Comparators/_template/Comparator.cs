using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparators._template
{
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Instantiates a new Comparator, creating the document objects for the given file paths (left and right).
        /// </summary>
        /// <param name="leftFilePath"></param>
        /// <param name="rightFilePath"></param>
        public Comparator(string fileLeftPath, string fileRightPath): base(fileLeftPath, fileRightPath){
        }  
               
        /// <summary>        
        /// Runs the Comparator and check (whatever you want to code) between the left and right files.
        /// <returns>The matching score and its details.</returns>
        /// </summary>
        public override ComparatorMatchingScore Run(){          
           return null;
        }          
    }   
}