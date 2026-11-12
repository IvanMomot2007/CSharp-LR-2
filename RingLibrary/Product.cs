using System;
namespace RingLibrary
{
    public class Product
    {
        private string name;
        private double price;

        public string Name
        {
            get => name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва товару не може бути порожньою.");
                name = value;
            }
        }

        public double Price
        {
            get => price;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Price), "Ціна не може бути від'ємною.");
                price = value;
            }
        }

        public Product() 
        {
            name = "Невідомий товар";
            price = 0.0;
        }

        public Product(string name, double price)
        {
            Name = name;
            Price = price;
        }

        public Product(Product other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));
            name = other.name;
            price = other.price;
        }

        public override bool Equals(object obj)
        {
            if (obj is Product other)
            {
                return name == other.name && Math.Abs(price - other.price) < 1e-6;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(name, Math.Round(price, 4));
        }

        public override string ToString()
        {
            return $"{Name} ({Price:F2} грн)";
        }
    }
}