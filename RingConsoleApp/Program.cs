using RingLibrary;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RingConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            ObjectRing ring = new ObjectRing();
            Console.WriteLine($"Початковий стан: {ring}");
            Console.WriteLine(ring ? "Кільце НЕ порожнє (true)" : "Кільце порожнє (false)");

            Console.WriteLine("\nДодавання елементів операцією '<'");
            ring = ring < new Product("Монітор LG", 8500.00);
            ring = ring < new Product("Клавіатура Keychron", 3200.50);
            ring = ring < new Product("Миша Logitech", 2100.00);
            ring = ring < new Product("Навушники Sony", 5400.00);

            Console.WriteLine(ring);
            Console.WriteLine(ring ? "Перевірка true/false: Кільце містить елементи (true)" : "Кільце порожнє (false)");

            Console.WriteLine($"\nЗчитування без вилучення (Peek): {ring.Peek()}");

            Console.WriteLine("\nОбертання кільця операціями '++' та '--'");
            ring++;
            Console.WriteLine($"Після ring++ (за годинниковою): {ring}");
            ring++;
            Console.WriteLine($"Після ще одного ring++:        {ring}");
            ring--;
            Console.WriteLine($"Після ring-- (проти годинникової): {ring}");

            Console.WriteLine("\nДоступ через індексатор");
            Console.WriteLine($"Елемент з індексом [0] (поточний): {ring[0]}");
            Console.WriteLine($"Елемент з індексом [2]:            {ring[2]}");

            Console.WriteLine("\nВилучення поточного елемента операцією '>'");
            Product extracted = new Product();
            ring = ring > extracted;
            Console.WriteLine($"Вилучено елемент: {extracted}");
            Console.WriteLine($"Кільце після вилучення: {ring}");

            Console.WriteLine("\nОперації приведення типів");
            int size = ring; 
            Console.WriteLine($"Неявне приведення до int (кількість елементів): {size}");

            Product[] productsArray = (Product[])ring;
            Console.WriteLine("Явне приведення до масиву Product[]:");
            for (int i = 0; i < productsArray.Length; i++)
            {
                Console.WriteLine($"  [{i}] = {productsArray[i]}");
            }

            Console.WriteLine("\nПорівняння кілець ('==' та '!=')");
            ObjectRing copyRing = new ObjectRing(ring);
            Console.WriteLine($"ring == copyRing: {ring == copyRing}");
            copyRing++;
            Console.WriteLine($"Після зсуву copyRing++ -> ring == copyRing: {ring == copyRing}");
            Console.WriteLine($"ring != copyRing: {ring != copyRing}");
        }
    }
}