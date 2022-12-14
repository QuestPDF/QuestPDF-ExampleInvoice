using System;
using System.Linq;
using QuestPDF.Helpers;

namespace QuestPDF.ExampleInvoice
{
    public static class InvoiceDocumentDataSource
    {
        private static Random Random = new Random();
        
        public static InvoiceModel GetInvoiceDetails()
        {
            var items = Enumerable
                .Range(1, 25)
                .Select(_ => GenerateRandomOrderItem())
                .ToList();
            
            return new InvoiceModel
            {
                InvoiceNumber = Random.Next(1_000, 10_000),
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now + TimeSpan.FromDays(14),

                SellerAddress = GenerateRandomAddress(),
                CustomerAddress = GenerateRandomAddress(),
                
                Items = items,
                Comments = Placeholders.Paragraph()
            };
        }

        private static OrderItem GenerateRandomOrderItem()
        {
            return new OrderItem
            {
                Name = Placeholders.Label(),
                Price = (decimal) Math.Round(Random.NextDouble() * 100, 2),
                Quantity = Random.Next(1, 10)
            };
        }

        private static Address GenerateRandomAddress()
        {
            return new Address
            {
                CompanyName = Placeholders.Name(),
                Street = Placeholders.Label(),
                City = Placeholders.Label(),
                State = Placeholders.Label(),
                Email = Placeholders.Email(),
                Phone = Placeholders.PhoneNumber() 
            };
        }
    }
}