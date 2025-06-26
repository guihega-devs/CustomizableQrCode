using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode.QrCodeRenderer
{
    /// <summary>
    /// Clase responsable de generar SVGs personalizados de códigos QR,
    /// incluyendo la personalización de módulos, marcos y centros de los ojos, colores y logos.
    /// </summary>
    public static class SvgRenderer
    {
        /// <summary>
        /// Genera el SVG completo de un código QR personalizado, permitiendo múltiples estilos para módulos y marcos de los ojos.
        /// </summary>
        /// <param name="matrix">Matriz booleana que representa los módulos del QR.</param>
        /// <param name="moduleShape">Forma de los módulos del QR.</param>
        /// <param name="moduleColor">Color de los módulos.</param>
        /// <param name="eyeFrameShape">Forma del marco de los ojos del QR.</param>
        /// <param name="eyeFrameColor">Color del marco de los ojos.</param>
        /// <param name="eyeCenterShape">Forma del centro de los ojos del QR.</param>
        /// <param name="eyeCenterColor">Color del centro de los ojos.</param>
        /// <param name="backgroundGradient">Color de fondo o gradiente (opcional).</param>
        /// <param name="logoBase64">Imagen del logo en base64 (opcional).</param>
        /// <param name="size">Tamaño en píxeles del SVG generado.</param>
        /// <returns>SVG como cadena de texto.</returns>
        public static string Render(
            bool[,] matrix,
            ModuleShape moduleShape,
            string moduleColor,
            EyeFrameShape eyeFrameShape,
            string eyeFrameColor,
            EyeCenterShape eyeCenterShape,
            string eyeCenterColor,
            string? backgroundGradient,
            string? logoBase64,
            int size
        )
        {
            int quietZone = 4;
            int modules = matrix.GetLength(0);
            double moduleSize = (double)size / modules;
            double eyeSize = moduleSize * 7;
            double innerSize = eyeSize - 2 * moduleSize;
            double centerSize = eyeSize - 4 * moduleSize;

            // Coordenadas de los tres ojos del QR (esquinas)
            var eyePositions = new (int x, int y)[]
            {
                (quietZone, quietZone),
                (modules - 7 - quietZone, quietZone),
                (quietZone, modules - 7 - quietZone)
            };

            var sb = new StringBuilder();
            sb.AppendLine($"<svg xmlns='http://www.w3.org/2000/svg' width='{size}' height='{size}' viewBox='0 0 {size} {size}'>");

            // Fondo del SVG
            if (!string.IsNullOrEmpty(backgroundGradient))
            {
                var colors = backgroundGradient.Split(',');
                if (colors.Length == 2)
                {
                    sb.AppendLine($"<defs><linearGradient id='bg' x1='0%' y1='0%' x2='100%' y2='100%'>" +
                        $"<stop offset='0%' stop-color='{colors[0]}'/>" +
                        $"<stop offset='100%' stop-color='{colors[1]}'/>" +
                        $"</linearGradient></defs>");
                    sb.AppendLine($"<rect width='{size}' height='{size}' fill='url(#bg)'/>");
                }
                else
                {
                    sb.AppendLine($"<rect width='{size}' height='{size}' fill='{colors[0]}'/>");
                }
            }
            else
            {
                sb.AppendLine($"<rect width='{size}' height='{size}' fill='#fff'/>");
            }

            // Función auxiliar: Determina si un módulo está en el área de algún ojo
            bool IsInEyeArea(int x, int y)
            {
                foreach (var (ex, ey) in eyePositions)
                {
                    if (x >= ex && x < ex + 7 && y >= ey && y < ey + 7)
                        return true;
                }
                return false;
            }

            // Renderizado de los módulos (sin superponer los ojos)
            for (int x = 0; x < modules; x++)
            {
                for (int y = 0; y < modules; y++)
                {
                    if (IsInEyeArea(x, y)) continue;
                    if (!matrix[x, y]) continue;
                    double px = x * moduleSize;
                    double py = y * moduleSize;

                    switch (moduleShape)
                    {
                        case ModuleShape.Square:
                            sb.AppendLine($"<rect x='{px}' y='{py}' width='{moduleSize}' height='{moduleSize}' fill='{moduleColor}'/>");
                            break;
                        case ModuleShape.Circle:
                            sb.AppendLine($"<circle cx='{px + moduleSize / 2}' cy='{py + moduleSize / 2}' r='{moduleSize / 2}' fill='{moduleColor}'/>");
                            break;
                        case ModuleShape.Hexagon:
                            var hex = GetHexagonPoints(px, py, moduleSize);
                            sb.AppendLine($"<polygon points='{hex}' fill='{moduleColor}'/>");
                            break;
                    }
                }
            }

            // Renderizado de los ojos del QR y sus centros
            for (int eyeIndex = 0; eyeIndex < eyePositions.Length; eyeIndex++)
            {
                var (ex, ey) = eyePositions[eyeIndex];
                double ox = ex * moduleSize;
                double oy = ey * moduleSize;
                double centerCx = ox + eyeSize / 2;
                double centerCy = oy + eyeSize / 2;

                // Determina el ángulo de rotación según el ojo (para marcos asimétricos)
                double angle = 0;
                switch (eyeIndex)
                {
                    case 0: angle = 270; break; // Superior izquierda
                    case 1: angle = 360; break; // Superior derecha
                    case 2: angle = -180; break; // Inferior izquierda
                }

                // Renderizado del marco del ojo según el estilo seleccionado
                switch (eyeFrameShape)
                {
                    case EyeFrameShape.Square:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Rounded:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' rx='{moduleSize * 1.5}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' rx='{moduleSize * 0.8}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Circle:
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{eyeSize / 2}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{innerSize / 2}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Diamond:
                        sb.AppendLine($"<path d='{GetDiamondPath(centerCx, centerCy, eyeSize / 2)}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<path d='{GetDiamondPath(centerCx, centerCy, innerSize / 2)}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Leaf:
                        sb.AppendLine(DrawLeafEye(ox, oy, eyeSize, eyeFrameColor));
                        sb.AppendLine(DrawLeafEye(ox + moduleSize, oy + moduleSize, innerSize, "#fff"));
                        break;
                    case EyeFrameShape.Point:
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{eyeSize / 2}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{innerSize / 2}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Dotted:
                        sb.AppendLine(DrawDottedEye(ox, oy, eyeSize, eyeFrameColor, moduleSize));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Double:
                        sb.AppendLine(DrawDoubleEye(ox, oy, eyeSize, eyeFrameColor, moduleSize));
                        sb.AppendLine($"<rect x='{ox + moduleSize * 2}' y='{oy + moduleSize * 2}' width='{eyeSize - 4 * moduleSize}' height='{eyeSize - 4 * moduleSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.CircleInSquare:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' rx='{moduleSize * 1.5}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{innerSize / 2}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.IrregularLeft:
                        sb.AppendLine(DrawCornerRoundedEye(ox, oy, eyeSize, eyeFrameColor, "left"));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.IrregularRight:
                        sb.AppendLine(DrawCornerRoundedEye(ox, oy, eyeSize, eyeFrameColor, "right"));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.IrregularTop:
                        sb.AppendLine(DrawCornerRoundedEye(ox, oy, eyeSize, eyeFrameColor, "top"));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.IrregularBottom:
                        sb.AppendLine(DrawCornerRoundedEye(ox, oy, eyeSize, eyeFrameColor, "bottom"));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Wavy:
                        sb.AppendLine(DrawWavyEye(ox, oy, eyeSize, eyeFrameColor));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.Pixelated:
                        sb.AppendLine(DrawPixelatedEye(ox, oy, eyeSize, eyeFrameColor));
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                    // === Estilos personalizados (esquinas rectas, redondeadas, combinadas, etc.) ===
                    case EyeFrameShape.CornerRect:
                        string marco = DrawBottomLeftSquareEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marco, centerCx, centerCy, angle));
                        string centro = DrawBottomLeftSquareEyeCenter(ox, oy, eyeSize, "#fff");
                        sb.AppendLine(WithRotation(centro, centerCx, centerCy, angle));
                        break;
                    case EyeFrameShape.TwoCornerRect:
                        int[] angles = { 90, 180, 0 };
                        double cx = ox + eyeSize / 2;
                        double cy = oy + eyeSize / 2;
                        sb.AppendLine(WithRotation(DrawTwoCornerRectEye(ox, oy, eyeSize, eyeFrameColor), cx, cy, angles[eyeIndex]));
                        sb.AppendLine(WithRotation(DrawTwoCornerRectEyeCenter(ox, oy, eyeSize, "#fff"), cx, cy, angles[eyeIndex]));
                        break;
                    case EyeFrameShape.CornerRectRadio:
                        string marcoradio = DrawBottomLeftSquareRadioEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marcoradio, centerCx, centerCy, angle));
                        string centroradio = DrawBottomLeftSquareRadioEyeCenter(ox, oy, eyeSize, "#fff");
                        sb.AppendLine(WithRotation(centroradio, centerCx, centerCy, angle));
                        break;
                    case EyeFrameShape.TwoCornerRectIn:
                        int[] anglesIn = { 0, 270, 90 };
                        double cxIn = ox + eyeSize / 2;
                        double cyIn = oy + eyeSize / 2;
                        sb.AppendLine(WithRotation(DrawTwoCornerRectInEye(ox, oy, eyeSize, eyeFrameColor), cxIn, cyIn, anglesIn[eyeIndex]));
                        sb.AppendLine(WithRotation(DrawTwoCornerRectInEyeCenter(ox, oy, eyeSize, "#fff"), cxIn, cyIn, anglesIn[eyeIndex]));
                        break;
                    case EyeFrameShape.CornerRoundOut:
                        string marcoRCO = DrawBottomLeftSquareRCOEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marcoRCO, centerCx, centerCy, angle));
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{innerSize / 2}' fill='#fff'/>");
                        break;
                    case EyeFrameShape.CornerRoundOutSP:
                        string marcoRCOSP = DrawBottomLeftSquareRCOSPEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marcoRCOSP, centerCx, centerCy, angle));
                        string centroRCOSP = DrawBottomLeftSquareRCOSPEyePupil(ox, oy, eyeSize, "#fff");
                        sb.AppendLine(WithRotation(centroRCOSP, centerCx, centerCy, angle));
                        break;
                    default:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                }

                // Renderizado del centro/pupila del ojo
                EyeCenterShape allowedShape = eyeCenterShape;
                if (eyeFrameShape == EyeFrameShape.Leaf && eyeCenterShape != EyeCenterShape.Circle && eyeCenterShape != EyeCenterShape.Leaf)
                    allowedShape = EyeCenterShape.Leaf;
                else if (eyeFrameShape == EyeFrameShape.Diamond && eyeCenterShape != EyeCenterShape.Circle && eyeCenterShape != EyeCenterShape.Diamond)
                    allowedShape = EyeCenterShape.Diamond;

                switch (allowedShape)
                {
                    case EyeCenterShape.Circle:
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{centerSize / 2}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Square:
                        sb.AppendLine($"<rect x='{centerCx - centerSize / 2}' y='{centerCy - centerSize / 2}' width='{centerSize}' height='{centerSize}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Rounded:
                        sb.AppendLine($"<rect x='{centerCx - centerSize / 2}' y='{centerCy - centerSize / 2}' width='{centerSize}' height='{centerSize}' rx='{centerSize * 0.25}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Point:
                        sb.AppendLine($"<circle cx='{centerCx}' cy='{centerCy}' r='{centerSize * 0.28}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Diamond:
                        sb.AppendLine($"<path d='{GetDiamondPath(centerCx, centerCy, centerSize / 2)}' fill='{eyeCenterColor}'/>");
                        break;
                    case EyeCenterShape.Leaf:
                        sb.AppendLine(DrawLeafEye(centerCx - centerSize / 2, centerCy - centerSize / 2, centerSize, eyeCenterColor));
                        break;
                    case EyeCenterShape.CornerRect:
                        string pupil = DrawBottomLeftSquareEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupil, centerCx, centerCy, angle));
                        break;
                    case EyeCenterShape.TwoCornerRect:
                        int[] angles = { 90, 180, 0 };
                        double cx = ox + eyeSize / 2;
                        double cy = oy + eyeSize / 2;
                        string pupila = DrawTwoCornerRectEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupila, cx, cy, angles[eyeIndex]));
                        break;
                    case EyeCenterShape.CornerRectRadio:
                        string pupilRadio = DrawBottomLeftSquareRadioEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupilRadio, centerCx, centerCy, angle));
                        break;
                    case EyeCenterShape.TwoCornerRectIn:
                        int[] anglesIn = { 0, 270, 90 };
                        double cxIn = ox + eyeSize / 2;
                        double cyIn = oy + eyeSize / 2;
                        string pupilaIn = DrawTwoCornerRectInEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupilaIn, cxIn, cyIn, anglesIn[eyeIndex]));
                        break;
                    case EyeCenterShape.CornerRoundOut:
                        string pupilCRO = DrawBottomLeftSquareRCOEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupilCRO, centerCx, centerCy, angle));
                        break;
                }
            }

            // Logo central (opcional)
            if (!string.IsNullOrWhiteSpace(logoBase64))
            {
                double logoSize = size * 0.22;
                double logoX = (size - logoSize) / 2;
                double logoY = (size - logoSize) / 2;
                sb.AppendLine($"<image href='{logoBase64}' x='{logoX}' y='{logoY}' width='{logoSize}' height='{logoSize}' style='pointer-events:none;' />");
            }

            sb.AppendLine("</svg>");
            return sb.ToString();
        }

        /// <summary>
        /// Calcula los puntos para dibujar un hexágono en SVG.
        /// </summary>
        /// <param name="x">Coordenada X de la esquina superior izquierda.</param>
        /// <param name="y">Coordenada Y de la esquina superior izquierda.</param>
        /// <param name="size">Tamaño del hexágono.</param>
        /// <returns>Cadena con los puntos SVG.</returns>
        private static string GetHexagonPoints(double x, double y, double size)
        {
            double dx = size / 2.0;
            double dy = size / 2.0;
            double r = size / 2.0;
            var points = new List<(double X, double Y)>();
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 3.0 * i - Math.PI / 6.0;
                double px = x + dx + r * Math.Cos(angle);
                double py = y + dy + r * Math.Sin(angle);
                points.Add((px, py));
            }
            return string.Join(" ", points.Select(p => $"{p.X},{p.Y}"));
        }

        /// <summary>
        /// Genera el path SVG para un diamante (rombo).
        /// </summary>
        /// <param name="cx">Centro X.</param>
        /// <param name="cy">Centro Y.</param>
        /// <param name="r">Radio (distancia desde el centro a los vértices).</param>
        /// <returns>Cadena path SVG.</returns>
        private static string GetDiamondPath(double cx, double cy, double r)
        {
            return $"M{cx},{cy - r} L{cx + r},{cy} L{cx},{cy + r} L{cx - r},{cy} Z";
        }

        /// <summary>
        /// Dibuja un "ojo hoja" (Leaf eye) para QR personalizados.
        /// </summary>
        private static string DrawLeafEye(double x, double y, double size, string color)
        {
            var cx = x + size / 2;
            var cy = y + size / 2;
            var r = size / 2;
            return $"<path d='M{cx} {cy - r} Q {cx + r} {cy} {cx} {cy + r} Q {cx - r} {cy} {cx} {cy - r} Z' fill='{color}'/>";
        }

        /// <summary>
        /// Dibuja un marco punteado alrededor de un ojo.
        /// </summary>
        private static string DrawDottedEye(double x, double y, double size, string color, double moduleSize)
        {
            int dots = 14;
            double r = moduleSize * 0.38;
            var sb = new StringBuilder();
            for (int i = 0; i < dots; i++)
            {
                double px = x + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{px}' cy='{y}' r='{r}' fill='{color}'/>");
            }
            for (int i = 0; i < dots; i++)
            {
                double px = x + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{px}' cy='{y + size}' r='{r}' fill='{color}'/>");
            }
            for (int i = 1; i < dots - 1; i++)
            {
                double py = y + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{x}' cy='{py}' r='{r}' fill='{color}'/>");
            }
            for (int i = 1; i < dots - 1; i++)
            {
                double py = y + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{x + size}' cy='{py}' r='{r}' fill='{color}'/>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja un ojo doble (dos bordes) para QR personalizados.
        /// </summary>
        private static string DrawDoubleEye(double x, double y, double size, string color, double moduleSize)
        {
            double margin = moduleSize * 0.9;
            var sb = new StringBuilder();
            sb.AppendLine($"<rect x='{x}' y='{y}' width='{size}' height='{size}' fill='none' stroke='{color}' stroke-width='{margin}'/>");
            sb.AppendLine($"<rect x='{x + margin * 1.4}' y='{y + margin * 1.4}' width='{size - margin * 2.8}' height='{size - margin * 2.8}' fill='none' stroke='{color}' stroke-width='{margin / 1.7}'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja un marco con ondas senoidales.
        /// </summary>
        private static string DrawWavyEye(double x, double y, double size, string color)
        {
            int waves = 4;
            double amplitude = size * 0.07;
            var sb = new StringBuilder();
            sb.Append($"<path d='M{x},{y + amplitude}");
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size * (t - 0.25 / waves)},{y - amplitude} {x + size * t},{y + amplitude}");
            }
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size + amplitude},{y + size * (t - 0.25 / waves)} {x + size - amplitude},{y + size * t}");
            }
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size * (1 - t + 0.25 / waves)},{y + size + amplitude} {x + size * (1 - t)},{y + size - amplitude}");
            }
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x - amplitude},{y + size * (1 - t + 0.25 / waves)} {x + amplitude},{y + size * (1 - t)}");
            }
            sb.Append(" Z' fill='" + color + "'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja una esquina del marco del ojo redondeada, según el parámetro.
        /// </summary>
        /// <param name="corner">Puede ser 'left', 'right', 'top', 'bottom'.</param>
        private static string DrawCornerRoundedEye(double x, double y, double size, string color, string corner)
        {
            double r = size * 0.25;
            var sb = new StringBuilder();

            switch (corner)
            {
                case "left":
                    sb.Append($"<path d='M{x + r},{y} ");
                    sb.Append($"L{x + size},{y} ");
                    sb.Append($"L{x + size},{y + size} ");
                    sb.Append($"L{x},{y + size} ");
                    sb.Append($"L{x},{y + r} ");
                    sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
                    sb.Append("Z' fill='" + color + "'/>");
                    break;
                case "right":
                    sb.Append($"<path d='M{x},{y} ");
                    sb.Append($"L{x + size - r},{y} ");
                    sb.Append($"A{r},{r} 0 0,1 {x + size},{y + r} ");
                    sb.Append($"L{x + size},{y + size} ");
                    sb.Append($"L{x},{y + size} ");
                    sb.Append($"Z' fill='{color}'/>");
                    break;
                case "top":
                    sb.Append($"<path d='M{x},{y} ");
                    sb.Append($"L{x + size},{y} ");
                    sb.Append($"L{x + size},{y + size} ");
                    sb.Append($"L{x + r},{y + size} ");
                    sb.Append($"A{r},{r} 0 0,1 {x},{y + size - r} ");
                    sb.Append($"Z' fill='{color}'/>");
                    break;
                case "bottom":
                    sb.Append($"<path d='M{x},{y} ");
                    sb.Append($"L{x + size},{y} ");
                    sb.Append($"L{x + size},{y + size - r} ");
                    sb.Append($"A{r},{r} 0 0,1 {x + size - r},{y + size} ");
                    sb.Append($"L{x},{y + size} ");
                    sb.Append($"Z' fill='{color}'/>");
                    break;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Dibuja el marco pixelado de un ojo de QR.
        /// </summary>
        private static string DrawPixelatedEye(double x, double y, double size, string color)
        {
            int pixels = 8;
            double pixelSize = size / (double)pixels;
            var sb = new StringBuilder();
            for (int i = 0; i < pixels; i++)
                sb.Append($"<rect x='{x + i * pixelSize}' y='{y}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            for (int i = 1; i < pixels; i++)
                sb.Append($"<rect x='{x + size - pixelSize}' y='{y + i * pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            for (int i = pixels - 2; i >= 0; i--)
                sb.Append($"<rect x='{x + i * pixelSize}' y='{y + size - pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            for (int i = pixels - 2; i > 0; i--)
                sb.Append($"<rect x='{x}' y='{y + i * pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            return sb.ToString();
        }

        /// <summary>
        /// Aplica rotación a un fragmento SVG alrededor de un centro dado.
        /// </summary>
        private static string WithRotation(string path, double cx, double cy, double angle)
        {
            return $"<g transform='rotate({angle},{cx},{cy})'>{path}</g>";
        }

        /// <summary>
        /// Dibuja un marco de ojo con una esquina inferior izquierda cuadrada, resto redondeado.
        /// </summary>
        private static string DrawBottomLeftSquareEye(double x, double y, double size, string color)
        {
            double r = size * 0.30;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + size} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x + size - r},{y} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size},{y + r} ");
            sb.Append($"L{x + size},{y + size - r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size - r},{y + size} ");
            sb.Append($"L{x},{y + size} Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Dibuja el centro (blanco) para el marco de ojo BottomLeftSquare.
        /// </summary>
        private static string DrawBottomLeftSquareEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double centerSize = size - 2 * padding;
            return DrawBottomLeftSquareEye(x + padding, y + padding, centerSize, color);
        }

        /// <summary>
        /// Dibuja la pupila para el marco de ojo BottomLeftSquare.
        /// </summary>
        private static string DrawBottomLeftSquareEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja un marco de ojo con dos esquinas opuestas redondeadas.
        /// </summary>
        private static string DrawTwoCornerRectEye(double x, double y, double size, string color, bool outward = true)
        {
            double r = size * 0.28;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"L{x},{y + size} ");
            sb.Append($"L{x + size - r},{y + size} ");
            sb.Append($"A{r},{r} 0 0,0 {x + size},{y + size - r} ");
            sb.Append($"L{x + size},{y} ");
            sb.Append($"L{x + r},{y} ");
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Centro blanco para marco TwoCornerRect.
        /// </summary>
        private static string DrawTwoCornerRectEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double centerSize = size - 2 * padding;
            return DrawTwoCornerRectEye(x + padding, y + padding, centerSize, color);
        }

        /// <summary>
        /// Pupila para marco TwoCornerRect.
        /// </summary>
        private static string DrawTwoCornerRectEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawTwoCornerRectEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja el marco exterior con esquinas redondeadas (radio mayor).
        /// </summary>
        private static string DrawBottomLeftSquareRadioEye(double x, double y, double size, string color)
        {
            double r = size * 0.50;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + size} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x + size - r},{y} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size},{y + r} ");
            sb.Append($"L{x + size},{y + size - r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size - r},{y + size} ");
            sb.Append($"L{x},{y + size} Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Centro blanco para el marco BottomLeftSquareRadio.
        /// </summary>
        private static string DrawBottomLeftSquareRadioEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double centerSize = size - 2 * padding;
            return DrawBottomLeftSquareRadioEye(x + padding, y + padding, centerSize, color);
        }

        /// <summary>
        /// Pupila para el marco BottomLeftSquareRadio.
        /// </summary>
        private static string DrawBottomLeftSquareRadioEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareRadioEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja el marco con esquinas internas redondeadas.
        /// </summary>
        private static string DrawTwoCornerRectInEye(double x, double y, double size, string color, bool outward = true)
        {
            double r = size * 0.23;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x + r},{y} ");
            sb.Append($"A{r},{r} 0 0,1 {x},{y + r} ");
            sb.Append($"L{x},{y + size - r} ");
            sb.Append($"A{r},{r} 0 0,0 {x + r},{y + size} ");
            sb.Append($"L{x + size - r},{y + size} ");
            sb.Append($"A{r},{r} 0 0,1 {x + size},{y + size - r} ");
            sb.Append($"L{x + size},{y + r} ");
            sb.Append($"A{r},{r} 0 0,0 {x + size - r},{y} ");
            sb.Append($"L{x + r},{y} ");
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Centro blanco para el marco TwoCornerRectIn.
        /// </summary>
        private static string DrawTwoCornerRectInEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double centerSize = size - 2 * padding;
            return DrawTwoCornerRectInEye(x + padding, y + padding, centerSize, color);
        }

        /// <summary>
        /// Pupila para el marco TwoCornerRectIn.
        /// </summary>
        private static string DrawTwoCornerRectInEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawTwoCornerRectInEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja el marco exterior "CornerRoundOut".
        /// </summary>
        private static string DrawBottomLeftSquareRCOEye(double x, double y, double size, string color)
        {
            double r = size * 0.50;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"L{x},{y + size} ");
            sb.Append($"L{x + size - r},{y + size} ");
            sb.Append($"A{r},{r} 0 0,0 {x + size},{y + size - r} ");
            sb.Append($"L{x + size},{y} ");
            sb.Append($"L{x + r},{y} ");
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Pupila para el marco CornerRoundOut.
        /// </summary>
        private static string DrawBottomLeftSquareRCOEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareRadioEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Dibuja el marco exterior "CornerRoundOutSP".
        /// </summary>
        private static string DrawBottomLeftSquareRCOSPEye(double x, double y, double size, string color)
        {
            double r = size * 0.50;
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + r} ");
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");
            sb.Append($"L{x},{y + r} ");
            sb.Append($"L{x},{y + size} ");
            sb.Append($"L{x + size - r},{y + size} ");
            sb.Append($"A{r},{r} 0 0,0 {x + size},{y + size - r} ");
            sb.Append($"L{x + size},{y} ");
            sb.Append($"L{x + r},{y} ");
            sb.Append("Z' ");
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        /// <summary>
        /// Pupila para el marco CornerRoundOutSP.
        /// </summary>
        private static string DrawBottomLeftSquareRCOSPEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.15;
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareRadioEye(x + padding, y + padding, pupilSize, color);
        }

        /// <summary>
        /// Pupila especial para el marco TwoCornerRectIn (no utilizado en principal, pero disponible).
        /// </summary>
        private static string DrawTwoCornerRCOSPInEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25;
            double pupilSize = size - 2 * padding;
            return DrawTwoCornerRectInEye(x + padding, y + padding, pupilSize, color);
        }
    }
}
