/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using DocumentPlagiarismChecker.Core;
namespace DocumentPlagiarismChecker.Scores
{
    /// <summary>
    /// Contains the mathing score full details.
    /// </summary>
    public class DetailsMatchingScore : BaseMatchingScore{                       
        /// <summary>
        /// Child-level details
        /// </summary>
        /// <value></value>
        public DetailsMatchingScore Child {get; set;}                                   

        /// <summary>
        /// Instantiates a new details matching socre object.
        /// </summary>
        public DetailsMatchingScore(DisplayLevel displayLevel = Core.DisplayLevel.FULL): base(displayLevel){                        
        }
    }      
}