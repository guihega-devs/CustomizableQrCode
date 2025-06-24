using CustomizableQrCode.Models;
using CustomizableQrCode.QrCodeRenderer;
using CustomizableQrCode.Utils;
using System.IO;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode
{
    public class QrCodeBuilder
    {
        // Usa QrCodeOptions para centralizar la mayoría de parámetros
        private QrCodeOptions options = new QrCodeOptions();

        // Parámetros que NO están en QrCodeOptions (y sí usa el renderer)
        private string _backgroundGradient = "#fff";
        private string _logoBase64;
        private QrExportFormat _exportFormat = QrExportFormat.Svg;
        private int _quality = 512;

        // Métodos de configuración (Builder pattern)
        public QrCodeBuilder WithContent(string content) { options.Content = content; return this; }
        public QrCodeBuilder WithModuleShape(ModuleShape shape) { options.ModuleShape = shape; return this; }
        public QrCodeBuilder WithModuleColor(string color) { options.ModuleColor = color; return this; }
        public QrCodeBuilder WithBgColor(string color) { options.BgColor = color; return this; }

        // OJOS - MARCO (FRAME) Y CENTRO (CENTER)
        public QrCodeBuilder WithEyeFrameShape(EyeFrameShape shape) { options.EyeFrameShape = shape; return this; }
        public QrCodeBuilder WithEyeFrameColor(string color) { options.EyeFrameColor = color; return this; }
        public QrCodeBuilder WithEyeCenterShape(EyeCenterShape shape) { options.EyeCenterShape = shape; return this; }
        public QrCodeBuilder WithEyeCenterColor(string color) { options.EyeCenterColor = color; return this; }

        public QrCodeBuilder WithEyeShape(EyeShape shape) { options.EyeShape = shape; return this; } // Para compatibilidad retro
        public QrCodeBuilder WithEyeColor(string color) { options.EyeColor = color; return this; } // Para compatibilidad retro

        public QrCodeBuilder WithBackgroundGradient(string backgroundGradient) { _backgroundGradient = backgroundGradient; return this; }
        public QrCodeBuilder WithLogoBase64(string logoBase64) { _logoBase64 = logoBase64; return this; }
        public QrCodeBuilder WithExportFormat(QrExportFormat format) { _exportFormat = format; return this; }
        public QrCodeBuilder WithCorrectionLevel(QrCorrectionLevel level) { options.CorrectionLevel = level; return this; }
        public QrCodeBuilder WithSize(int size) { options.Size = size; return this; }
        public QrCodeBuilder WithQuality(int quality) { _quality = quality; return this; }

        public QrCodeOptions BuildOptions() => options;

        public CustomQrCode Build()
        {
            // Genera la matriz QR
            var matrix = QrMatrixGenerator.GenerateMatrix(options.Content, options.CorrectionLevel);

            // Llama a tu renderer con todos los nuevos parámetros
            string svg = SvgRenderer.Render(
                matrix,
                options.ModuleShape,
                options.ModuleColor,
                options.EyeFrameShape,
                options.EyeFrameColor,
                options.EyeCenterShape,
                options.EyeCenterColor,
                _backgroundGradient,
                _logoBase64,
                options.Size // Usa Size como calidad/tamaño real del SVG
            );

            return new CustomQrCode(svg, _exportFormat);
        }
    }
}
