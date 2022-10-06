using System.Collections.Generic;

using Svg;

namespace PlantUmlViewer.DiagramGeneration
{
    public class GeneratedDiagram
    {
        public List<SvgDocument> Pages { get; }

        public GeneratedDiagram()
        {
            Pages = new List<SvgDocument>();
        }

        public GeneratedDiagram(params SvgDocument[] pages)
        {
            Pages = new List<SvgDocument>(pages);
        }
    }
}
