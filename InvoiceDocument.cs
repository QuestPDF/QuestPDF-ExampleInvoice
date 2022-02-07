using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace QuestPDF.ExampleInvoice
{
    public class InvoiceDocument : IDocument
    {
        public InvoiceModel Model { get; }

        public InvoiceDocument(InvoiceModel model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);
                    
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.Size(20).SemiBold().Color(Colors.Blue.Medium);
            
            container.Row(row =>
            {
                row.RelativeColumn().Stack(stack =>
                {
                    stack.Item().Text($"Invoice #{Model.InvoiceNumber}", titleStyle);
                    
                    stack.Item().Text(text =>
                    {
                        text.Span("Issue date: ", TextStyle.Default.SemiBold());
                        text.Span($"{Model.IssueDate:d}");
                    });
                    
                    stack.Item().Text(text =>
                    {
                        text.Span("Due date: ", TextStyle.Default.SemiBold());
                        text.Span($"{Model.DueDate:d}");
                    });
                });
                
                row.ConstantColumn(100).Height(50).Placeholder();
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Stack(column => 
            {
                column.Spacing(20);
                
                column.Item().Row(row =>
                {
                    row.RelativeColumn().Component(new AddressComponent("From", Model.SellerAddress));
                    row.ConstantColumn(50);
                    row.RelativeColumn().Component(new AddressComponent("For", Model.CustomerAddress));
                });

                column.Item().Element(ComposeTable);

                var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);
                column.Item().PaddingRight(5).AlignRight().Text($"Grand total: {totalPrice}$", TextStyle.Default.SemiBold());

                if (!string.IsNullOrWhiteSpace(Model.Comments))
                    column.Item().PaddingTop(25).Element(ComposeComments);
            });
        }

        void ComposeTable(IContainer container)
        {
            var headerStyle = TextStyle.Default.SemiBold();
            
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });
                
                table.Header(header =>
                {
                    header.Cell().Text("#", headerStyle);
                    header.Cell().Text("Product", headerStyle);
                    header.Cell().AlignRight().Text("Unit price", headerStyle);
                    header.Cell().AlignRight().Text("Quantity", headerStyle);
                    header.Cell().AlignRight().Text("Total", headerStyle);
                    
                    header.Cell().ColumnSpan(5).PaddingTop(5).BorderBottom(1).BorderColor(Colors.Black);
                });
                
                foreach (var item in Model.Items)
                {
                    table.Cell().Element(CellStyle).Text(Model.Items.IndexOf(item) + 1);
                    table.Cell().Element(CellStyle).Text(item.Name);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price}$");
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity);
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Price * item.Quantity}$");
                    
                    static IContainer CellStyle(IContainer container) => container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                }
            });
        }

        void ComposeComments(IContainer container)
        {
            container.ShowEntire().Background(Colors.Grey.Lighten3).Padding(10).Stack(message => 
            {
                message.Spacing(5);
                message.Item().Text("Comments", TextStyle.Default.Size(14).SemiBold());
                message.Item().Text(Model.Comments);
            });
        }
    }
    
    public class AddressComponent : IComponent
    {
        private string Title { get; }
        private Address Address { get; }

        public AddressComponent(string title, Address address)
        {
            Title = title;
            Address = address;
        }
        
        public void Compose(IContainer container)
        {
            container.ShowEntire().Stack(column =>
            {
                column.Spacing(2);

                column.Item().Text(Title, TextStyle.Default.SemiBold());
                column.Item().PaddingBottom(5).LineHorizontal(1); 
                
                column.Item().Text(Address.CompanyName);
                column.Item().Text(Address.Street);
                column.Item().Text($"{Address.City}, {Address.State}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.Phone);
            });
        }
    }
}