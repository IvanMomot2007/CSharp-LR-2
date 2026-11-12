using System;
using System.IO;
using Xunit;
using RingLibrary;

namespace RingTests
{
    public class ObjectRingUnitTests
    {
        [Fact]
        public void Product_DefaultAndParameterizedConstructors_WorkCorrectly()
        {
            Product p1 = new Product();
            Assert.Equal("Невідомий товар", p1.Name);
            Assert.Equal(0.0, p1.Price);

            Product p2 = new Product("Телефон", 15000);
            Assert.Equal("Телефон", p2.Name);
            Assert.Equal(15000, p2.Price);

            Product p3 = new Product(p2);
            Assert.Equal(p2, p3);
            Assert.Equal(p2.GetHashCode(), p3.GetHashCode());
            Assert.Contains("Телефон", p2.ToString());
        }

        [Fact]
        public void Product_ValidationAndEquals_HandlesAllBranches()
        {
            Product p = new Product("Тест", 100);

            Assert.Throws<ArgumentException>(() => p.Name = "");
            Assert.Throws<ArgumentException>(() => p.Name = "   ");
            Assert.Throws<ArgumentOutOfRangeException>(() => p.Price = -10);
            Assert.Throws<ArgumentNullException>(() => new Product(null));

            Assert.False(p.Equals(null));
            Assert.False(p.Equals("не продукт"));
            Assert.False(p.Equals(new Product("Інший", 100)));
            Assert.False(p.Equals(new Product("Тест", 200)));
            Assert.True(p.Equals(new Product("Тест", 100)));
        }

        [Fact]
        public void Constructors_ValidAndInvalidArguments_CoverAllBranches()
        {
            ObjectRing empty = new ObjectRing();
            Assert.True(empty.IsEmpty);
            Assert.Equal(0, empty.Count);

            ObjectRing single = new ObjectRing(new Product("A", 10));
            Assert.False(single.IsEmpty);
            Assert.Equal(1, single.Count);

            Assert.Throws<ArgumentNullException>(() => new ObjectRing((Product)null));
            Assert.Throws<ArgumentNullException>(() => new ObjectRing((Product[])null));
            Assert.Throws<ArgumentException>(() => new ObjectRing(new Product[] { new Product("A", 1), null }));
            Assert.Throws<ArgumentNullException>(() => new ObjectRing((ObjectRing)null));

            ObjectRing copyOfEmpty = new ObjectRing(empty);
            Assert.True(copyOfEmpty.IsEmpty);
        }

        [Fact]
        public void Peek_EmptyAndNonEmpty_WorksAndThrows()
        {
            ObjectRing empty = new ObjectRing();
            Assert.Throws<InvalidOperationException>(() => empty.Peek());

            ObjectRing ring = new ObjectRing(new Product("A", 10));
            Assert.Equal("A", ring.Peek().Name);
        }

        [Fact]
        public void Indexer_GetAndSet_CoversAllBranchesAndExceptions()
        {
            ObjectRing empty = new ObjectRing();
            Assert.Throws<InvalidOperationException>(() => { var _ = empty[0]; });
            Assert.Throws<InvalidOperationException>(() => { empty[0] = new Product("A", 1); });

            ObjectRing ring = new ObjectRing(new[] { new Product("A", 1), new Product("B", 2) });

            Assert.Equal("A", ring[0].Name);
            Assert.Equal("B", ring[1].Name);

            ring[1] = new Product("C", 3);
            Assert.Equal("C", ring[1].Name);

            Assert.Throws<IndexOutOfRangeException>(() => { var _ = ring[-1]; });
            Assert.Throws<IndexOutOfRangeException>(() => { var _ = ring[5]; });
            Assert.Throws<IndexOutOfRangeException>(() => { ring[-1] = new Product("X", 1); });
            Assert.Throws<IndexOutOfRangeException>(() => { ring[5] = new Product("X", 1); });
            Assert.Throws<ArgumentNullException>(() => { ring[0] = null; });
        }

        [Fact]
        public void OperatorLess_ValidAndNullArguments_CoversAllBranches()
        {
            ObjectRing ring = new ObjectRing();
            ring = ring < new Product("A", 100) < new Product("B", 200);
            Assert.Equal(2, ring.Count);

            ObjectRing nullRing = null;
            Assert.Throws<ArgumentNullException>(() => { var _ = nullRing < new Product("A", 1); });
            Assert.Throws<ArgumentNullException>(() => { var _ = ring < null; });
        }

        [Fact]
        public void OperatorGreater_SingleMultipleAndExceptions_CoversAllBranches()
        {
            ObjectRing empty = new ObjectRing();
            Product extracted = new Product();
            ObjectRing nullRing = null;

            Assert.Throws<ArgumentNullException>(() => { var _ = nullRing > extracted; });
            Assert.Throws<ArgumentNullException>(() => { var _ = empty > null; });
            Assert.Throws<InvalidOperationException>(() => { var _ = empty > extracted; });

            ObjectRing single = new ObjectRing(new Product("OnlyOne", 99));
            single = single > extracted;
            Assert.Equal("OnlyOne", extracted.Name);
            Assert.True(single.IsEmpty);

            ObjectRing multi = new ObjectRing(new[] { new Product("A", 1), new Product("B", 2), new Product("C", 3) });
            multi = multi > extracted;
            Assert.Equal("A", extracted.Name);
            Assert.Equal(2, multi.Count);
            Assert.Equal("B", multi.Peek().Name);
        }

        [Fact]
        public void OperatorsIncrementAndDecrement_CoverAllBranches()
        {
            ObjectRing nullRing = null;
            Assert.Throws<ArgumentNullException>(() => { nullRing++; });
            Assert.Throws<ArgumentNullException>(() => { nullRing--; });

            ObjectRing empty = new ObjectRing();
            empty++;
            empty--;
            Assert.True(empty.IsEmpty);

            ObjectRing ring = new ObjectRing(new[] { new Product("A", 1), new Product("B", 2) });
            ring++;
            Assert.Equal("B", ring.Peek().Name);
            ring--;
            Assert.Equal("A", ring.Peek().Name);
        }

        [Fact]
        public void EqualityAndHashCode_CoverAllBranches()
        {
            ObjectRing r1 = new ObjectRing(new[] { new Product("A", 1), new Product("B", 2) });
            ObjectRing r2 = new ObjectRing(new[] { new Product("A", 1), new Product("B", 2) });
            ObjectRing r3 = new ObjectRing(new[] { new Product("A", 1), new Product("C", 3) });
            ObjectRing rDiffCount = new ObjectRing(new Product("A", 1));
            ObjectRing empty1 = new ObjectRing();
            ObjectRing empty2 = new ObjectRing();
            ObjectRing nullRing = null;

            Assert.True(r1 == r1);
            Assert.False(r1 == nullRing);
            Assert.False(nullRing == r1);
            Assert.True(nullRing == null);

            Assert.True(r1 == r2);
            Assert.False(r1 != r2);
            Assert.True(r1 != r3);
            Assert.False(r1.Equals(rDiffCount));
            Assert.False(r1.Equals("не кольцо"));
            Assert.True(empty1.Equals(empty2));

            Assert.Equal(r1.GetHashCode(), r2.GetHashCode());
            Assert.Equal(empty1.GetHashCode(), empty2.GetHashCode());
        }


        [Fact]
        public void TrueFalseAndCasts_CoverAllBranches()
        {
            ObjectRing nullRing = null;
            ObjectRing empty = new ObjectRing();
            ObjectRing filled = new ObjectRing(new[] { new Product("A", 1), new Product("B", 2) });

            Assert.False(nullRing ? true : false);
            Assert.False(empty ? true : false);
            Assert.True(filled ? true : false);

            var opFalseMethod = typeof(ObjectRing).GetMethod("op_False");
            Assert.True((bool)opFalseMethod.Invoke(null, new object[] { nullRing }));
            Assert.True((bool)opFalseMethod.Invoke(null, new object[] { empty }));
            Assert.False((bool)opFalseMethod.Invoke(null, new object[] { filled }));

            int nullCount = nullRing;
            int filledCount = filled;
            Assert.Equal(0, nullCount);
            Assert.Equal(2, filledCount);

            Product[] emptyArr = (Product[])empty;
            Product[] nullArr = (Product[])nullRing;
            Product[] filledArr = (Product[])filled;
            Assert.Empty(emptyArr);
            Assert.Empty(nullArr);
            Assert.Equal(2, filledArr.Length);

            ObjectRing fromArr = (ObjectRing)filledArr;
            Assert.Equal(filled, fromArr);
        }

        [Fact]
        public void ToString_EmptyAndFilled_ReturnsFormattedString()
        {
            ObjectRing empty = new ObjectRing();
            Assert.Equal("[Кільце порожнє]", empty.ToString());

            ObjectRing filled = new ObjectRing(new[] { new Product("A", 1), new Product("B", 2) });
            string str = filled.ToString();
            Assert.Contains("Поточний -> A", str);
            Assert.Contains("<-> [B", str);
        }

        [Fact]
        public void Input_ValidAndInvalidConsoleInput_CoversAllBranches()
        {
            string validInput = "1\nНоутбук\n25000\n";
            Console.SetIn(new StringReader(validInput));
            ObjectRing ring = new ObjectRing();
            ring.Input();
            Assert.Equal(1, ring.Count);
            Assert.Equal("Ноутбук", ring.Peek().Name);

            Console.SetIn(new StringReader("не_число\n"));
            Assert.Throws<FormatException>(() => ring.Input());

            Console.SetIn(new StringReader("-5\n"));
            Assert.Throws<FormatException>(() => ring.Input());

            Console.SetIn(new StringReader("1\nТовар\nне_ціна\n"));
            Assert.Throws<FormatException>(() => ring.Input());
        }
    }
}