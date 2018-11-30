using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    internal abstract class BaseWorker<T> where T: BaseDocument{
        protected ResultHeader ResultHeader{get; private set;}
        public T Left{get; protected set;}
        public T Right{get; protected set;}

        protected BaseWorker(string leftFilePath, string rightFilePath){          
            this.Left = (T)Activator.CreateInstance(typeof(T), leftFilePath);  
            this.Right = (T)Activator.CreateInstance(typeof(T), rightFilePath);  
            this.ResultHeader = new ResultHeader(this.GetType().ToString(), Path.GetFileName(leftFilePath),  Path.GetFileName(rightFilePath));
        }

        /// <summary>
        /// Runs the comparer and sends the results to the output.
        /// </summary>        
        public abstract void Run();            
    }
}