using Bunit;
using CustomizableQrCode.DemoBlazor.Components.Blazor;
using CustomizableQrCode.Logic;
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
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();

            // Busca el input de enlace (ajusta el selector si tu HTML cambia)
            var input = comp.Find("input[value='https://midominio.com']");
            input.Change("https://google.com");

            // Busca y da click al botón de Generar
            var btn = comp.Find("button:contains('Generar')");
            btn.Click();

            // Verifica que el componente hijo recibió el valor correcto (ejemplo)
            Assert.Contains("google.com", comp.Markup);

            // O puedes verificar que el QR visualmente cambió, si tu HTML lo permite
        }

        [Fact]
        public void GeneraQr_Enlace()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();

            comp.Find("input[value='https://midominio.com']").Change("https://openai.com");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("openai.com", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Texto()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Comparte un mensaje de texto.']").Click();
            comp.Find("button:contains('Comparte un mensaje de texto')").Click();
            comp.Find("textarea[placeholder='Introduce el texto...']").Change("Hola mundo!");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("Hola mundo", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Email()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Correo electrónico prellenado.']").Click();
            comp.Find("button:contains('Correo electrónico prellenado')").Click();
            comp.Find("input[placeholder='ejemplo@email.com']").Change("correo@dominio.com");
            comp.Find("input[placeholder='Asunto del correo']").Change("Asunto");
            comp.Find("textarea[placeholder='Escribe tu mensaje...']").Change("El cuerpo del correo");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("correo@dominio.com", comp.Markup);
            Assert.Contains("Asunto", comp.Markup);
            Assert.Contains("cuerpo", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Llamada()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Inicia una llamada telefónica.']").Click();
            comp.Find("button:contains('Inicia una llamada telefónica')").Click();
            comp.Find("input[placeholder='Ej: +521234567890']").Change("+525512345678");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("+525512345678", comp.Markup);
        }

        [Fact]
        public void GeneraQr_SMS()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Envía un mensaje SMS.']").Click();
            comp.Find("button:contains('Envía un mensaje SMS')").Click();
            comp.Find("input[placeholder='Teléfono']").Change("+521234567890");
            comp.Find("textarea[placeholder='Mensaje SMS']").Change("Hola mundo SMS");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("+521234567890", comp.Markup);
            Assert.Contains("Hola mundo SMS", comp.Markup);
        }

        [Fact]
        public void GeneraQr_VCard()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Agrega contacto a la agenda.']").Click();
            comp.Find("button:contains('Agrega contacto a la agenda')").Click();
            comp.Find("input[placeholder='Nombre']").Change("Ada");
            comp.Find("input[placeholder='Apellido']").Change("Lovelace");
            comp.Find("input[placeholder='Teléfono']").Change("1234567890");
            comp.Find("input[placeholder='Email']").Change("ada@lovelace.com");
            comp.Find("input[placeholder='Empresa']").Change("OpenAI");
            comp.Find("input[placeholder='Cargo']").Change("Pionera");
            comp.Find("input[placeholder='Dirección']").Change("UK");
            comp.Find("button:contains('Generar')").Click();
            //Assert.Contains("BEGIN:VCARD", comp.Markup);
            Assert.Contains("Ada", comp.Markup);
            Assert.Contains("Lovelace", comp.Markup);
            Assert.Contains("OpenAI", comp.Markup);
        }

        [Fact]
        public void GeneraQr_WhatsApp()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Abre chat de WhatsApp.']").Click();
            comp.Find("button:contains('Abre chat de WhatsApp')").Click();
            comp.Find("input[placeholder='Teléfono WhatsApp']").Change("+525512345678");
            comp.Find("textarea[placeholder='Mensaje para enviar']").Change("¡Hola desde pruebas!");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("+525512345678", comp.Markup);
            Assert.Contains("¡Hola desde pruebas!", comp.Markup); // Puede variar el encode
        }

        [Fact]
        public void GeneraQr_WiFi()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Conéctate a una red WiFi.']").Click();
            comp.Find("button:contains('Conéctate a una red WiFi')").Click();
            comp.Find("input[placeholder='Nombre de red']").Change("MiRedWiFi");
            comp.Find("input[placeholder='Contraseña']").Change("MiPassword123");
            comp.Find("select.qr-select").Change("WPA");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("MiRedWiFi", comp.Markup);
            Assert.Contains("MiPassword123", comp.Markup);
        }

        [Fact]
        public void GeneraQr_PDF()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Descarga un archivo PDF.']").Click();
            comp.Find("button:contains('Descarga un archivo PDF')").Click();
            comp.Find("input[placeholder='URL del PDF']").Change("https://midominio.com/doc.pdf");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("doc.pdf", comp.Markup);
        }

        [Fact]
        public void GeneraQr_App()
        {
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[tooltip='Redirige a una app móvil.']").Click();
            comp.Find("button:contains('Redirige a una app móvil')").Click();
            comp.Find("input[placeholder='URL de la App (App Store, Play Store...)']").Change("https://play.google.com/store/apps/details?id=com.example");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("play.google.com", comp.Markup);
        }

        [Fact]
        public void GeneraQr_Imagen()
        {
            // 1. Activa el modo test antes de renderizar
            AppEnvironment.IsTestMode = true;

            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1920);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button:contains('Muestra una imagen'").Click();
            //comp.Find("button:contains('Más tipo'").Click();
            comp.Find("button:contains('Muestra una imagen')").Click();
            comp.Find("input[placeholder='URL de la imagen']").Change("https://midominio.com/imagen.png");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("imagen.png", comp.Markup);

            // 2. (Opcional) Restaura al final
            AppEnvironment.IsTestMode = false;
        }

        [Fact]
        public void GeneraQr_Video()
        {
            // 1. Activa el modo test antes de renderizar
            AppEnvironment.IsTestMode = true;

            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1920);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Muestra un video.']").Click();
            comp.Find("button:contains('Muestra un video')").Click();
            comp.Find("input[placeholder='URL del video']").Change("https://video.com/v.mp4");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("v.mp4", comp.Markup);

            // 2. (Opcional) Restaura al final
            AppEnvironment.IsTestMode = false;
        }

        [Fact]
        public void GeneraQr_Social()
        {
            // 1. Activa el modo test antes de renderizar
            AppEnvironment.IsTestMode = true;

            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1920);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Perfil/red social.']").Click();
            comp.Find("button:contains('Perfil/red social')").Click();
            comp.Find("input[placeholder='URL del perfil social']").Change("https://twitter.com/user");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("twitter.com/user", comp.Markup);

            // 2. (Opcional) Restaura al final
            AppEnvironment.IsTestMode = false;
        }

        [Fact]
        public void GeneraQr_Evento()
        {
            // 1. Activa el modo test antes de renderizar
            AppEnvironment.IsTestMode = true;
            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1920);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Evento de calendario (iCal).']").Click();
            comp.Find("button:contains('Evento de calendario')").Click();
            comp.Find("input[placeholder='Título del evento']").Change("MiEvento");
            comp.Find("input[placeholder='Ubicación']").Change("MiLugar");
            comp.Find("input[placeholder='YYYY-MM-DD HH:mm']").Change("2025-01-01 09:00");
            comp.FindAll("input[placeholder='YYYY-MM-DD HH:mm']")[1].Change("2025-01-01 11:00");
            comp.Find("button:contains('Generar')").Click();
            //Assert.Contains("VEVENT", comp.Markup);
            Assert.Contains("MiEvento", comp.Markup);

            // 2. (Opcional) Restaura al final
            AppEnvironment.IsTestMode = false;
        }

        [Fact]
        public void GeneraQr_Barcode2D()
        {
            // 1. Activa el modo test antes de renderizar
            AppEnvironment.IsTestMode = true;

            using var ctx = new TestContext();

            // Mockea la función subscribeResize para que no truene la prueba
            ctx.JSInterop.SetupVoid("subscribeResize", _ => true);

            // Configura el JSInterop para el método JS que usa el componente
            ctx.JSInterop.Setup<int>("getWindowWidth").SetResult(1024);

            // Renderiza el componente
            //var comp = RenderComponent<QrCompGenerator>();
            var comp = ctx.RenderComponent<QrCompGenerator>();
            //comp.Find("button[title='Código de barras (DataMatrix, etc).']").Click();
            comp.Find("button:contains('Código de barras')").Click();
            comp.Find("input[placeholder='Texto o número']").Change("DATAMATRIX-2025");
            comp.Find("button:contains('Generar')").Click();
            Assert.Contains("DATAMATRIX-2025", comp.Markup);

            // 2. (Opcional) Restaura al final
            AppEnvironment.IsTestMode = false;
        }
    }
}
