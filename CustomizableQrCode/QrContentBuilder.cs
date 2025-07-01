using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomizableQrCode
{
    public static class QrContentBuilder
    {
        public static string BuildLink(string url) =>
            url?.Trim() ?? "";

        public static string BuildText(string text) =>
            text?.Trim() ?? "";

        public static string BuildEmail(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to)) return "";
            var subjectParam = string.IsNullOrWhiteSpace(subject) ? "" : $"subject={Uri.EscapeDataString(subject)}";
            var bodyParam = string.IsNullOrWhiteSpace(body) ? "" : $"body={Uri.EscapeDataString(body)}";
            var sep = (!string.IsNullOrEmpty(subjectParam) && !string.IsNullOrEmpty(bodyParam)) ? "&" : "";
            var query = (subjectParam + sep + bodyParam);
            return $"mailto:{to}" + (string.IsNullOrEmpty(query) ? "" : $"?{query}");
        }

        public static string BuildCall(string phone) =>
            string.IsNullOrWhiteSpace(phone) ? "" : $"tel:{phone}";

        public static string BuildSMS(string phone, string message)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";
            var msg = string.IsNullOrWhiteSpace(message) ? "" : $"?body={Uri.EscapeDataString(message)}";
            return $"sms:{phone}{msg}";
        }

        public static string BuildVCard(string firstName, string lastName, string phone, string email, string company, string job, string address)
        {
            return
$@"BEGIN:VCARD
VERSION:3.0
N:{lastName};{firstName}
FN:{firstName} {lastName}
ORG:{company}
TITLE:{job}
TEL:{phone}
EMAIL:{email}
ADR:{address}
END:VCARD";
        }

        public static string BuildWhatsApp(string phone, string message)
        {
            if (string.IsNullOrWhiteSpace(phone)) return "";
            var url = $"https://wa.me/{phone}";
            if (!string.IsNullOrWhiteSpace(message))
                url += $"?text={Uri.EscapeDataString(message)}";
            return url;
        }

        public static string BuildWiFi(string ssid, string password, string encryption)
        {
            if (string.IsNullOrWhiteSpace(ssid)) return "";
            return $"WIFI:T:{encryption};S:{ssid};P:{password};;";
        }

        public static string BuildPDF(string url) =>
            url?.Trim() ?? "";

        public static string BuildApp(string url) =>
            url?.Trim() ?? "";

        public static string BuildImage(string url) =>
            url?.Trim() ?? "";

        public static string BuildVideo(string url) =>
            url?.Trim() ?? "";

        public static string BuildSocial(string url) =>
            url?.Trim() ?? "";

        public static string BuildEvent(string title, string location, string start, string end)
        {
            return
$@"BEGIN:VEVENT
SUMMARY:{title}
LOCATION:{location}
DTSTART:{start}
DTEND:{end}
END:VEVENT";
        }

        public static string BuildBarcode2D(string content) =>
            content?.Trim() ?? "";


        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                // Usa System.Net.Mail si quieres ser estricto
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(
                phone, @"^\+?\d{7,15}$");
        }

        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            return Uri.TryCreate(url, UriKind.Absolute, out var temp)
                && (temp.Scheme == Uri.UriSchemeHttp || temp.Scheme == Uri.UriSchemeHttps);
        }
    }
}
