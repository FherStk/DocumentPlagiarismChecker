/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
namespace DocumentPlagiarismChecker.Comparators._template
{
    internal class Document: Core.BaseDocument
    {                
        
        /// <summary>
        /// The document constructor is used in order to load and parse the given file path into whatever you need.
        /// </summary>
        /// <param name="filePath">The location of the file.</param>
        public Document(string filePath): base(filePath){
        }
    }
}