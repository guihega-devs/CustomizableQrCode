using static CustomizableQrCode.Models.QrModels;

namespace CustomizableQrCode.ConsoleTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");

            var qr = new QrCodeBuilder()
            .WithContent("https://midominio.com")
            .WithModuleShape(ModuleShape.Circle)
            .WithModuleColor("#000")
            .WithEyeShape(EyeShape.Square)
            .WithEyeColor("#222")
            .WithBackgroundGradient("#fff,#f0f0f0")
            .WithExportFormat(QrExportFormat.Svg)
            .WithSize(512)
            .Build();

            qr.SaveAs("qrpersonalizado.svg");

            Console.WriteLine("QR generado correctamente!");
        }
    }
}
