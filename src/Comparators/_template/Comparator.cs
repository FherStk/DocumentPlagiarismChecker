/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using DocumentPlagiarismChecker.Scores;
namespace DocumentPlagiarismChecker.Comparators._template
{
    internal class Comparator: Core.BaseComparator<Document>
    {  
        /// <summary>
        /// Instantiates a new Comparator, creating the document objects for the given file paths (left and right).
        /// </summary>
        /// <param name="fileLeftPath">The left side file's path.</param>
        /// <param name="fileRightPath">The right side file's path.</param>
        /// <param name="settings">The settings instance that will use the comparator.</param>
        public Comparator(string fileLeftPath, string fileRightPath, Settings settings): base(fileLeftPath, fileRightPath, settings){
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