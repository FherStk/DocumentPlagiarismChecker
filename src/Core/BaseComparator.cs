/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    /// <summary>
    /// Contains the Comparator schema that every Comparator object must inherit in order to work as expected.
    /// </summary>
    /// <typeparam name="T">The comprare's document type.</typeparam>
    internal abstract class BaseComparator<T> where T: BaseDocument{
        /// <summary>
        /// The left-side document that will be compared.
        /// </summary>
        public T Left{get; protected set;}

        /// <summary>
        /// The right-side document that will be compared.
        /// </summary>
        public T Right{get; protected set;}

        /// <summary>
        /// The right-side document that will be compared.
        /// </summary>
        public T Sample{get; protected set;}

        /// <summary>
        /// Instantiates a new Comparator, creating the document objects for the given file paths (left and right).
        /// </summary>
        /// <param name="leftFilePath">The file's path (left-side of the comparisson).</param>
        /// <param name="rightFilePath">The file's path (right-side of the comparisson).</param>
        /// <param name="sampleFilePath">The sample's path (its content will be used for ignore some comparissons).</param>
        protected BaseComparator(string leftFilePath, string rightFilePath, string sampleFilePath=null){          
            this.Left = (T)Activator.CreateInstance(typeof(T), leftFilePath);  
            this.Right = (T)Activator.CreateInstance(typeof(T), rightFilePath);

            if(!string.IsNullOrEmpty(sampleFilePath)) 
                this.Sample = (T)Activator.CreateInstance(typeof(T), sampleFilePath);
        }

        /// <summary>        
        /// Runs the Comparator and check (whatever you want to code) between the left and right files.
        /// <returns>The matching score and its details.</returns>
        /// </summary>
        public abstract ComparatorMatchingScore Run();            
    }
}