using J2N;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace RandomizedTesting.Generators
{
    [TestFixture]
    public class TestCodepointSetGenerator
    {
        private readonly static int[] codepoints = {
            'a', 'b', 'c', 'd',
            0xd7ff,
            0xffff,
            0x10000,
            0x1D11E,
            0x10FFFD,
        };

        private readonly static int[] surrogates = {
            0x10000,
            0x1D11E,
            0x10FFFD,
        };

        private readonly static string withSurrogates = Character.ToString(codepoints, 0, codepoints.Length); //new string(codepoints, 0, codepoints.Length);

        public class CodepointSetOnChars : StringGeneratorTestBase
        {
            public CodepointSetOnChars()
                : base(new CodepointSetGenerator(new char[] {
                    'a', 'b', 'c', 'd',
                    (char)0x100,
                    (char)0xd7ff,
                    (char)0xffff
                }))
            {
            }

            [Test]
            public void testAllCharactersUsed()
            {
                char[] domain = "abcdefABCDEF".ToCharArray();
                ISet<char> chars = new HashSet<char>();
                foreach (char chr in domain)
                {
                    chars.Add(chr);
                }

                CodepointSetGenerator gen = new CodepointSetGenerator(domain);
                Random r = new Random(RandomInt32());
                for (int i = 0; i < 1000000 && chars.Count > 0; i++)
                {
                    foreach (char ch in gen.OfCodeUnitsLength(r, 100, 100).ToCharArray())
                    {
                        chars.Remove(ch);
                    }
                }

                Assert.AreEqual(0, chars.Count);
            }

            [Test]
            public void TestSurrogatesInConstructor()
            {
                Assert.Throws<ArgumentException>(() => new CodepointSetGenerator(withSurrogates.ToCharArray()));
            }
        }

        public class CodepointSetOnCodePoints : StringGeneratorTestBase
        {
            public CodepointSetOnCodePoints()
                : base(new CodepointSetGenerator(withSurrogates))
            {
            }

            [Test]

            public void TestAllCharactersUsed()
            {
                char[] domain = "abcdefABCDEF".ToCharArray();
                ISet<char> chars = new HashSet<char>();
                foreach (char chr in domain)
                {
                    chars.Add(chr);
                }

                CodepointSetGenerator gen = new CodepointSetGenerator(new string(domain));
                Random r = new Random(RandomInt32());
                for (int i = 0; i < 1000000 && chars.Count > 0; i++)
                {
                    foreach (char ch in gen.OfCodeUnitsLength(r, 100, 100).ToCharArray())
                    {
                        chars.Remove(ch);
                    }
                }

                Assert.AreEqual(0, chars.Count);
            }
        }

        public class CodepointSetOnSurrogatesOnly : StringGeneratorTestBase
        {
            public CodepointSetOnSurrogatesOnly()
                : base(new CodepointSetGenerator(Character.ToString(surrogates, 0, surrogates.Length)))
            {
            }

            [Test]
            public void TestOddCodePoints()
            {
                Assert.Throws<ArgumentException>(() => generator.OfCodeUnitsLength(Random, 3, 3));
            }

            protected override int IterationFix(int i)
            {
                return i & ~1;      // Even only.
            }
        }
    }
}
