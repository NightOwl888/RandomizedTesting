using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RandomizedTesting.Generators
{
    public class TestRandomPicks : RandomizedTest
    {
        [Test]
        public void TestRandomFromEmptyCollection()
        {
            Assert.Throws<ArgumentException>(() => RandomPicks.RandomFrom(Random, new HashSet<object>()));
        }

        [Test]
        public void TestRandomFromCollection()
        {
            object t = new object();
            object r = RandomPicks.RandomFrom(Random, new HashSet<object>(new object[] { t }));
            Assert.AreSame(r, t);
        }

        [Test]
        public void TestRandomFromList()
        {
            Assert.Throws<ArgumentException>(() => RandomPicks.RandomFrom(Random, new List<object>()));
        }

        [Test]
        public void TestRandomFromArray()
        {
            Assert.Throws<ArgumentException>(() => RandomPicks.RandomFrom(Random, new object[] { }));
        }
    }
}
