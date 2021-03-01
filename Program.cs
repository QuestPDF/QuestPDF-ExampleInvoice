using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using QuestPDF.Fluent;

namespace QuestPDF.ExampleInvoice
{
    class Program
    {
        /// <summary>
        /// For documentation and implementation details, please visit:
        /// https://www.questpdf.com/documentation/getting-started.html
        /// </summary>
        static void Main(string[] args)
        {
            var filePath = "invoice.pdf";
            
            var model = InvoiceDocumentDataSource.GetInvoiceDetails();
            var document = new InvoiceDocument(model);
            document.GeneratePdf(filePath);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start("explorer.exe", filePath);
            }
            else
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                Console.WriteLine($"Output PDF file is available here: {fullPath}");
            }
        }
    }
}