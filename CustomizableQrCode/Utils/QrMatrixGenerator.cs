using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomizableQrCode.Models;
using ZXing;
using ZXing.QrCode;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode.Utils
{
    public static class QrMatrixGenerator
    {
        public static bool[,] GenerateMatrix(string content, QrCorrectionLevel level)
        {
            var writer = new QRCodeWriter();
            var hints = new Dictionary<EncodeHintType, object>
            {
                [EncodeHintType.ERROR_CORRECTION] = level.ToString()
            };
            var bitMatrix = writer.encode(content, BarcodeFormat.QR_CODE, 21, 21, hints);

            int width = bitMatrix.Width;
            int height = bitMatrix.Height;
            var matrix = new bool[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    matrix[x, y] = bitMatrix[x, y];
            return matrix;
        }
    }
}
