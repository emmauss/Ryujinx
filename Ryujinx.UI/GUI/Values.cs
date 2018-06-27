using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace ImGuiNET
{
    public struct Values
    {
        public const  float ButtonWidth        = 170f;
        public const  float ButtonHeight       = 50f;
        public const  float DefaultWindowScale = 1.0f;
        public const  float SelectibleHeight   = 20.0f;
        public static float CurrentWindowScale = 1.0f;
        public static float CurrentFontScale   = 1.2f;

        public struct Color
        {
            public static Vector4 Yellow = new Vector4(1.0f, 1.0f, 0, 1.0f);
        }
    }
}
