using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparers._template
{
    internal class Worker: Core.BaseWorker<Document>
    {  
        internal Worker(string fileLeftPath, string fileRightPath): base(fileLeftPath, fileRightPath){
        }  
        
        /// <summary>
        /// [INFO]:     This method can be deleted if default behaviour is enough.
        /// [USE]:      Custom code in order to get the files from the "folderPath" and parse its content into your "Document" objects. 
        /// [WARNING]:  Must be overriden if your "Document" object constructor has new parameters.
        /// <returns></returns>
        /// </summary>
        public override ComparerMatchingScore Run(){          
           return null;
        }          
    }   
}