using System;
using UnityEngine;

public class UniqueIDGenerator : MonoBehaviour
{
    public static string GetUniqueID()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString();
    }
}
