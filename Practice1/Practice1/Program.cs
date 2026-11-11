using System;
using System.Collections.Generic;
using System.Linq;
using AircraftShop.Models;
using AircraftShop.Services;

namespace AircraftShop.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public CustomerCard? Card { get; set; }                
        public List<Order> Orders { get; set; } = new();       
    }

    public class CustomerCard
    {
        public int CustomerId { get; set; }                    
        public string CardNumber { get; set; } = "";
        public decimal DiscountPct { get; set; }
        public Customer? Customer { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<ModelKit> ModelKits { get; set; } = new(); 
    }

    public class ModelKit
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Scale { get; set; } = "";
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<ModelKitSupplier> ModelKitSuppliers { get; set; } = new(); 
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public List<OrderItem> Items { get; set; } = new();    
    }

    public class OrderItem                                    
    {
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ModelKitId { get; set; }
        public ModelKit? ModelKit { get; set; }
        public int Quantity { get; set; }
    }

    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<ModelKitSupplier> ModelKitSuppliers { get; set; } = new();
    }

    public class ModelKitSupplier                              
    {
        public int SupplierId { get; set; }
        public Supplier? Supplier { get; set; }
        public int ModelKitId { get; set; }
        public ModelKit? ModelKit { get; set; }
    }
}


namespace AircraftShop.Services
{
    public class InfoService
    {
        private readonly List<Customer> _customers;
        private readonly List<Category> _categories;
        private readonly List<ModelKit> _kits;
        private readonly List<Order> _orders;

        public InfoService(List<Customer> customers, List<Category> categories,
                           List<ModelKit> kits, List<Order> orders)
        {
            _customers = customers;
            _categories = categories;
            _kits = kits;
            _orders = orders;
        }

        public void PrintSummary()
        {
            Console.WriteLine("Aircraft Model Store");
            Console.WriteLine($"Categories: {_categories.Count}");
            Console.WriteLine($"Model kits: {_kits.Count}");
            Console.WriteLine($"Customers: {_customers.Count}");
            Console.WriteLine($"Orders: {_orders.Count}");
        }

        public void PrintCategoryKits(Category cat)
        {
            Console.WriteLine($"\n{cat.Name} model kits (1:N):");
            foreach (var k in cat.ModelKits)
                Console.WriteLine($"- {k.Name} ({k.Price:F0} uah)");
        }

        public void PrintCustomerDetails(Customer c)
        {
            Console.WriteLine($"\n{c.FullName} details (1:1):");
            Console.WriteLine($"- Card: {c.Card?.CardNumber ?? "none"}");
            Console.WriteLine($"- Discount: {c.Card?.DiscountPct ?? 0}%");
            Console.WriteLine($"- Orders (1:N): {c.Orders.Count}");
        }
        public void PrintOrderItems(Order o)
        {
            Console.WriteLine($"\nOrder #{o.Id} items (N:M):");
            foreach (var i in o.Items)
                Console.WriteLine($"- {i.ModelKit!.Name} x{i.Quantity}");
            Console.WriteLine($"- Total: {o.Items.Sum(i => i.Quantity * i.ModelKit!.Price):F0} uah");
        }

        public void PrintKitSuppliers(ModelKit k)
        {
            Console.WriteLine($"\n{k.Name} suppliers (N:M):");
            foreach (var ms in k.ModelKitSuppliers)
                Console.WriteLine($"- {ms.Supplier!.Name}");
        }
    }
}

namespace AircraftShop
{
    public static class Program
    {
        public static void Main()
        {
            var fighters = new Category { Id = 1, Name = "Fighter" };
            var bombers = new Category { Id = 2, Name = "Bomber" };
            var drones = new Category { Id = 3, Name = "Drone" };

            var f35 = new ModelKit { Id = 1, Name = "F-35A Lightning II", Scale = "1:48", Price = 1800, CategoryId = 1, Category = fighters };
            var f47 = new ModelKit { Id = 2, Name = "F-47 (NGAD)", Scale = "1:72", Price = 1200, CategoryId = 1, Category = fighters };
            var b21 = new ModelKit { Id = 3, Name = "B-21 Raider", Scale = "1:72", Price = 2100, CategoryId = 2, Category = bombers };
            var fq42 = new ModelKit { Id = 4, Name = "YFQ-42A", Scale = "1:48", Price = 950, CategoryId = 3, Category = drones };
            fighters.ModelKits.AddRange(new[] { f35, f47 });
            bombers.ModelKits.Add(b21);
            drones.ModelKits.Add(fq42);

            var c1 = new Customer { Id = 1, FullName = "Ivan Ivanenko" };
            var c2 = new Customer { Id = 2, FullName = "Olena Petrenko" };
            c1.Card = new CustomerCard { CustomerId = 1, CardNumber = "CARD-001", DiscountPct = 5, Customer = c1 };

            var o1 = new Order { Id = 1, OrderDate = DateTime.Now, CustomerId = 1, Customer = c1 };
            o1.Items.Add(new OrderItem { OrderId = 1, Order = o1, ModelKitId = 1, ModelKit = f35, Quantity = 1 });
            o1.Items.Add(new OrderItem { OrderId = 1, Order = o1, ModelKitId = 3, ModelKit = b21, Quantity = 2 });
            c1.Orders.Add(o1);

            var s1 = new Supplier { Id = 1, Name = "ModelHobby Ltd" };
            var s2 = new Supplier { Id = 2, Name = "SkyKits" };
            foreach (var (s, k) in new[] { (s1, f35), (s2, f35), (s1, b21) })
            {
                var link = new ModelKitSupplier { SupplierId = s.Id, Supplier = s, ModelKitId = k.Id, ModelKit = k };
                s.ModelKitSuppliers.Add(link);
                k.ModelKitSuppliers.Add(link);
            }

            var service = new InfoService(
                new List<Customer> { c1, c2 },
                new List<Category> { fighters, bombers, drones },
                new List<ModelKit> { f35, f47, b21, fq42 },
                new List<Order> { o1 });

            service.PrintSummary();
            service.PrintCategoryKits(fighters);   // 1:N
            service.PrintCustomerDetails(c1);      // 1:1
            service.PrintOrderItems(o1);           // N:M
            service.PrintKitSuppliers(f35);        // N:M
        }
    }
}