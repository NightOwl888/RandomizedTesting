using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomizedTesting.Generators
{
    public class TestRandomExtensions
    {
        Random r;

        private class Int32Class
        {
            public int Value { get; set; }
        }

        [Test]
        public void TestNextInt64Range()
        {
            IDictionary<long, Int32Class> valueToCount = new Dictionary<long, Int32Class>();
            long minValue = 3000000000;
            long maxValue = 3000000010;
            int tries = 1000000 * 10;

            for (int i = 0; i < tries; i++)
            {
                long value = r.NextInt64(minValue, maxValue);
                if (valueToCount.TryGetValue(value, out Int32Class times))
                    times.Value++;
                else
                    valueToCount[value] = new Int32Class { Value = 1 };
            }


            long minRunValue = long.MaxValue, maxRunValue = long.MinValue;

            foreach (var count in valueToCount.Values)
            {
                if (minRunValue > count.Value)
                    minRunValue = count.Value;
                if (maxRunValue < count.Value)
                    maxRunValue = count.Value;
            }

            long maxDeviation = maxRunValue - minRunValue;

            double variance = (double)maxDeviation / (double)tries;
            if (variance > 0.01d)
                Assert.Fail($"Variance was higher than 1%: {variance}");
        }

        /**
         * @tests java.util.Random#nextBoolean()
         */
        [Test]
        public void Test_nextBoolean()
        {
            // Test for method boolean java.util.Random.nextBoolean()
            bool falseAppeared = false, trueAppeared = false;
            for (int counter = 0; counter < 100; counter++)
                if (r.NextBoolean())
                    trueAppeared = true;
                else
                    falseAppeared = true;
            Assert.IsTrue(falseAppeared, "Calling nextBoolean() 100 times resulted in all trues");
            Assert.IsTrue(trueAppeared, "Calling nextBoolean() 100 times resulted in all falses");
        }

        /**
        * @tests java.util.Random#nextFloat()
        */
        [Test]
        public void Test_nextFloat()
        {
            // Test for method float java.util.Random.nextFloat()
            float lastNum = r.NextSingle();
            float nextNum;
            bool someDifferent = false;
            bool inRange = true;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.NextSingle();
                if (nextNum != lastNum)
                    someDifferent = true;
                if (!(0 <= nextNum && nextNum < 1.0))
                    inRange = false;
                lastNum = nextNum;
            }
            Assert.IsTrue(someDifferent, "Calling nextFloat 100 times resulted in same number");
            Assert.IsTrue(inRange, "Calling nextFloat resulted in a number out of range [0,1)");
        }

        /**
         * @tests java.util.Random#nextGaussian()
         */
        [Test]
        public void Test_nextGaussian()
        {
            // Test for method double java.util.Random.nextGaussian()
            double lastNum = r.NextGaussian();
            double nextNum;
            bool someDifferent = false;
            bool someInsideStd = false;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.NextGaussian();
                if (nextNum != lastNum)
                    someDifferent = true;
                if (-1.0 <= nextNum && nextNum <= 1.0)
                    someInsideStd = true;
                lastNum = nextNum;
            }
            Assert.IsTrue(someDifferent, "Calling nextGaussian 100 times resulted in same number");
            Assert.IsTrue(someInsideStd, "Calling nextGaussian 100 times resulted in no number within 1 std. deviation of mean");
        }

        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        [SetUp]
        public void SetUp()
        {
            r = new Random();
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        [TearDown]
        public void TearDown()
        {
        }
    }
}
