using System;

namespace JTuresson.AdventOfCode
{
    public static class Algorithms
    {
        public static int ManhattanDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
        }
    }
}