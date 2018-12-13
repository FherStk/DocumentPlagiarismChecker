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
        /// The left side file name.
        /// </summary>
        public string LeftFileName {get; private set;}

        /// <summary>
        /// The right side file name.
        /// </summary>
        public string RightFileName {get; private set;} 
        
        /// <summary>
        /// The Comparator's name.
        /// </summary>
        public string Comparator {get; private set;}              
        
        /// <summary>
        /// Instantiates a new comparator matching socre object.
        /// </summary>
        /// <param name="leftFileName">The left side's comparisson file name.</param>
        /// <param name="rightFileName">The right side's comparisson file name.</param>
        /// <param name="Comparator">The used comparato's name.</param>
        /// <param name="detailsDisplayLevel">The display level for the details (must be lower or the same as the global settings value in order to be displayed at the output).</param>
        /// <returns></returns>
        public ComparatorMatchingScore(string leftFileName, string rightFileName, string Comparator, DisplayLevel detailsDisplayLevel = Core.DisplayLevel.DETAILED): base(detailsDisplayLevel){
            this.Comparator = Comparator;                        
            this.LeftFileName = leftFileName;
            this.RightFileName = rightFileName;            
        }

        /// <summary>
        /// Copy the current ComparatorMatchingScore but its details will be linked to the original ones (no copy for the DetailsMatchingScore items will be performed).
        /// </summary>
        /// <param name="leftFileName">The left side's comparisson file name.</param>
        /// <param name="rightFileName">The right side's comparisson file name.</param>
        /// <returns>A copy of the current item.</returns>
        public ComparatorMatchingScore Copy(string leftFileName, string rightFileName){
            return new ComparatorMatchingScore(leftFileName, rightFileName, this.Comparator, this.DisplayLevel){
                Child = this.Child,
                DetailsCaption = this.DetailsCaption,
                DetailsFormat = this.DetailsFormat,
                DetailsData = this.DetailsData,
                DetailsMatch = this.DetailsMatch
            };            
        }
    }      
}