using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode.Models
{
    public static class QrModels
    {
        public enum ModuleShape { Square, Circle, Hexagon }
        public enum EyeShape { Square, Circle, Diamond }
        public enum QrExportFormat { Svg, Png, Jpg }
        public enum QrCorrectionLevel { L, M, Q, H }
        public enum EyeFrameShape { Square, Rounded, Circle, Leaf, Point, Diamond, Double, IrregularLeft, IrregularRight, IrregularTop, IrregularBottom, Wavy, Dotted, 
            CircleInSquare, Pixelated, RoundedAll, RoundedTopRight, Blob, BlobTopRight, Skewed, CornerRect
        }
        public enum EyeCenterShape { Square, Circle, Diamond, Rounded, Point, Leaf , CornerRect }

    }

    // QrModels.cs
    public class QrCodeOptions
    {
        public string Content { get; set; } = "https://midominio.com";
        public ModuleShape ModuleShape { get; set; } = ModuleShape.Square;

        // Ojo - marco (borde)
        public EyeFrameShape EyeFrameShape { get; set; } = EyeFrameShape.Square;
        public string EyeFrameColor { get; set; } = "#000000";

        // Ojo - centro
        public EyeCenterShape EyeCenterShape { get; set; } = EyeCenterShape.Square;
        public string EyeCenterColor { get; set; } = "#000000";

        // --- COMPATIBILIDAD RETRO ---
        public EyeShape EyeShape { get; set; } = EyeShape.Square; // <= agrega esto
        public string EyeColor { get; set; } = "#000000";          // <= agrega esto

        public string ModuleColor { get; set; } = "#4a90e2";
        public string BgColor { get; set; } = "#ffffff";

        public QrCorrectionLevel CorrectionLevel { get; set; } = QrCorrectionLevel.Q;
        public int Size { get; set; } = 512;
        public string? LogoBase64 { get; set; }
        public int Quality { get; set; } = 512;
    }

}
