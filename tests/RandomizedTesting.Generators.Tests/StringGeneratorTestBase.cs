using J2N;
using NUnit.Framework;
using System;

namespace RandomizedTesting.Generators
{
    public abstract class StringGeneratorTestBase : RandomizedTest
    {
        protected readonly StringGenerator generator;

        protected StringGeneratorTestBase(StringGenerator generator)
        {
            this.generator = generator;
        }

        [Test]
        [Repeat(10)]
        public void CheckFixedCodePointLength()
        {
            int codepoints = IterationFix(RandomInt32Between(1, 100));
            string s = generator.OfCodePointsLength(Random, codepoints, codepoints);
            Assert.AreEqual(codepoints, s.CodePointCount(0, s.Length), s);
        }

        [Test]
        [Repeat(10)]
        public void CheckRandomCodePointLength()
        {
            int from = IterationFix(RandomInt32Between(1, 100));
            int to = from + RandomInt32Between(0, 100);

            string s = generator.OfCodePointsLength(Random, from, to);
            int codepoints = s.CodePointCount(0, s.Length);
            Assert.IsTrue(from <= codepoints && codepoints <= to, codepoints + " not within " +
                from + "-" + to);
        }

        [Test]
        [Repeat(10)]
        public void CheckFixedCodeUnitLength()
        {
            int codeunits = IterationFix(RandomInt32Between(1, 100));
            string s = generator.OfCodeUnitsLength(Random, codeunits, codeunits);
            Assert.AreEqual(codeunits, s.Length, s);
            Assert.AreEqual(codeunits, s.ToCharArray().Length, s);
        }

        [Test]
        [Repeat(10)]
        public void CheckRandomCodeUnitLength()
        {
            int from = IterationFix(RandomInt32Between(1, 100));
            int to = from + RandomInt32Between(0, 100);

            string s = generator.OfCodeUnitsLength(Random, from, to);
            int codeunits = s.Length;
            Assert.IsTrue(from <= codeunits && codeunits <= to, codeunits + " not within " +
                from + "-" + to);
        }

        [Test]
        [Repeat(10)]
        public void CheckZeroLength()
        {
            Assert.AreEqual(generator.OfCodePointsLength(Random, 0, 0), "");
            Assert.AreEqual(generator.OfCodeUnitsLength(Random, 0, 0), "");
        }

        /// <summary>
        /// Correct the count if a given generator doesn't support all possible values (in tests).
        /// </summary>
        protected virtual int IterationFix(int i)
        {
            return i;
        }
    }
}
