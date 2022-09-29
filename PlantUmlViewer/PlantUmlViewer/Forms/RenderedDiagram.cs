using System.Collections.Generic;

using Svg;

namespace PlantUmlViewer.Forms
{
    public class RenderedDiagram
    {
        public List<SvgDocument> Pages { get; } = new List<SvgDocument>();
    }
}
