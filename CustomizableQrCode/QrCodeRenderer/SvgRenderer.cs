using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode.QrCodeRenderer
{
    public static class SvgRenderer
    {
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

            var eyePositions = new (int x, int y)[]
            {
                (quietZone, quietZone),
                (modules - 7 - quietZone, quietZone),
                (quietZone, modules - 7 - quietZone)
            };

            var sb = new StringBuilder();
            sb.AppendLine($"<svg xmlns='http://www.w3.org/2000/svg' width='{size}' height='{size}' viewBox='0 0 {size} {size}'>");

            // Fondo
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

            // Helper para saber si (x,y) está dentro de algún ojo
            bool IsInEyeArea(int x, int y)
            {
                foreach (var (ex, ey) in eyePositions)
                {
                    if (x >= ex && x < ex + 7 && y >= ey && y < ey + 7)
                        return true;
                }
                return false;
            }

            // Render módulos QR (sin superponer los ojos)
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

            // Renderizado de los ojos
            //foreach (var (ex, ey) in eyePositions)
            for (int eyeIndex = 0; eyeIndex < eyePositions.Length; eyeIndex++)
            {
                //double ox = ex * moduleSize;
                //double oy = ey * moduleSize;
                //double centerCx = ox + eyeSize / 2;
                //double centerCy = oy + eyeSize / 2;

                var (ex, ey) = eyePositions[eyeIndex];
                double ox = ex * moduleSize;
                double oy = ey * moduleSize;
                double centerCx = ox + eyeSize / 2;
                double centerCy = oy + eyeSize / 2;

                double angle = 0;
                switch (eyeIndex)
                {
                    case 0: angle = 270; break;    // Superior izquierda
                    case 1: angle = 360; break;   // Superior derecha
                    case 2: angle = -180; break;  // Inferior izquierda
                }

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
                    // --- Nuevos estilos de marco (pro) ---
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
                    //Nuevas pruebas
                    case EyeFrameShape.CornerRect:
                        //sb.AppendLine(DrawBottomLeftSquareEye(ox, oy, eyeSize, eyeFrameColor)); // Marco
                        //sb.AppendLine(DrawBottomLeftSquareEyeCenter(ox, oy, eyeSize, "#fff"));  // Centro blanco

                        string marco = DrawBottomLeftSquareEye(ox, oy, eyeSize, eyeFrameColor);
                        sb.AppendLine(WithRotation(marco, centerCx, centerCy, angle));
                        // CENTRO blanco
                        string centro = DrawBottomLeftSquareEyeCenter(ox, oy, eyeSize, "#fff");
                        sb.AppendLine(WithRotation(centro, centerCx, centerCy, angle));

                        //continue;
                        break;
                    default:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                }

                // Pupila/centro del ojo (igual que antes)
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
                        //sb.AppendLine(DrawBottomLeftSquareEyePupil(ox, oy, eyeSize, eyeCenterColor)); // Pupila negra
                        string pupil = DrawBottomLeftSquareEyePupil(ox, oy, eyeSize, eyeCenterColor);
                        sb.AppendLine(WithRotation(pupil, centerCx, centerCy, angle));
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

        // --- Helpers ---
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

        private static string GetDiamondPath(double cx, double cy, double r)
        {
            return $"M{cx},{cy - r} L{cx + r},{cy} L{cx},{cy + r} L{cx - r},{cy} Z";
        }

        private static string DrawLeafEye(double x, double y, double size, string color)
        {
            var cx = x + size / 2;
            var cy = y + size / 2;
            var r = size / 2;
            return $"<path d='M{cx} {cy - r} Q {cx + r} {cy} {cx} {cy + r} Q {cx - r} {cy} {cx} {cy - r} Z' fill='{color}'/>";
        }

        private static string DrawDottedEye(double x, double y, double size, string color, double moduleSize)
        {
            // Círculos en los bordes, efecto punteado
            int dots = 14;
            double r = moduleSize * 0.38;
            var sb = new StringBuilder();
            // Superior
            for (int i = 0; i < dots; i++)
            {
                double px = x + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{px}' cy='{y}' r='{r}' fill='{color}'/>");
            }
            // Inferior
            for (int i = 0; i < dots; i++)
            {
                double px = x + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{px}' cy='{y + size}' r='{r}' fill='{color}'/>");
            }
            // Izquierda
            for (int i = 1; i < dots - 1; i++)
            {
                double py = y + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{x}' cy='{py}' r='{r}' fill='{color}'/>");
            }
            // Derecha
            for (int i = 1; i < dots - 1; i++)
            {
                double py = y + (i / (double)(dots - 1)) * size;
                sb.AppendLine($"<circle cx='{x + size}' cy='{py}' r='{r}' fill='{color}'/>");
            }
            return sb.ToString();
        }

        private static string DrawDoubleEye(double x, double y, double size, string color, double moduleSize)
        {
            // Doble línea cuadrada: borde externo e interno
            double margin = moduleSize * 0.9;
            var sb = new StringBuilder();
            sb.AppendLine($"<rect x='{x}' y='{y}' width='{size}' height='{size}' fill='none' stroke='{color}' stroke-width='{margin}'/>");
            sb.AppendLine($"<rect x='{x + margin * 1.4}' y='{y + margin * 1.4}' width='{size - margin * 2.8}' height='{size - margin * 2.8}' fill='none' stroke='{color}' stroke-width='{margin / 1.7}'/>");
            return sb.ToString();
        }

        private static string DrawWavyEye(double x, double y, double size, string color)
        {
            // Un marco con ondas senoidales en los 4 lados
            int waves = 4;
            double amplitude = size * 0.07;
            var sb = new StringBuilder();
            sb.Append($"<path d='M{x},{y + amplitude}");
            // Top
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size * (t - 0.25 / waves)},{y - amplitude} {x + size * t},{y + amplitude}");
            }
            // Right
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size + amplitude},{y + size * (t - 0.25 / waves)} {x + size - amplitude},{y + size * t}");
            }
            // Bottom
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x + size * (1 - t + 0.25 / waves)},{y + size + amplitude} {x + size * (1 - t)},{y + size - amplitude}");
            }
            // Left
            for (int i = 1; i <= waves; i++)
            {
                double t = (double)i / waves;
                sb.Append($" Q{x - amplitude},{y + size * (1 - t + 0.25 / waves)} {x + amplitude},{y + size * (1 - t)}");
            }
            sb.Append(" Z' fill='" + color + "'/>");
            return sb.ToString();
        }

        // Un cuadrado con una sola esquina redondeada (por ejemplo, la superior izquierda)
        private static string DrawCornerRoundedEye(double x, double y, double size, string color, string corner)
        {
            double r = size * 0.25; // Radio de la esquina redondeada
            var sb = new StringBuilder();

            switch (corner)
            {
                case "left": // Solo la esquina superior izquierda redondeada
                    sb.Append($"<path d='M{x + r},{y} ");                    // Inicio después del radio
                    sb.Append($"L{x + size},{y} ");                         // Línea al vértice sup. derecho
                    sb.Append($"L{x + size},{y + size} ");                  // Línea al vértice inf. derecho
                    sb.Append($"L{x},{y + size} ");                         // Línea al vértice inf. izquierdo
                    sb.Append($"L{x},{y + r} ");                            // Línea hasta el comienzo del arco
                    sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");              // ***Arco externo, sweep=1***
                    sb.Append("Z' fill='" + color + "'/>");
                    break;
                case "right": // Solo la esquina superior derecha redondeada
                    sb.Append($"<path d='M{x},{y} ");
                    sb.Append($"L{x + size - r},{y} ");
                    sb.Append($"A{r},{r} 0 0,1 {x + size},{y + r} ");
                    sb.Append($"L{x + size},{y + size} ");
                    sb.Append($"L{x},{y + size} ");
                    sb.Append($"Z' fill='{color}'/>");
                    break;
                case "top": // Solo la esquina inferior izquierda redondeada
                    sb.Append($"<path d='M{x},{y} ");
                    sb.Append($"L{x + size},{y} ");
                    sb.Append($"L{x + size},{y + size} ");
                    sb.Append($"L{x + r},{y + size} ");
                    sb.Append($"A{r},{r} 0 0,1 {x},{y + size - r} ");
                    sb.Append($"Z' fill='{color}'/>");
                    break;
                case "bottom": // Solo la esquina inferior derecha redondeada
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

        private static string DrawPixelatedEye(double x, double y, double size, string color)
        {
            int pixels = 8;
            double pixelSize = size / (double)pixels;
            var sb = new StringBuilder();

            // Solo dibuja el borde externo pixel a pixel (no todo el cuadrado)
            // Top
            for (int i = 0; i < pixels; i++)
                sb.Append($"<rect x='{x + i * pixelSize}' y='{y}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            // Right
            for (int i = 1; i < pixels; i++)
                sb.Append($"<rect x='{x + size - pixelSize}' y='{y + i * pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            // Bottom
            for (int i = pixels - 2; i >= 0; i--)
                sb.Append($"<rect x='{x + i * pixelSize}' y='{y + size - pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            // Left
            for (int i = pixels - 2; i > 0; i--)
                sb.Append($"<rect x='{x}' y='{y + i * pixelSize}' width='{pixelSize * 0.9}' height='{pixelSize * 0.9}' fill='{color}'/>");
            return sb.ToString();
        }


        // Dibuja el marco exterior
        private static string DrawBottomLeftSquareEye(double x, double y, double size, string color)
        {
            double r = size * 0.30; // radio de las esquinas redondeadas
            var sb = new StringBuilder();

            sb.Append($"<path d='");
            sb.Append($"M{x},{y + size} ");         // esquina inferior izquierda (cuadrada)
            sb.Append($"L{x},{y + r} ");            // lado izquierdo
            sb.Append($"A{r},{r} 0 0,1 {x + r},{y} ");      // esquina superior izquierda (redondeada)
            sb.Append($"L{x + size - r},{y} ");     // lado superior
            sb.Append($"A{r},{r} 0 0,1 {x + size},{y + r} "); // esquina superior derecha (redondeada)
            sb.Append($"L{x + size},{y + size - r} "); // lado derecho
            sb.Append($"A{r},{r} 0 0,1 {x + size - r},{y + size} "); // esquina inferior derecha (redondeada)
            sb.Append($"L{x},{y + size} Z' ");      // cerrar figura
            sb.Append($"fill='{color}'/>");

            return sb.ToString();
        }

        // Centro blanco, padding medio
        private static string DrawBottomLeftSquareEyeCenter(double x, double y, double size, string color)
        {
            double padding = size * 0.15; // padding ajustable (ajusta 0.28-0.33 según veas)
            double centerSize = size - 2 * padding;
            return DrawBottomLeftSquareEye(x + padding, y + padding, centerSize, color);
        }

        // Pupila, padding mayor (más pequeña)
        private static string DrawBottomLeftSquareEyePupil(double x, double y, double size, string color)
        {
            double padding = size * 0.25; // padding mayor para la pupila
            double pupilSize = size - 2 * padding;
            return DrawBottomLeftSquareEye(x + padding, y + padding, pupilSize, color);
        }

        private static string WithRotation(string path, double cx, double cy, double angle)
        {
            return $"<g transform='rotate({angle},{cx},{cy})'>{path}</g>";
        }


    }
}
