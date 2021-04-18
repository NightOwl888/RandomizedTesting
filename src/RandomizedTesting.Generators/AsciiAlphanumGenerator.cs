namespace RandomizedTesting.Generators
{
    /// <summary>
    /// A generator emitting simple ASCII alphanumeric letters and numbers 
    /// from the set (newlines not counted):
    /// <para/>
    /// abcdefghijklmnopqrstuvwxyz
    /// ABCDEFGHIJKLMNOPQRSTUVWXYZ
    /// 0123456789
    /// </summary>
    public class AsciiAlphanumGenerator : CodepointSetGenerator
    {
        private readonly static char[] Chars =
          ("abcdefghijklmnopqrstuvwxyz" +
           "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
           "0123456789").ToCharArray();

        public AsciiAlphanumGenerator()
            : base(Chars)
        {
        }
    }
}
