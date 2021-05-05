using System.Linq;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
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

        public void Compose(IContainer container)
        {
            container
                .PaddingHorizontal(50)
                .PaddingVertical(50)
                .Page(page =>
                {
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().AlignCenter().PageNumber("Page {number}");
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().Stack(stack =>
                {
                    stack.Item().Text($"Invoice #{Model.InvoiceNumber}", TextStyle.Default.Size(20).Bold());
                    stack.Item().Text($"Issue date: {Model.IssueDate:d}");
                    stack.Item().Text($"Due date: {Model.DueDate:d}");
                });
                
                row.ConstantColumn(100).Height(50).Placeholder();
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Stack(column => 
            {
                column.Spacing(5);
                
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
            
            container.PaddingTop(10).Decoration(decoration =>
            {
                // header
                decoration.Header().BorderBottom(1).Padding(5).Row(row => 
                {
                    row.ConstantColumn(25).Text("#", headerStyle);
                    row.RelativeColumn(3).Text("Product", headerStyle);
                    row.RelativeColumn().AlignRight().Text("Unit price", headerStyle);
                    row.RelativeColumn().AlignRight().Text("Quantity", headerStyle);
                    row.RelativeColumn().AlignRight().Text("Total", headerStyle);
                });

                // content
                decoration
                    .Content()
                    .Stack(column =>
                    {
                        foreach (var item in Model.Items)
                        {
                            column.Item().BorderBottom(1).BorderColor("CCC").Padding(5).Row(row => 
                            {
                                row.ConstantColumn(25).Text(Model.Items.IndexOf(item) + 1);
                                row.RelativeColumn(3).Text(item.Name);
                                row.RelativeColumn().AlignRight().Text($"{item.Price}$");
                                row.RelativeColumn().AlignRight().Text(item.Quantity);
                                row.RelativeColumn().AlignRight().Text($"{item.Price * item.Quantity}$");
                            });
                        }
                    });
            });
        }

        void ComposeComments(IContainer container)
        {
            container.ShowEntire().Background("#EEE").Padding(10).Stack(message => 
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
                column.Spacing(5);

                column.Item().BorderBottom(1).PaddingBottom(5).Text(Title, TextStyle.Default.SemiBold());
                
                column.Item().Text(Address.CompanyName);
                column.Item().Text(Address.Street);
                column.Item().Text($"{Address.City}, {Address.State}");
                column.Item().Text(Address.Email);
                column.Item().Text(Address.Phone);
            });
        }
    }
}