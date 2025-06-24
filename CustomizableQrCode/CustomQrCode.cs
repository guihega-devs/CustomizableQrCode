using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode
{
    public class CustomQrCode
    {
        private readonly string _svg;
        private readonly QrExportFormat _format;

        public CustomQrCode(string svg, QrExportFormat format)
        {
            _svg = svg;
            _format = format;
        }

        public string AsSvg() => _svg;

        public void SaveAs(string filePath)
        {
            if (_format == QrExportFormat.Svg)
                File.WriteAllText(filePath, _svg);
            else
                throw new NotImplementedException("Solo SVG implementado en este ejemplo.");
        }
    }
}
