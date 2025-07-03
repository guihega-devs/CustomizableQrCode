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
            CircleInSquare, Pixelated, RoundedAll, CornerRect, TwoCornerRect, CornerRectRadio, TwoCornerRectIn, CornerRoundOut, CornerRoundOutSP
        }
        public enum EyeCenterShape { Square, Circle, Diamond, Rounded, Point, Leaf , CornerRect, TwoCornerRect, CornerRectRadio, TwoCornerRectIn, CornerRoundOut, 
            Plus, Star, Starburst, ConcaveSquircle, DiagonalCut, TripleBar, TripleBarV, BubbleGrid, BubbleOver, BlockGrid, IrregularSquare, WavySquare
        }

        public enum QrContentType
        {
            Link,        // URL
            Text,        // Texto plano
            Email,       // Correo electrónico
            Call,        // Llamada telefónica
            SMS,         // Mensaje SMS
            VCard,       // Contacto
            WhatsApp,    // Mensaje WhatsApp
            WiFi,        // Conexión WiFi
            PDF,         // Archivo PDF
            App,         // App móvil
            Images,      // Imágenes
            Video,       // Video
            SocialMedia, // Red social
            Event,       // Evento (iCal)
            Barcode2D    // Código de barras
        }
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
        public string BgColor { get; set; } = "#f7f7f7";

        public QrCorrectionLevel CorrectionLevel { get; set; } = QrCorrectionLevel.Q;
        public int Size { get; set; }
        public string? LogoBase64 { get; set; }
        public int Quality { get; set; }
        public string ExportFormat { get; set; } = "svg";
    }

    public class QrTypeOption
    {
        public QrContentType Type { get; set; }
        public string Icon { get; set; }
        public string Tooltip { get; set; }
        public string Label { get; set; }
        public double Angle { get; set; }
    }
}
