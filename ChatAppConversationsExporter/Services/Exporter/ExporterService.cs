using SelectPdf;
using System;
using System.IO;
using System.Reflection;

namespace WindowsFormsApp1.Services.Exporter
{
    public class ExporterService
    {
        public byte[] ExportToPdfFileBytes(string name, string identificationNumber, string chat)
        {
            string reportHTMLTemplate = GetTemplate("PDF");

            var finalContent = reportHTMLTemplate
                .Replace("{nomePerito}", name)
                .Replace("{numeroIdentificacao}", identificationNumber)
                .Replace("{dataGeracao}", $"{DateTime.Now: dd/MM/yyyy HH:mm:ss}")
                .Replace("{conversa}", chat);

            HtmlToPdf converter = new HtmlToPdf();

            // set converter options
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            converter.Options.WebPageWidth = 1240;
            converter.Options.WebPageHeight = 1754;

            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(finalContent);

            // save pdf document
            var res = doc.Save();

            // close pdf document
            doc.Close();

            return res;
        }

        public string ExportToHTMLFile(string name, string identificationNumber, string chat)
        {
            string reportHTMLTemplate = GetTemplate("HTML");

            var finalContent = reportHTMLTemplate
                .Replace("{nomePerito}", name)
                .Replace("{numeroIdentificacao}", identificationNumber)
                .Replace("{dataGeracao}", $"{DateTime.Now: dd/MM/yyyy HH:mm:ss}")
                .Replace("{conversa}", chat);

            return finalContent;            
        }

        private string GetTemplate(string reportType)
        {
            var html = string.Empty;

            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            var targetResource = Array.Find(resources, s => s.Contains($"{reportType}ReportTemplate.html"));
            var resourceStream = assembly.GetManifestResourceStream(targetResource);

            using (var reader = new StreamReader(resourceStream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }
    }
}
