/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System.Linq;
using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    /// <summary>
    /// Contains the global comparator matching score (%) with its detailed data.
    /// </summary>
    public class ComparatorMatchingScore : DetailsMatchingScore{       
        /// <summary>
        /// The Comparator's name.
        /// </summary>
        public string Comparator {get; private set;}              
        
        /// <summary>
        /// Instantiates a new comparator matching socre object.
        /// </summary>
        /// <param name="Comparator">The Comparator's name.</param>
        public ComparatorMatchingScore(string Comparator): base(){
            this.Comparator = Comparator;                        
        }
    }      
}