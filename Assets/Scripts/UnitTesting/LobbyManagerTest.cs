// using NUnit.Framework;
// using UnityEngine;
//
// namespace KitchenKrapper.Tests
// {
//     public class LobbyManagerTests
//     {
//         private LobbyManager lobbyManager;
//
//         [SetUp]
//         public void Setup()
//         {
//             GameObject gameObject = new GameObject();
//             lobbyManager = gameObject.AddComponent<LobbyManager>();
//         }
//
//         [TearDown]
//         public void Teardown()
//         {
//             Object.Destroy(lobbyManager.gameObject);
//         }
//
//         [Test]
//         public void TestCreateLobby()
//         {
//             // Arrange
//             int expectedMaxPlayers = 4;
//
//             // Act
//             lobbyManager.CreateLobby();
//
//             // Assert
//             Assert.IsNotNull(lobbyManager.GetCurrentLobby());
//             Assert.AreEqual(expectedMaxPlayers, lobbyManager.GetCurrentLobby().MaxNumLobbyMembers);
//         }
//
//         [Test]
//         public void TestJoinLobby()
//         {
//             // Arrange
//             Lobby lobby = new Lobby();
//             LobbyDetails lobbyDetails = new LobbyDetails();
//
//             // Act
//             lobbyManager.JoinLobby(lobby, lobbyDetails);
//
//             // Assert
//             Assert.IsNotNull(lobbyManager.GetCurrentLobby());
//             Assert.AreEqual(lobby, lobbyManager.GetCurrentLobby());
//         }
//
//         [Test]
//         public void TestLeaveLobby()
//         {
//             // Arrange
//             lobbyManager.CreateLobby();
//
//             // Act
//             lobbyManager.LeaveLobby();
//
//             // Assert
//             Assert.IsNull(lobbyManager.GetCurrentLobby());
//         }
//     }
// }