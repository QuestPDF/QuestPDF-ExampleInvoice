using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;

namespace QuestPDF.ExampleInvoice
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Please make sure that you are eligible to use the Community license.
            // To learn more about the QuestPDF licensing, please visit:
            // https://www.questpdf.com/pricing.html
            Settings.License = LicenseType.Community;
            
            // For documentation and implementation details, please visit:
            // https://www.questpdf.com/documentation/getting-started.html
            var model = InvoiceDocumentDataSource.GetInvoiceDetails();
            var document = new InvoiceDocument(model);

            // Generate PDF file and show it in the default viewer
            document.GeneratePdfAndShow();
            
            // Or open the QuestPDF Previewer and experiment with the document's design
            // in real-time without recompilation after each code change
            // https://www.questpdf.com/document-previewer.html
            //document.ShowInPreviewer();
        }
    }
}