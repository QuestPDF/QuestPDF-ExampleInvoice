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
                    page.Header(ComposeHeader);
                    page.Content(ComposeContent);
                    page.Footer().AlignCenter().PageNumber("Page {number}");
                });
        }

        void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeColumn().Stack(column =>
                {
                    column.Element().Text($"Invoice #{Model.InvoiceNumber}", TextStyle.Default.Size(20).Bold());
                    column.Element().Text($"Issue date: {Model.IssueDate:d}");
                    column.Element().Text($"Due date: {Model.DueDate:d}");
                });
                
                row.ConstantColumn(100).Height(50).Placeholder();
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).PageableStack(column => 
            {
                column.Spacing(5);
                
                column.Element().Row(row =>
                {
                    row.RelativeColumn().Component(new AddressComponent("From", Model.SellerAddress));
                    row.ConstantColumn(50);
                    row.RelativeColumn().Component(new AddressComponent("For", Model.CustomerAddress));
                });

                column.Element(ComposeTable);

                var totalPrice = Model.Items.Sum(x => x.Price * x.Quantity);
                column.Element().AlignRight().Text($"Grand total: {totalPrice}$", TextStyle.Default.SemiBold());

                if (!string.IsNullOrWhiteSpace(Model.Comments))
                    column.Element().PaddingTop(25).Element(ComposeComments);
            });
        }

        void ComposeTable(IContainer container)
        {
            container.PaddingTop(10).Section(section =>
            {
                // header
                section.Header().BorderBottom(1).Padding(5).Row(row => 
                {
                    row.ConstantColumn(25).Text("#", TextStyle.Default.SemiBold());
                    row.RelativeColumn(3).Text("Product", TextStyle.Default.SemiBold());
                    row.RelativeColumn().AlignRight().Text("Unit price", TextStyle.Default.SemiBold());
                    row.RelativeColumn().AlignRight().Text("Quantity", TextStyle.Default.SemiBold());
                    row.RelativeColumn().AlignRight().Text("Total", TextStyle.Default.SemiBold());
                });

                // content
                section
                    .Content()
                    .PageableStack(column =>
                    {
                        foreach (var item in Model.Items)
                        {
                            column.Element().BorderBottom(1).BorderColor("CCC").Padding(5).Row(row => 
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
            container.Background("#EEE").Padding(10).Stack(message => 
            {
                message.Spacing(5);
                message.Element().Text("Comments", TextStyle.Default.Size(14).SemiBold());
                message.Element().Text(Model.Comments);
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
            container.Stack(column =>
            {
                column.Spacing(5);

                column.Element().BorderBottom(1).PaddingBottom(5).Text(Title, TextStyle.Default.SemiBold());
                
                column.Element().Text(Address.CompanyName);
                column.Element().Text(Address.Street);
                column.Element().Text($"{Address.City}, {Address.State}");
                column.Element().Text(Address.Email);
                column.Element().Text(Address.Phone);
            });
        }
    }
}