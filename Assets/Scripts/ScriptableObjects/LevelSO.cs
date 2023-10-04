using UnityEngine;

namespace KitchenKrapper
{
    // holds basic level information (label name, level number, scene name for loading, thumbnail graphic for display, etc.)
    [CreateAssetMenu(fileName = "LevelData", menuName = "KitchenKrapper/Level", order = 11)]
    public class LevelSO : ScriptableObject
    {
        public int levelNumber;
        public string levelLabel;
        public Sprite thumbnail;
        public string sceneName;
    }
}
