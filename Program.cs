using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using QuestPDF.Fluent;
using QuestPDF.Previewer;

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
            var model = InvoiceDocumentDataSource.GetInvoiceDetails();
            var document = new InvoiceDocument(model);

            // Generate PDF file and show it in the default viewer
            GenerateDocumentAndShow(document);
            
            // Or open the QuestPDF Previewer and experiment with the document's design
            // in real-time without recompilation after each code change
            //document.ShowInPreviewer();
        }

        static void GenerateDocumentAndShow(InvoiceDocument document)
        {
            const string filePath = "invoice.pdf";
            
            document.GeneratePdf(filePath);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            };

            process.Start();
        }
    }
}