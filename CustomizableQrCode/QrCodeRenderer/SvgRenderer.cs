using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode.QrCodeRenderer
{
    public static class SvgRenderer
    {
        public static string Render(
            bool[,] matrix,
            ModuleShape moduleShape,
            string moduleColor,
            EyeFrameShape eyeFrameShape,    // nuevo
            string eyeFrameColor,           // nuevo
            EyeCenterShape eyeCenterShape,  // nuevo
            string eyeCenterColor,          // nuevo
            string? backgroundGradient,
            string? logoBase64,
            int size // ahora es "calidad"
        )
        {
            int modules = matrix.GetLength(0);
            double moduleSize = (double)size / modules;

            var sb = new StringBuilder();
            sb.AppendLine($"<svg xmlns='http://www.w3.org/2000/svg' width='{size}' height='{size}' viewBox='0 0 {size} {size}'>");

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
                else if (colors.Length == 1)
                {
                    sb.AppendLine($"<rect width='{size}' height='{size}' fill='{colors[0]}'/>");
                }
                else
                {
                    sb.AppendLine($"<rect width='{size}' height='{size}' fill='#fff'/>");
                }
            }
            else
            {
                sb.AppendLine($"<rect width='{size}' height='{size}' fill='#fff'/>");
            }

            // Render módulos
            for (int x = 0; x < modules; x++)
            {
                for (int y = 0; y < modules; y++)
                {
                    if (!matrix[x, y]) continue;
                    double px = x * moduleSize;
                    double py = y * moduleSize;

                    if (moduleShape == ModuleShape.Square)
                    {
                        sb.AppendLine($"<rect x='{px}' y='{py}' width='{moduleSize}' height='{moduleSize}' fill='{moduleColor}'/>");
                    }
                    else if (moduleShape == ModuleShape.Circle)
                    {
                        sb.AppendLine($"<circle cx='{px + moduleSize / 2}' cy='{py + moduleSize / 2}' r='{moduleSize / 2}' fill='{moduleColor}'/>");
                    }
                    else if (moduleShape == ModuleShape.Hexagon)
                    {
                        // Calcular puntos del hexágono
                        var hex = GetHexagonPoints(px, py, moduleSize);
                        sb.AppendLine($"<polygon points='{hex}' fill='{moduleColor}'/>");
                    }
                    // Puedes agregar otros casos aquí
                }
            }

            // Render de "ojos" básicos (sólo esquinas, ejemplo simple)
            // Puedes ampliar para soportar formas y bordes personalizados
            // Tamaño y posición de los ojos
            double eyeSize = moduleSize * 7;
            double r = eyeSize / 2;
            var eyePositions = new (double x, double y)[] {
                (0, 0),
                (size - eyeSize, 0),
                (0, size - eyeSize),
            };

            // --- Parámetros comunes ---
            


            foreach (var (ex, ey) in eyePositions)
            {
                // --- MARCO (borde) del ojo ---
                // Parámetros clave:
                // ex, ey => posición esquina superior izquierda del ojo
                // r      => radio (eyeSize / 2)
                // eyeSize => tamaño del ojo (7 módulos normalmente)
                // moduleSize => tamaño de un módulo QR
                // eyeFrameColor => color del marco del ojo

                double cx = ex + r;
                double cy = ey + r;
                // Calcula el ancho del borde (ajústalo para hacerlo más o menos delgado)
                //double strokeWidth = moduleSize * 0.4;
                double strokeWidth;
                if (eyeFrameShape == EyeFrameShape.Square ||
                    eyeFrameShape == EyeFrameShape.Rounded ||
                    eyeFrameShape == EyeFrameShape.Circle)
                {
                    strokeWidth = moduleSize * 1.2; // Más grueso (antes: 0.7)
                }
                else
                {
                    strokeWidth = moduleSize * 0.7; // El actual, más fino para Leaf, Point, Diamond
                }
                double borderWidth = eyeSize * 0.08; // Ajusta para un borde delgado, puedes bajar hasta 0.08 si lo quieres más light
                double centerSize = eyeSize * 0.32;  // Centro pequeño y bien alineado

                // --- Dibuja el MARCO exterior del ojo, según el tipo ---
                if (eyeFrameShape == EyeFrameShape.Square)
                {
                    // Cuadrado normal, borde delgado
                    sb.AppendLine(
                        $"<rect x='{ex + strokeWidth / 2}' y='{ey + strokeWidth / 2}' width='{eyeSize - strokeWidth}' height='{eyeSize - strokeWidth}' " +
                        $"fill='none' stroke='{eyeFrameColor}' stroke-width='{strokeWidth}'/>");
                }
                else if (eyeFrameShape == EyeFrameShape.Rounded)
                {
                    // Cuadrado con esquinas redondeadas, borde delgado
                    double rx = moduleSize * 1.2;
                    sb.AppendLine(
                        $"<rect x='{ex + strokeWidth / 2}' y='{ey + strokeWidth / 2}' width='{eyeSize - strokeWidth}' height='{eyeSize - strokeWidth}' " +
                        $"rx='{rx}' ry='{rx}' fill='none' stroke='{eyeFrameColor}' stroke-width='{strokeWidth}'/>");
                }
                else if (eyeFrameShape == EyeFrameShape.Circle)
                {
                    // Círculo, borde delgado
                    cx = ex + eyeSize / 2;
                    cy = ey + eyeSize / 2;
                    r = (eyeSize - borderWidth) / 2;
                    sb.AppendLine(
                        $"<circle cx='{cx}' cy='{cy}' r='{r - strokeWidth / 2}' fill='none' stroke='{eyeFrameColor}' stroke-width='{strokeWidth}'/>");
                }
                else if (eyeFrameShape == EyeFrameShape.Diamond)
                {
                    // DIAMOND: un rombo (diagonal)
                    // Exterior diamond
                    string d1 = GetDiamondPath(ex + r, ey + r, r);
                    sb.AppendLine($"<path d='{d1}' fill='{eyeFrameColor}'/>");
                    // Interior diamond blanco (más chico)
                    string d2 = GetDiamondPath(ex + r, ey + r, r * 0.68);
                    sb.AppendLine($"<path d='{d2}' fill='#fff'/>");
                    // Centro (pupila)
                    string d3 = GetDiamondPath(ex + r, ey + r, r * 0.25);
                    sb.AppendLine($"<path d='{d3}' fill='{eyeCenterColor}'/>");
                    //continue;

                }
                else if (eyeFrameShape == EyeFrameShape.Point)
                {
                    // POINT: círculo pequeño en cada esquina (tipo “dot”)
                    double R = r * 0.95;
                    double r2 = r * 0.62;
                    double r3 = r * 0.22;
                    sb.AppendLine($"<circle cx='{ex + r}' cy='{ey + r}' r='{R}' fill='{eyeFrameColor}'/>");
                    sb.AppendLine($"<circle cx='{ex + r}' cy='{ey + r}' r='{r2}' fill='#fff'/>");
                    sb.AppendLine($"<circle cx='{ex + r}' cy='{ey + r}' r='{r3}' fill='{eyeCenterColor}'/>");
                    //continue;

                }
                else if (eyeFrameShape == EyeFrameShape.Leaf)
                {
                    // LEAF: dos arcos suaves como hoja (moderno QR)
                    // Hoja con extremos puntiagudos, tipo “almendra”
                    // Exterior (hoja)
                    sb.AppendLine(DrawLeafEye(ex, ey, eyeSize, eyeFrameColor));
                    // Interior blanco (más chico)
                    sb.AppendLine(DrawLeafEye(ex + eyeSize * 0.19, ey + eyeSize * 0.19, eyeSize * 0.62, "#fff"));
                    // Centro (más pequeño aún)
                    sb.AppendLine(DrawLeafEye(ex + eyeSize * 0.33, ey + eyeSize * 0.33, eyeSize * 0.34, eyeCenterColor));
                    //continue; // No dibujes el rect blanco ni el centro cuadrado

                }
                else
                {
                    // Fallback: cuadrado
                    sb.AppendLine(
                        $"<rect x='{ex + strokeWidth / 2}' y='{ey + strokeWidth / 2}' width='{eyeSize - strokeWidth}' height='{eyeSize - strokeWidth}' fill='none' stroke='{eyeFrameColor}' stroke-width='{strokeWidth}'/>");
                }

                //// --- CENTRO BLANCO ---
                //double inner = moduleSize * 3;
                //double offset = moduleSize * 2;
                //sb.AppendLine(
                //    $"<rect x='{ex + offset}' y='{ey + offset}' width='{inner}' height='{inner}' fill='#fff' rx='{moduleSize * 0.5}'/>");

                //// --- PUPILA (centro real del ojo) ---
                //double center = moduleSize * 1.2;
                //sb.AppendLine(
                //    $"<rect x='{ex + offset + center}' y='{ey + offset + center}' width='{inner - center * 2}' height='{inner - center * 2}' " +
                //    $"fill='{eyeCenterColor}' rx='{moduleSize * 0.25}'/>");

                // --- CENTRO DEL OJO (solo Circle permitido para Diamond/Leaf) ---
                EyeCenterShape allowedShape = eyeCenterShape;
                if (eyeFrameShape == EyeFrameShape.Leaf )
                {
                    if (eyeCenterShape == EyeCenterShape.Circle)
                    {
                        allowedShape = EyeCenterShape.Circle;
                    }
                    else
                    {
                        allowedShape = EyeCenterShape.Leaf;
                    }
                }
                else if (eyeFrameShape == EyeFrameShape.Diamond)
                {
                    if (eyeCenterShape == EyeCenterShape.Circle)
                    {
                        allowedShape = EyeCenterShape.Circle;
                    }
                    else
                    {
                        allowedShape = EyeCenterShape.Diamond;
                    }
                }

                // Ajusta el centro para cada forma
                //if (allowedShape == EyeCenterShape.Circle)
                //{
                //    sb.AppendLine($"<circle cx='{cx}' cy='{cy}' r='{eyeSize * 0.22}' fill='{eyeCenterColor}'/>");
                //}
                //else if (allowedShape == EyeCenterShape.Square)
                //{
                //    double sz = eyeSize * 0.42;
                //    sb.AppendLine($"<rect x='{cx - sz / 2}' y='{cy - sz / 2}' width='{sz}' height='{sz}' fill='{eyeCenterColor}'/>");
                //}
                //else if (allowedShape == EyeCenterShape.Rounded)
                //{
                //    double sz = eyeSize * 0.42;
                //    sb.AppendLine($"<rect x='{cx - sz / 2}' y='{cy - sz / 2}' width='{sz}' height='{sz}' rx='{sz * 0.28}' fill='{eyeCenterColor}'/>");
                //}
                //else if (allowedShape == EyeCenterShape.Point)
                //{
                //    sb.AppendLine($"<circle cx='{cx}' cy='{cy}' r='{eyeSize * 0.13}' fill='{eyeCenterColor}'/>");
                //}
                if (allowedShape == EyeCenterShape.Circle)
                {
                    sb.AppendLine($"<circle cx='{cx}' cy='{cy}' r='{eyeSize * 0.22}' fill='{eyeCenterColor}'/>");
                }
                else if (allowedShape == EyeCenterShape.Square)
                {
                    double sz = eyeSize * 0.42;
                    sb.AppendLine($"<rect x='{cx - sz / 2}' y='{cy - sz / 2}' width='{sz}' height='{sz}' fill='{eyeCenterColor}'/>");
                }
                else if (allowedShape == EyeCenterShape.Rounded)
                {
                    double sz = eyeSize * 0.42;
                    sb.AppendLine($"<rect x='{cx - sz / 2}' y='{cy - sz / 2}' width='{sz}' height='{sz}' rx='{sz * 0.28}' fill='{eyeCenterColor}'/>");
                }
                else if (allowedShape == EyeCenterShape.Point)
                {
                    sb.AppendLine($"<circle cx='{cx}' cy='{cy}' r='{eyeSize * 0.13}' fill='{eyeCenterColor}'/>");
                }
                else if (allowedShape == EyeCenterShape.Diamond)
                {
                    double rDia = eyeSize * 0.22;
                    string d = GetDiamondPath(cx, cy, rDia);
                    sb.AppendLine($"<path d='{d}' fill='{eyeCenterColor}'/>");
                }
                else if (allowedShape == EyeCenterShape.Leaf)
                {
                    // Puedes ajustar el path como gustes
                    double leafSize = eyeSize * 0.42;
                    string leaf = $"M{cx} {cy - leafSize / 2} Q {cx + leafSize / 2} {cy} {cx} {cy + leafSize / 2} Q {cx - leafSize / 2} {cy} {cx} {cy - leafSize / 2} Z";
                    sb.AppendLine($"<path d='{leaf}' fill='{eyeCenterColor}'/>");
                }

            }

            // Renderizar logo central (si existe)
            if (!string.IsNullOrWhiteSpace(logoBase64))
            {
                double logoSize = size * 0.22; // Proporción del QR
                double logoX = (size - logoSize) / 2;
                double logoY = (size - logoSize) / 2;
                sb.AppendLine(
                    $"<image href='{logoBase64}' x='{logoX}' y='{logoY}' width='{logoSize}' height='{logoSize}' style='pointer-events:none;' />"
                );
            }

            sb.AppendLine("</svg>");
            return sb.ToString();
        }

        private static string GetDiamondPath(double cx, double cy, double r)
        {
            return $"M{cx},{cy - r} L{cx + r},{cy} L{cx},{cy + r} L{cx - r},{cy} Z";
        }


        private static string GetHexagonPoints(double x, double y, double size)
        {
            double w = size;
            double h = size;
            double dx = w / 2.0;
            double dy = h / 2.0;
            double r = size / 2.0;

            // Calcula los 6 vértices del hexágono
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

        private static string DrawLeafEye(double x, double y, double size, string color)
        {
            // Ajusta el path como gustes
            var cx = x + size / 2;
            var cy = y + size / 2;
            var r = size / 2;
            return $"<path d='M{cx} {cy - r} Q {cx + r} {cy} {cx} {cy + r} Q {cx - r} {cy} {cx} {cy - r} Z' fill='{color}'/>";
        }

        private static string DrawPointEye(double x, double y, double size, string color)
        {
            var cx = x + size / 2;
            var cy = y + size / 2;
            var r = size / 2;
            return $"<polygon points='{cx},{cy - r} {cx + r},{cy} {cx},{cy + r} {cx - r},{cy}' fill='{color}' />";
        }

    }


}
