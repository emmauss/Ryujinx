using System;

namespace ARMeilleure.Translation.PTC
{
    public class PtcTranslationProgressEvent : EventArgs
    {
        public int FunctionCount { get; set; }
        public int Translated { get; set; }
        public int Rejitted { get; set; }
    }
}
