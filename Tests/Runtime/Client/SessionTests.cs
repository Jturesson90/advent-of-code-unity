using System.Threading.Tasks;
using JTuresson.AdventOfCode;
using JTuresson.AdventOfCode.AOCClient;
using NUnit.Framework;
using UnityEngine.Networking;

namespace Client
{
    public class SessionTests
    {
        [Test]
        public async Task SessionId_IsValid()
        {
            // Arrange
            var aocClient = new AdventOfCodeClient(AdventOfCodeSettings.Instance, AdventOfCodeSettings.Instance.GetCache());
            // Act
            var result = await aocClient.SessionIsValid();
            // Assert*/
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SessionId_CanGetInput()
        {
            // Arrange
            var aocClient = new AdventOfCodeClient(AdventOfCodeSettings.Instance,AdventOfCodeSettings.Instance.GetCache());
            // Act
            var result = await aocClient.CanGetDay(1);
            // Assert*/
            Assert.IsTrue(result);
        }

        [Test]
        public async Task SessionId_CanGetDescription()
        {
            // Arrange
            var aocClient = new AdventOfCodeClient(AdventOfCodeSettings.Instance, AdventOfCodeSettings.Instance.GetCache());
            // Act
            var result = await aocClient.LoadDescription(1);
            // Assert*/
            Assert.IsFalse(result.Equals(string.Empty));
        }
    }
}