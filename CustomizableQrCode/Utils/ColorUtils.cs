namespace CustomizableQrCode.Utils
{
    public static class ColorUtils
    {
        public static bool IsContrastAccessible(string hexColor1, string hexColor2, double threshold = 4.5)
        {
            var rgb1 = HexToRgb(hexColor1);
            var rgb2 = HexToRgb(hexColor2);

            if (rgb1 == null || rgb2 == null) return false;

            double lum1 = GetLuminance(rgb1.Value);
            double lum2 = GetLuminance(rgb2.Value);

            double contrast = (Math.Max(lum1, lum2) + 0.05) / (Math.Min(lum1, lum2) + 0.05);
            return contrast >= threshold;
        }

        private static (int R, int G, int B)? HexToRgb(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            if (hex.StartsWith("#")) hex = hex.Substring(1);

            if (hex.Length != 6) return null;

            try
            {
                return (
                    Convert.ToInt32(hex.Substring(0, 2), 16),
                    Convert.ToInt32(hex.Substring(2, 2), 16),
                    Convert.ToInt32(hex.Substring(4, 2), 16)
                );
            }
            catch
            {
                return null;
            }
        }

        private static double GetLuminance((int R, int G, int B) color)
        {
            double R = Linearize(color.R / 255.0);
            double G = Linearize(color.G / 255.0);
            double B = Linearize(color.B / 255.0);

            return 0.2126 * R + 0.7152 * G + 0.0722 * B;
        }

        private static double Linearize(double channel)
        {
            return (channel <= 0.03928)
                ? channel / 12.92
                : Math.Pow((channel + 0.055) / 1.055, 2.4);
        }
    }
}
