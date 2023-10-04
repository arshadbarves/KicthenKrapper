using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KitchenKrapper
{
    public enum CharacterClass
    {
        Paladin,
        Wizard,
        Barbarian,
        Necromancer
    }
    public enum Rarity
    {
        Common,
        Rare,
        Special,
        All, // for filtering
    }

    // baseline data for a specific character

    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Characters/CharacterGameData", menuName = "KitchenKrapper/Character", order = 1)]
    public class CharacterBaseSO : ScriptableObject
    {
        public string characterName;
        public GameObject characterVisualsPrefab;
        public CharacterClass characterClass;
        public Rarity rarity;

        public string bioTitle;
        [TextArea] public string bio;

        public float basePointsLife;
        public float basePointsDefense;
        public float basePointsAttack;
        public float basePointsAttackSpeed;
        public float basePointsSpecialAttack;
        public float basePointsCriticalHit;
    }
}