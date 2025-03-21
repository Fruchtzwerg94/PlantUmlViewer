﻿namespace PlantUmlViewer.Settings
{
    public class PlantUmlViewerSettings
    {
        public string JavaPath { get; set; } = "";
        public string PlantUmlPath { get; set; } = "";
        public string Include { get; set; } = "";
        public decimal ExportSizeFactor { get; set; } = 1;
        public bool ExportDocument { get; set; } = true;
        public OpenExport OpenExport { get; set; } = OpenExport.Ask;
    }
}
