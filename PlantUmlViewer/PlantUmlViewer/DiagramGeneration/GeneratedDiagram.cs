using System.Collections.Generic;

using Svg;

namespace PlantUmlViewer.DiagramGeneration
{
    internal class GeneratedDiagram
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
