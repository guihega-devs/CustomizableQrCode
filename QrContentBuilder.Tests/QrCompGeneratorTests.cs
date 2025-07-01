using Bunit;
using CustomizableQrCode.DemoBlazor.Components.Blazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QrContentBuilder.Tests
{
    public class QrCompGeneratorTests:TestContext
    {
        [Fact]
        public void GeneraQr_Actualiza_Contenido()
        {
            // Configura el JSInterop para el método JS que usa el componente
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            var comp = RenderComponent<QrCompGenerator>();

            // Busca el input de enlace (ajusta el selector si tu HTML cambia)
            var input = comp.Find("input[placeholder='https://tu-enlace.com']");
            input.Change("https://google.com");

            // Busca y da click al botón de generar QR
            var btn = comp.Find("button:contains('Generar QR')");
            btn.Click();

            // Verifica que el componente hijo recibió el valor correcto (ejemplo)
            Assert.Contains("google.com", comp.Markup);

            // O puedes verificar que el QR visualmente cambió, si tu HTML lo permite
        }

        [Fact]
        public void GeneraQr_Enlace()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("input[placeholder='https://tu-enlace.com']").Change("https://openai.com");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("openai.com", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Texto()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Comparte un mensaje de texto.']").Click();
            comp.Find("textarea[placeholder='Introduce el texto...']").Change("Hola mundo!");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("Hola mundo", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Email()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Correo electrónico prellenado.']").Click();
            comp.Find("input[placeholder='ejemplo@email.com']").Change("correo@dominio.com");
            comp.Find("input[placeholder='Asunto del correo']").Change("Asunto");
            comp.Find("textarea[placeholder='Escribe tu mensaje...']").Change("El cuerpo del correo");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("correo@dominio.com", comp.Markup);
            Assert.Contains("Asunto", comp.Markup);
            Assert.Contains("cuerpo", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Llamada()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Inicia una llamada telefónica.']").Click();
            comp.Find("input[placeholder='Ej: +521234567890']").Change("+525512345678");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("+525512345678", comp.Markup);
        }

        [Fact]
        public void GeneraQr_SMS()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Envía un mensaje SMS.']").Click();
            comp.Find("input[placeholder='Teléfono']").Change("+521234567890");
            comp.Find("textarea[placeholder='Mensaje SMS']").Change("Hola mundo SMS");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("+521234567890", comp.Markup);
            Assert.Contains("Hola mundo SMS", comp.Markup);
        }

        [Fact]
        public void GeneraQr_VCard()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Agrega contacto a la agenda.']").Click();
            comp.Find("input[placeholder='Nombre']").Change("Ada");
            comp.Find("input[placeholder='Apellido']").Change("Lovelace");
            comp.Find("input[placeholder='Teléfono']").Change("1234567890");
            comp.Find("input[placeholder='Email']").Change("ada@lovelace.com");
            comp.Find("input[placeholder='Empresa']").Change("OpenAI");
            comp.Find("input[placeholder='Cargo']").Change("Pionera");
            comp.Find("input[placeholder='Dirección']").Change("UK");
            comp.Find("button:contains('Generar QR')").Click();
            //Assert.Contains("BEGIN:VCARD", comp.Markup);
            Assert.Contains("Ada", comp.Markup);
            Assert.Contains("Lovelace", comp.Markup);
            Assert.Contains("OpenAI", comp.Markup);
        }

        [Fact]
        public void GeneraQr_WhatsApp()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Abre chat de WhatsApp.']").Click();
            comp.Find("input[placeholder='Teléfono WhatsApp']").Change("+525512345678");
            comp.Find("textarea[placeholder='Mensaje para enviar']").Change("¡Hola desde pruebas!");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("+525512345678", comp.Markup);
            Assert.Contains("¡Hola desde pruebas!", comp.Markup); // Puede variar el encode
        }

        [Fact]
        public void GeneraQr_WiFi()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Conéctate a una red WiFi.']").Click();
            comp.Find("input[placeholder='Nombre de red']").Change("MiRedWiFi");
            comp.Find("input[placeholder='Contraseña']").Change("MiPassword123");
            comp.Find("select.qr-select").Change("WPA");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("MiRedWiFi", comp.Markup);
            Assert.Contains("MiPassword123", comp.Markup);
        }

        [Fact]
        public void GeneraQr_PDF()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Descarga un archivo PDF.']").Click();
            comp.Find("input[placeholder='URL del PDF']").Change("https://midominio.com/doc.pdf");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("doc.pdf", comp.Markup);
        }

        [Fact]
        public void GeneraQr_App()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Redirige a una app móvil.']").Click();
            comp.Find("input[placeholder='URL de la App (App Store, Play Store...)']").Change("https://play.google.com/store/apps/details?id=com.example");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("play.google.com", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Imagen()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Muestra una imagen.']").Click();
            comp.Find("input[placeholder='URL de la imagen']").Change("https://midominio.com/imagen.png");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("imagen.png", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Video()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Muestra un video.']").Click();
            comp.Find("input[placeholder='URL del video']").Change("https://video.com/v.mp4");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("v.mp4", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Social()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Perfil/red social.']").Click();
            comp.Find("input[placeholder='URL del perfil social']").Change("https://twitter.com/user");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("twitter.com/user", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Evento()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Evento de calendario (iCal).']").Click();
            comp.Find("input[placeholder='Título del evento']").Change("MiEvento");
            comp.Find("input[placeholder='Ubicación']").Change("MiLugar");
            comp.Find("input[placeholder='YYYY-MM-DD HH:mm']").Change("2025-01-01 09:00");
            comp.FindAll("input[placeholder='YYYY-MM-DD HH:mm']")[1].Change("2025-01-01 11:00");
            comp.Find("button:contains('Generar QR')").Click();
            //Assert.Contains("VEVENT", comp.Markup);
            Assert.Contains("MiEvento", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Barcode2D()
        {
            JSInterop.Setup<int>("getWindowWidth").SetResult(1024);
            var comp = RenderComponent<QrCompGenerator>();
            comp.Find("button[title='Código de barras (DataMatrix, etc).']").Click();
            comp.Find("input[placeholder='Texto o número']").Change("DATAMATRIX-2025");
            comp.Find("button:contains('Generar QR')").Click();
            Assert.Contains("DATAMATRIX-2025", comp.Markup);
        }
    }
}
