Randomized Testing
=========

[![Nuget](https://img.shields.io/nuget/dt/RandomizedTesting.Generators)](https://www.nuget.org/packages/RandomizedTesting.Generators)
[![Azure DevOps builds (branch)](https://img.shields.io/azure-devops/build/NightOwl888/RandomizedTesting/3/release/v2.7.8)](https://dev.azure.com/NightOwl888/RandomizedTesting/_build?definitionId=3)
[![GitHub](https://img.shields.io/github/license/NightOwl888/RandomizedTesting)](https://github.com/NightOwl888/RandomizedTesting/blob/master/LICENSE.txt)
[![GitHub Sponsors](https://img.shields.io/badge/-Sponsor-fafbfc?logo=GitHub%20Sponsors)](https://github.com/sponsors/NightOwl888)

This is a C# port of the only the generators from the [Java randomizedtesting](https://github.com/randomizedtesting/randomizedtesting) library.

In addition to the random generators, the `RandomExtensions` class contains several useful low-level extension methods for generating numbers, text (html-like, regex-like, unicode, realistic unicode, plain ASCII), random picks from collections, or random regular expression instances.

Why would you want tests to have random data? This is to fill situations where it is not practical to test the entire range of values in a single test run, for example, with applications that analyze text. Tests can instead be designed to provide different input every time they run to catch edge cases that are difficult to test for. Generating random data also has many other purposes, such as quickly generating a set of text files to benchmark with without having to store several hundred MB worth of files (since the same random seed will always generate the same data).

## Random Generator Examples

```c#
// Create a System.Random instance
Random random = new Random();

// Create a random bool
bool boolValue = random.NextBoolean();

// Create a random long
long longValue = random.NextInt64();

// Create a random long between 10 (inclusive) and 100 (inclusive)
long longRangeValue = random.NextInt64(minValue: 10, maxValue: 100);

// Create a random float
float floatValue = random.NextSingle();

// Create a random BigInteger (from System.Numerics)
BigInteger bigIntValue = random.NextBigInteger();

// Pick a random KeyValuePair from a generic dictionary
var dictionary = new Dictionary<string, string>
{
    ["one"] = "oneValue",
    ["two"] = "twoValue",
    ["three"] = "threeValue",
    ["four"] = "fourValue",
    ["five"] = "fiveValue",
};
KeyValuePair<string, string> kvp = random.NextFrom(dictionary);

// Pick a random element from an array
var array = new int[] { 1, 4, 7, 10, 14, 74, 136 };
var arrayElement = random.RandomFrom(array);

// Create a random string (plain ASCII)
string asciiValue = random.NextSimpleString(maxLength: 15);

// Create a random string with specific chars
string specValue = random.NextSimpleStringRange(minChar: 'A', maxChar: 'F', maxLength: 10);

// Create a random Unicode string (may contain surrogate pairs)
string unicodeValue = random.NextUnicodeString(maxLength: 20);

// Create a random realistic Unicode string (random characters picked from the same Unicode block)
string realUnicodeValue = random.NextRealisticUnicodeString(maxLength: 30);

// Fill part of a char[] with random Unicode characters
char[] chars = "The quick brown fox jumped over the lazy dog.".ToCharArray();
random.NextFixedLengthUnicodeString(chars, startIndex: 8, length: 20);

// Generate a random regex-like string
string regexLike = random.NextRegexishString(maxLength: 20);

// Generate a random HTML-like string
string htmlLike = random.NextHtmlishString(numElements: 144);

// Randomly recase a string
string toRecase = "This Is A Pascal Cased String";
string recased = random.NextStringRecasing(toRecase);

// Generate a random Regex that compiles
Regex pattern = random.NextRegex();
```

## Saying Thanks

If you find this library to be useful, please star us [on GitHub](https://github.com/NightOwl888/RandomizedTesting/) and consider a financial sponsorship so we can continue bringing you great free tools like this one.

[![GitHub Sponsors](https://img.shields.io/badge/-Sponsor-fafbfc?logo=GitHub%20Sponsors)](https://github.com/sponsors/NightOwl888)
