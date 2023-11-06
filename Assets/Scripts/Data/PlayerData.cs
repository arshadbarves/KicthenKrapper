using System;
using Unity.Netcode;
using Unity.Collections;

namespace KitchenKrapper
{
    public enum PlayerSessionState
    {
        None,
        Joined,
        Left,
        Kicked,
        Banned,
        Disconnected,
        Failed,
        Queued,
        Connecting,
        Connected,
        Timedout,
        Aborted,
        Initialized,
        SessionEnded,
        Destroyed
    }

    public enum PlayerGameState : byte
    {
        NotReady,
        Ready
    }

    [Serializable]
    public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
    {
        public ulong clientId;
        public PlayerGameState playerGameState;
        public PlayerSessionState playerSessionState;
        public FixedString64Bytes playerName;
        public FixedString64Bytes playerId;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref playerGameState);
            serializer.SerializeValue(ref playerSessionState);
            serializer.SerializeValue(ref playerName);
            serializer.SerializeValue(ref playerId);
        }

        public bool Equals(PlayerData other)
        {
            return clientId == other.clientId
                && playerGameState == other.playerGameState
                && playerSessionState == other.playerSessionState
                && playerName.Equals(other.playerName)
                && playerId.Equals(other.playerId);
        }
    }
}