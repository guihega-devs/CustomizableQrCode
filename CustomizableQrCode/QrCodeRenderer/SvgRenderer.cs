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
            // Ajusta este valor si tu generador QR agrega quiet zone
            int quietZone = 4;
            int modules = matrix.GetLength(0);
            double moduleSize = (double)size / modules;
            double eyeSize = moduleSize * 7;
            double innerSize = eyeSize - 2 * moduleSize;
            double centerSize = eyeSize - 4 * moduleSize;

            // Ojos en posiciones de QR estándar, considerando quiet zone
            var eyePositions = new (int x, int y)[]
            {
                (quietZone, quietZone),                                  // Top-left
                (modules - 7 - quietZone, quietZone),                    // Top-right
                (quietZone, modules - 7 - quietZone)                     // Bottom-left
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

            // Render módulos QR (SIN superponer los ojos)
            for (int x = 0; x < modules; x++)
            {
                for (int y = 0; y < modules; y++)
                {
                    if (IsInEyeArea(x, y)) continue; // Omitir áreas de los ojos
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
                            // Más formas...
                    }
                }
            }

            // Renderizado de los ojos
            foreach (var (ex, ey) in eyePositions)
            {
                double ox = ex * moduleSize;
                double oy = ey * moduleSize;
                double centerCx = ox + eyeSize / 2;
                double centerCy = oy + eyeSize / 2;

                // Marco exterior del ojo
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
                    default:
                        sb.AppendLine($"<rect x='{ox}' y='{oy}' width='{eyeSize}' height='{eyeSize}' fill='{eyeFrameColor}'/>");
                        sb.AppendLine($"<rect x='{ox + moduleSize}' y='{oy + moduleSize}' width='{innerSize}' height='{innerSize}' fill='#fff'/>");
                        break;
                }

                // Pupila/centro del ojo
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
    }
}
