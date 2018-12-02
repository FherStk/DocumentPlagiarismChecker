/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */
 
using System.Collections.Generic;

namespace DocumentPlagiarismChecker.Core
{
    /// <summary>
    /// Used in order to stablish the output detail level.
    /// </summary>
    internal enum OutputLevel{
        BASIC = 0,
        MATCHING = 1,
        DETAILED = 2
    }

    /// <summary>
    /// Contains the output schema that every output object must inherit in order to work as expected.
    /// </summary>
    internal abstract class BaseOutput{ 
        /// <summary>
        /// This method must be implemented in order to writes the score's content into the output with the selected detail level.
        /// </summary>
        /// <param name="results">A set of scores regarding each pair of compared files.</param>
        /// <param name="level">The detail level that will be displayed.</param>       
        public abstract void Write(List<FileMatchingScore> results, OutputLevel level = OutputLevel.BASIC);
    }
}