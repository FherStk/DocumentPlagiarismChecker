
/*
    Copyright (C) 2018 Fernando Porrino Serrano.
    This software it's under the terms of the GNU Affero General Public License version 3.
    Please, refer to (https://github.com/FherStk/DocumentPlagiarismChecker/blob/master/LICENSE) for further licensing details.
 */

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using iTextSharp.text.pdf.parser;

namespace DocumentPlagiarismChecker.Utils
{
    /// <summary>
    /// Source: https://stackoverflow.com/questions/8846653/how-to-get-the-particular-paragraph-in-pdf-file-using-itextsharp-in-c
    /// </summary>
    public class TextAsParagraphsExtractionStrategy : ITextExtractionStrategy {
        //Text buffer
        private StringBuilder result = new StringBuilder();

        //Store last used properties
        private Vector lastBaseLine;

        //Buffer of lines of text and their Y coordinates. NOTE, these should be exposed as properties instead of fields but are left as is for simplicity's sake
        private List<string> strings = new List<String>();
        private List<float> baselines = new List<float>();

        public List<string> Paragraphs {get; private set;}

        //This is called whenever a run of text is encountered
        public void RenderText(iTextSharp.text.pdf.parser.TextRenderInfo renderInfo) {
            //This code assumes that if the baseline changes then we're on a newline
            Vector curBaseline = renderInfo.GetBaseline().GetStartPoint();

            //See if the baseline has changed
            if ((this.lastBaseLine != null) && (curBaseline[Vector.I2] != lastBaseLine[Vector.I2])) {
                //See if we have text and not just whitespace
                if ((!String.IsNullOrWhiteSpace(this.result.ToString()))) {
                    //Mark the previous line as done by adding it to our buffers
                    this.baselines.Add(this.lastBaseLine[Vector.I2]);
                    this.strings.Add(this.result.ToString());
                }

                //Reset our "line" buffer
                this.result.Clear();
            }

            //Append the current text to our line buffer
            this.result.Append(renderInfo.GetText());

            //Reset the last used line
            this.lastBaseLine = curBaseline; 
        }

        public string GetResultantText() {
            //One last time, see if there's anything left in the buffer
            if ((!String.IsNullOrWhiteSpace(this.result.ToString()))) {
                this.baselines.Add(this.lastBaseLine[Vector.I2]);
                this.strings.Add(this.result.ToString());

                //Copyright (C) 2018 Fernando Porrino Serrano.
                //START 18-12-2018: grouping paragraphs
                this.Paragraphs = new List<string>();
                Dictionary<float, int> spacing = new Dictionary<float, int>();
                for (int i = 1; i < strings.Count; i++) {
                    float space = MathF.Round(this.baselines[i-1] - this.baselines[i], 1);
                    if(!spacing.ContainsKey(space)) spacing.Add(space, 0);
                    spacing[space] += 1;  
                }

                //Regular spacing (inter-line) will be assumed as the most usual
                float regular = spacing.Where(x => x.Value == spacing.Max(y => y.Value)).FirstOrDefault().Key;
                string p = string.Empty;

                for (int i = 1; i < strings.Count; i++) {
                    float space = MathF.Round(this.baselines[i-1] - this.baselines[i], 1);
                    p = string.Format("{0} {1}", p, this.strings[i-1]);

                    if(space > regular) {
                        p = p.Trim();
                        if(p.Length > 0) this.Paragraphs.Add(p);

                        p = string.Empty;
                    }
                }
                
                //Adding the last line (the loop skiped it)
                p = string.Format("{0} {1}", p, this.strings.Last()).Trim();
                if(p.Length > 0) this.Paragraphs.Add(p);
                //END 18-12-2018: grouping paragraphs
                
            }

            //We're not going to use this method to return a string, instead after callers should inspect this class's strings and baselines fields.
            return null;
        }

        //Not needed, part of interface contract
        public void BeginTextBlock() { }
        public void EndTextBlock() { }
        public void RenderImage(ImageRenderInfo renderInfo) { }
    }
}