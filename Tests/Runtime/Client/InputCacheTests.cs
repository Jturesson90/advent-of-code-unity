using AdventOfCode;
using NUnit.Framework;
using UnityEngine;

namespace Client
{
    public class InputCacheTests
    {
        [Test]
        public void CanAddAndRemove()
        {
            // Arrange
            var cache = ScriptableObject.CreateInstance<AdventOfCodeCache>();
            cache.AddInput(1514, 2, "hej");
            var hej = cache.GetInput(1514, 2);
            Assert.AreEqual(hej, "hej");
            cache.DeleteDay(1514, 2);
            var f = cache.HasInput(1514, 2);
            Assert.IsFalse(f);
        }
    }
}