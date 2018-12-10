/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System.Collections.Generic;
using System.Linq;

namespace DocumentPlagiarismChecker.Core
{
    /// <summary>
    /// Contains the pair of files matching score (%) with all its Comparators score data.
    /// </summary>
    public class FileMatchingScore{
        /// <summary>
        /// The file name.
        /// </summary>
        public string FileName {get; private set;}        
        
        /// <summary>
        /// The set of all the matching results, regarding the current pair of files, that has been produced by all the different Comparators used.
        /// </summary>
        public List<ComparatorMatchingScore> ComparatorResults{get; set;}

        /// <summary>
        /// The global matching score between [0,1].
        /// </summary>
        /// <value></value>
        public float Matching {
            get{
                 return (ComparatorResults.Count == 0 ? 0 : ComparatorResults.Sum(x => x.Matching)/ComparatorResults.Count);
            }
        }
        
        /// <summary>
        /// Instantiates a new file matching socre object.
        /// </summary>
        /// <param name="fileName">The file's name.</param>
        public FileMatchingScore(string fileName){
            this.FileName = fileName;
            this.ComparatorResults = new List<ComparatorMatchingScore>();
        }             
    }      
}