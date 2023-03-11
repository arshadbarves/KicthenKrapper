using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
    public ulong clientId;
    public bool isReady;
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref isReady);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }

    public bool Equals(PlayerData other)
    {
        return clientId == other.clientId
            && isReady == other.isReady
            && playerName.Equals(other.playerName)
            && playerId.Equals(other.playerId);
    }
}
