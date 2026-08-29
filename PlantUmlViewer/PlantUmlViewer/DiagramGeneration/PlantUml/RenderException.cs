using System;

namespace PlantUmlViewer.DiagramGeneration.PlantUml
{
    public class RenderException : Exception
    {
        public string Code { get; }

        public RenderException(string code, string error)  : base(error)
        {
            Code = code;
        }

        public override string Message => $"{base.Message}, Code={Code}";
    }
}
