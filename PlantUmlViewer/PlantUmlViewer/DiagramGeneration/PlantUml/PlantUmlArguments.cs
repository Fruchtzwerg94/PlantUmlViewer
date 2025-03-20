namespace PlantUmlViewer.DiagramGeneration.PlantUml
{
    internal class PlantUmlArguments
    {
        /// <summary>
        /// Error format
        /// </summary>
        public ErrorFormat ErrorFormat { get; set; } = ErrorFormat.Verbose;

        /// <summary>
        /// Output format
        /// </summary>
        public OutputFormat OutputFormat { get; set; } = OutputFormat.Svg;

        /// <summary>
        /// The file directory to use which may is needed for relative includes,
        /// null or empty to use the applications executable directory
        /// </summary>
        public string FileDirectory { get; set; } = null;

        /// <summary>
        /// Include a file as if '!include file' were used, also allowing pattern like '*.puml'
        /// </summary>
        public string Include { get; set; } = null;

        /// <summary>
        /// Override %filename% variable
        /// </summary>
        public string FileName { get; set; } = null;

        /// <summary>
        /// Separators between diagrams to determine where
        /// one image ends and another starts if multiple diagrams are generated
        /// </summary>
        public string Delimitor { get; set; } = "";

        /// <summary>
        /// To generate the Nth image
        /// </summary>
        public int ImageIndex { get; set; } = 0;
    }
}
