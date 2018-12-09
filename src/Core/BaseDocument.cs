/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System.IO;

namespace DocumentPlagiarismChecker.Core
{
    /// <summary>
    /// Contains the document schema that every docuemnt object must inherit in order to work as expected.
    /// </summary>
    internal abstract class BaseDocument{
        /// <summary>
        /// The document name (matches with the loaded file name).
        /// </summary>
        public string Name {get; protected set;}        

        /// <summary>
        /// Instantiates a new document object.
        /// </summary>
        /// <param name="filePath"></param>
        protected BaseDocument(string filePath){
            //Check pre-conditions
            if(!File.Exists(filePath)) 
                throw new FileNotFoundException();

            this.Name = System.IO.Path.GetFullPath(filePath);
        }       
    }
}