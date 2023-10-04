using System;
using UnityEngine;

namespace KitchenKrapper
{
    public class UniqueIDGenerator : MonoBehaviour
    {
        public static string GetUniqueID()
        {
            Guid guid = Guid.NewGuid();
            return guid.ToString();
        }
    }
}