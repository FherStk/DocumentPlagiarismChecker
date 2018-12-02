using System.Linq;
using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    /// <summary>
    /// Contains the global comparer matching score (%) with its detailed data.
    /// </summary>
    public class ComparerMatchingScore{       
        /// <summary>
        /// The comparer's name.
        /// </summary>
        public string Comparer {get; private set;}              

        /// <summary>
        /// The caption row used in order to display the details of the comparisson.
        /// </summary>
        public string[] DetailsCaption {get; set;}

        /// <summary>
        /// The details row used in order to display the details of the comparisson.
        /// </summary>
        public List<string[]> DetailsData {get; set;} 

        /// <summary>
        /// The matching score between [0,1].
        /// </summary>
        /// <value></value>
        public float Matching {
            get{                
                return (_matching.Count == 0 ? 0 : _matching.Sum(x => x)/_matching.Count);
            }            
        }  
        private List<float> _matching;
           
        /// <summary>
        /// Adds a match socre to the global amount.
        /// </summary>
        /// <param name="match">The match score to add (a number between [0,1])</param>
        public void AddMatch(float match){
            if(match < 0 || match > 1)
                throw new MatchValueNotValid();
            
            _matching.Add(match);
        }

        /// <summary>
        /// Instantiates a new comparer matching socre object.
        /// </summary>
        /// <param name="comparer">The comparer's name.</param>
        public ComparerMatchingScore(string comparer){
            _matching = new List<float>();
            this.DetailsData = new List<string[]>();
            this.Comparer = comparer;                        
        }
    }      
}