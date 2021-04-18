using NUnit.Framework;

namespace RandomizedTesting.Generators
{
    public class TestRandomBytes : RandomizedTest
    {
        [Test]
        [Repeat(100)]
        public void TestRandomBytes_()
        {
            int len = RandomInt32Between(0, 100);
            Assert.IsTrue(RandomBytesOfLength(len).Length == len);
        }

        [Test]
        [Repeat(100)]
        public void TestRandomBytesOfLength()
        {
            int min = RandomInt32Between(0, 100);
            int max = min + RandomInt32Between(0, 10);

            byte[] bytes = RandomBytesOfLength(min, max);
            Assert.IsTrue(bytes.Length >= min);
            Assert.IsTrue(bytes.Length <= max);
        }
    }
}
