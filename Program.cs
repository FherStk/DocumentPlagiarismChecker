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

            List<FileMatchingScore> results = API.CompareFiles(folderPath, fileExtension);            
            API.WriteOutput(results);        
            
            /*                      
                PENDING:                                
                    It will be possible to setup some configuration inside an XML file (for example, kind of output and number of check plugins to use (all by default)).                    
                    It will be possible to send a layout file in order to exclude its content from the comparisson.
                    It will be possible to set the threshold for each comparer in order to alert if its exceeded.
                    Paragraph word counter plugin.
             */
        }

        
    }
}
