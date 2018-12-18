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
    /// Contains the score schema that every score object must inherit in order to work as expected.
    /// </summary>
    public class BaseMatchingScore{         
        /// <summary>
        /// The output display level will be compared with this one in order to determine if the details data must be shown.
        /// </summary>
        /// <value></value>
        public DisplayLevel DisplayLevel {get; private set;}               

        /// <summary>
        /// The caption row used in order to display the details of the comparisson (same length and same order as data and match details)
        /// </summary>
        public string[] DetailsCaption {get; set;}

        /// <summary>
        /// The format used in order to display each details row of the comparisson (same length and same order as caption and match details)
        /// </summary>
        public string[] DetailsFormat {get; set;} 

        /// <summary>
        /// The details row used in order to display the details of the comparisson (same length and same order as caption and match details)
        /// </summary>
        public List<object[]> DetailsData {get; set;}       

        /// <summary>
        /// The match set used to compute the matching field (same length and same order as caption and data details)
        /// </summary>
        public List<float> DetailsMatch {get; protected set;} 

        /// <summary>
        /// The matching score between [0,1].
        /// </summary>
        /// <value></value>
        public float Matching {
            get{                
                return (DetailsMatch.Count == 0 ? 0 : DetailsMatch.Sum(x => x)/DetailsMatch.Count);
            }            
        }  
           
        /// <summary>
        /// Adds a match socre to the global amount.
        /// </summary>
        /// <param name="match">The match score to add (a number between [0,1])</param>
        public void AddMatch(float match){
            if(match < 0 || match > 1)
                throw new Exceptions.MatchValueNotValid();
            
            DetailsMatch.Add(match);
        }

        /// <summary>
        /// Instantiates a new details matching socre object.
        /// </summary>
        public BaseMatchingScore(DisplayLevel displayLevel = Core.DisplayLevel.FULL){            
            if(displayLevel < Core.DisplayLevel.COMPARATOR) throw new Exceptions.DisplayLevelNotAllowed();
            else{
                this.DisplayLevel = displayLevel;
                this.DetailsData = new List<object[]>();
                this.DetailsMatch = new List<float>();         
            }            
        }
    }      
}