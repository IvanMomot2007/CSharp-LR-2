using System;
using System.Collections.Generic;
using System.Text;

namespace RingLibrary
{
    public class ObjectRing
    {
        private class Node
        {
            public Product Data { get; set; }
            public Node Next { get; set; }
            public Node Prev { get; set; }

            public Node(Product data)
            {
                Data = new Product(data);
                Next = this;
                Prev = this;
            }
        }

        private Node current;
        private int count;

        public int Count => count;

        public bool IsEmpty => current == null;

        public ObjectRing()
        {
            current = null;
            count = 0;
        }

        public ObjectRing(Product initialItem)
        {
            if (initialItem == null)
                throw new ArgumentNullException(nameof(initialItem));

            current = new Node(initialItem);
            count = 1;
        }

        public ObjectRing(Product[] items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            current = null;
            count = 0;

            foreach (var item in items)
            {
                if (item == null)
                    throw new ArgumentException("Масив не повинен містити null-елементів.");
                InsertNode(item);
            }
        }

        public ObjectRing(ObjectRing other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            current = null;
            count = 0;

            if (other.current != null)
            {
                Node temp = other.current;
                for (int i = 0; i < other.count; i++)
                {
                    InsertNode(temp.Data);
                    temp = temp.Next;
                }
            }
        }

        private void InsertNode(Product item)
        {
            Node newNode = new Node(item);
            if (current == null)
            {
                current = newNode;
            }
            else
            {
                Node tail = current.Prev;
                tail.Next = newNode;
                newNode.Prev = tail;
                newNode.Next = current;
                current.Prev = newNode;
            }
            count++;
        }

        public Product Peek()
        {
            if (current == null)
                throw new InvalidOperationException("Кільце порожнє. Неможливо зчитати елемент.");

            return new Product(current.Data);
        }

        public Product this[int index]
        {
            get
            {
                if (current == null)
                    throw new InvalidOperationException("Кільце порожнє.");
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException("Індекс виходить за межі кількості елементів кільця.");

                Node temp = current;
                for (int i = 0; i < index; i++)
                    temp = temp.Next;

                return new Product(temp.Data);
            }
            set
            {
                if (current == null)
                    throw new InvalidOperationException("Кільце порожнє.");
                if (index < 0 || index >= count)
                    throw new IndexOutOfRangeException("Індекс виходить за межі кількості елементів кільця.");
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                Node temp = current;
                for (int i = 0; i < index; i++)
                    temp = temp.Next;

                temp.Data = new Product(value);
            }
        }

        public static ObjectRing operator <(ObjectRing ring, Product item)
        {
            if (ring is null)
                throw new ArgumentNullException(nameof(ring));
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            ObjectRing result = new ObjectRing(ring);
            result.InsertNode(item);
            return result;
        }

        public static ObjectRing operator >(ObjectRing ring, Product extractedItem)
        {
            if (ring is null)
                throw new ArgumentNullException(nameof(ring));
            if (extractedItem is null)
                throw new ArgumentNullException(nameof(extractedItem));
            if (ring.current == null)
                throw new InvalidOperationException("Кільце порожнє. Неможливо вилучити елемент.");

            extractedItem.Name = ring.current.Data.Name;
            extractedItem.Price = ring.current.Data.Price;

            ObjectRing result = new ObjectRing();
            if (ring.count == 1)
            {
                return result;
            }

            Node temp = ring.current.Next;
            for (int i = 1; i < ring.count; i++)
            {
                result.InsertNode(temp.Data);
                temp = temp.Next;
            }

            return result;
        }

        public static ObjectRing operator ++(ObjectRing ring)
        {
            if (ring is null)
                throw new ArgumentNullException(nameof(ring));

            ObjectRing result = new ObjectRing(ring);
            if (result.current != null)
            {
                result.current = result.current.Next;
            }
            return result;
        }

        public static ObjectRing operator --(ObjectRing ring)
        {
            if (ring is null)
                throw new ArgumentNullException(nameof(ring));

            ObjectRing result = new ObjectRing(ring);
            if (result.current != null)
            {
                result.current = result.current.Prev;
            }
            return result;
        }

        public static bool operator ==(ObjectRing a, ObjectRing b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(ObjectRing a, ObjectRing b)
        {
            return !(a == b);
        }

        public static bool operator true(ObjectRing ring)
        {
            return ring != null && ring.current != null;
        }

        public static bool operator false(ObjectRing ring)
        {
            return ring == null || ring.current == null;
        }

        public static implicit operator int(ObjectRing ring)
        {
            return ring == null ? 0 : ring.count;
        }

        public static explicit operator Product[](ObjectRing ring)
        {
            if (ring == null || ring.current == null)
                return Array.Empty<Product>();

            Product[] array = new Product[ring.count];
            Node temp = ring.current;
            for (int i = 0; i < ring.count; i++)
            {
                array[i] = new Product(temp.Data);
                temp = temp.Next;
            }
            return array;
        }

        public static explicit operator ObjectRing(Product[] array)
        {
            return new ObjectRing(array);
        }

        public void Input()
        {
            Console.Write("Введіть кількість елементів для додавання в кільце: ");
            if (!int.TryParse(Console.ReadLine(), out int n) || n < 0)
                throw new FormatException("Некоректна кількість елементів.");

            for (int i = 0; i < n; i++)
            {
                Console.Write($"Товар #{i + 1} - Назва: ");
                string name = Console.ReadLine();

                Console.Write($"Товар #{i + 1} - Ціна: ");
                if (!double.TryParse(Console.ReadLine(), out double price))
                    throw new FormatException("Некоректне значення ціни.");

                InsertNode(new Product(name, price));
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ObjectRing other))
                return false;
            if (count != other.count)
                return false;
            if (count == 0)
                return true;

            Node tempA = current;
            Node tempB = other.current;
            for (int i = 0; i < count; i++)
            {
                if (!tempA.Data.Equals(tempB.Data))
                    return false;
                tempA = tempA.Next;
                tempB = tempB.Next;
            }
            return true;
        }

        public override int GetHashCode()
        {
            int hash = count.GetHashCode();
            if (current != null)
            {
                hash = HashCode.Combine(hash, current.Data.GetHashCode());
            }
            return hash;
        }

        public override string ToString()
        {
            if (current == null)
                return "[Кільце порожнє]";

            StringBuilder sb = new StringBuilder();
            Node temp = current;
            for (int i = 0; i < count; i++)
            {
                if (i == 0)
                    sb.Append($"[Поточний -> {temp.Data}]");
                else
                    sb.Append($" <-> [{temp.Data}]");
                temp = temp.Next;
            }
            sb.Append(" (замкнено в кільце)");
            return sb.ToString();
        }
    }
}
