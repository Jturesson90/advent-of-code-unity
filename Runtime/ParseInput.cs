using System.Linq;

namespace JTuresson.AdventOfCode
{
    public static class ParseInput
    {
        public static string[] ParseAsArray(string input, char separator = '\n') => input.Split(separator);

        public static int[] ParseAsIntArray(string input, char separator = '\n') =>
            ParseAsArray(input, separator).Select(int.Parse).ToArray();

        public static byte[] ParseAsByteArray(string input, char separator = '\n') =>
            ParseAsArray(input, separator).Select(byte.Parse).ToArray();

        public static long[] ParseAsLongArray(string input, char separator = '\n') =>
            ParseAsArray(input, separator).Select(long.Parse).ToArray();

        public static string[][] ParseAsMultiArray(string input, char separator = ',') => ParseAsArray(input)
            .Select((string row) => row.Split(separator).ToArray()).ToArray();
    }
}