using NUnit.Framework;
using UnityEngine;

namespace KitchenKrapper.Tests
{
    public class MatchmakingManagerTests
    {
        private MatchmakingManager matchmakingManager;

        [SetUp]
        public void Setup()
        {
            matchmakingManager = new GameObject().AddComponent<MatchmakingManager>();
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(matchmakingManager.gameObject);
        }

        [Test]
        public void StartMatchmaking_WhenNotInQueue_ShouldSetIsInQueueToTrue()
        {
            // Arrange
            MatchType matchType = MatchType.Ranked;

            // Act
            matchmakingManager.StartMatchmaking(matchType);

            // Assert
            Assert.IsTrue(matchmakingManager.IsInQueue);
        }

        [Test]
        public void StartMatchmaking_WhenAlreadyInQueue_ShouldNotStartMatchmaking()
        {
            // Arrange
            MatchType matchType = MatchType.Ranked;
            matchmakingManager.StartMatchmaking(matchType);

            // Act
            matchmakingManager.StartMatchmaking(matchType);

            // Assert
            Assert.IsTrue(matchmakingManager.IsInQueue);
        }

        [Test]
        public void CancelMatchmaking_WhenNotInQueue_ShouldNotCancelMatchmaking()
        {
            // Arrange
            MatchType matchType = MatchType.Ranked;

            // Act
            matchmakingManager.CancelMatchmaking();

            // Assert
            Assert.IsFalse(matchmakingManager.IsInQueue);
        }

        [Test]
        public void CancelMatchmaking_WhenInQueue_ShouldSetIsInQueueToFalse()
        {
            // Arrange
            MatchType matchType = MatchType.Ranked;
            matchmakingManager.StartMatchmaking(matchType);

            // Act
            matchmakingManager.CancelMatchmaking();

            // Assert
            Assert.IsFalse(matchmakingManager.IsInQueue);
        }
    }
}