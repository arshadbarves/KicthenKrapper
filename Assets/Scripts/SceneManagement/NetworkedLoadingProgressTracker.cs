using Unity.Netcode;

/// <summary>
/// Simple object that keeps track of the scene loading progress of a specific instance.
/// </summary>
/// <remarks>
/// This is a simple example of how to use NetworkVariables to keep track of the loading progress of a specific instance.
/// </remarks>

namespace KitchenKrapper
{
    public class NetworkedLoadingProgressTracker : NetworkBehaviour
    {
        /// <summary>
        /// The current loading progress associated with the owner of this NetworkBehavior
        /// </summary>
        public NetworkVariable<float> Progress { get; } = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }
}