using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace  BookManage.Models
{
    public class Book
    {
        public int id { get; set; }
        
        [StringLength(20)]
        public string Class { get; set; }
        
        [StringLength(20)]
        public string Title { get; set; }
        
        [StringLength(20)]
        public string Publish { get; set; }
        public int Pubyear { get; set; }
        [StringLength(20)]
        public string Author { get; set; }
        public double Price { get; set; }
        public int Total { get; set; }
        public int Stock { get; set; }

        public Book()
        {
            id = -1;
        }

        public Book(string cls, string title, string publish, int pubyear, string author, double price, int total,
            int stock)
        {
            id = -1;
            Class = cls;
            Title = title;
            Publish = publish;
            Pubyear = pubyear;
            Author = author;
            Price = price;
            Stock = stock;
        }
    }

    public class Query
    {
        [StringLength(20)]
        public string Class { get; set; }
        [StringLength(20)]
        public string Title { get; set; }
        [StringLength(20)]
        public string Publish { get; set; }
        public Tuple<int, int> Pubyear { get; set; }
        [StringLength(20)]
        public string Author { get; set; }
        public Tuple<double, double> Price { get; set; }
    }

    public class Person
    {
        public int id { get; set; }
        
        [StringLength(20)]
        public string Name { get; set; }
        
        [StringLength(20)]
        public string Company { get; set; }
        
        [StringLength(20)]
        public string Class { get; set; }
        
        [StringLength(20)]
        public string Password { get; set; }

        public Person()
        {
            id = -1;
        }
    }

    
}
//class, title, publish, pubyear, author, price, total, stock