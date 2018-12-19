
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
    /// Modified by Fernando Porrino Serrano
    /// </summary>
    public class TextAsParagraphsExtractionStrategy : ITextExtractionStrategy {
        //Text buffer
        private StringBuilder _result = new StringBuilder();

        //Store last used properties
        private Vector _lastBaseLine;

        //Buffer of lines of text and their Y coordinates. NOTE, these should be exposed as properties instead of fields but are left as is for simplicity's sake
        private List<string> _strings = new List<String>();
        private List<float> _baselines = new List<float>();

        private bool _computed = false;
        private List<string> _paragraphs;

        public List<string> Paragraphs {
            get{               
                if(!_computed) ComputeParagraphContent();
                return _paragraphs;
            }

            private set{
                _paragraphs = value;
            }
        }

        //This is called whenever a run of text is encountered
        public void RenderText(iTextSharp.text.pdf.parser.TextRenderInfo renderInfo) {
            //This code assumes that if the baseline changes then we're on a newline
            Vector curBaseline = renderInfo.GetBaseline().GetStartPoint();

            //See if the baseline has changed
            if ((this._lastBaseLine != null) && (curBaseline[Vector.I2] != _lastBaseLine[Vector.I2])) {
                //See if we have text and not just whitespace
                if ((!String.IsNullOrWhiteSpace(this._result.ToString()))) {
                    //Mark the previous line as done by adding it to our buffers
                    this._baselines.Add(this._lastBaseLine[Vector.I2]);
                    this._strings.Add(this._result.ToString());
                }

                //Reset our "line" buffer
                this._result.Clear();
            }

            //Append the current text to our line buffer
            this._result.Append(renderInfo.GetText());

            //Reset the last used line
            this._lastBaseLine = curBaseline; 
        }

        public string GetResultantText() {
            //One last time, see if there's anything left in the buffer
            if ((!String.IsNullOrWhiteSpace(this._result.ToString()))) {
                this._baselines.Add(this._lastBaseLine[Vector.I2]);
                this._strings.Add(this._result.ToString()); 
            }            

            //We're not going to use this method to return a string, instead after callers should inspect this class's strings and baselines fields.
            this._computed = false;
            return null;
        }

        private void ComputeParagraphContent(){            
            _paragraphs = new List<string>();

            //Getting all the vertical spacings between lines in order to detect the regular one between lines inside the same paragraph.
            Dictionary<float, int> spacing = new Dictionary<float, int>();
            for (int i = 1; i < _strings.Count; i++) {                
                float space = MathF.Round(this._baselines[i-1] - this._baselines[i], 0);
                if(!spacing.ContainsKey(space)) spacing.Add(space, 0);
                spacing[space] += 1;  
            }

            //Regular spacing will be assumed as the most usual
            float regular = spacing.OrderByDescending(x => x.Value).FirstOrDefault().Key;
            float next = spacing.OrderBy(x => x.Key).Where(x => x.Key > regular).FirstOrDefault().Key;
            float errorMargin = (next - regular)/2;

            string p = string.Empty;

            //All the paragraphs will be grouped using the spacing between lines
            for (int i = 1; i < _strings.Count; i++) {
                float space = MathF.Round(this._baselines[i-1] - this._baselines[i], 0);
                p = string.Format("{0} {1}", p, this._strings[i-1].Trim());
                
                if(space < 0 || (space > regular && Math.Abs(space - regular) > errorMargin)) {
                    //New paragraph detected, so the previous sentences will be added as a single paragraph (removing also control chars and leading/trailing spaces).
                    //Notice that an error margin of ±1 is accepted
                    AddParagraph(p);
                    p = string.Empty;
                }
            }
            
            //Adding the last line (the loop skiped it)
            AddParagraph(string.Format("{0} {1}", p, this._strings.Last().Trim()));          
            this._computed = true;  
        }
        private void AddParagraph(string p){
            if(p.Length > 0) _paragraphs.Add(new string(p.Trim().Where(c =>  char.IsLetterOrDigit(c) || c.Equals('’') || (c >= ' ' && c <= byte.MaxValue)).ToArray()));
        }

        //Not needed, part of interface contract
        public void BeginTextBlock() { }
        public void EndTextBlock() { }
        public void RenderImage(ImageRenderInfo renderInfo) { }
    }
}