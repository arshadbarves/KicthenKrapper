using UnityEngine;
using UnityEngine.UIElements;

namespace RecipeRage.UI.Base
{
    public abstract class UIScreen : MonoBehaviour
    {
        public VisualElement Root { get; protected set; }
        public VisualElement ScreenPanel { get; protected set; }
        
        protected virtual void Awake()
        {
            Root = GetComponent<UIDocument>().rootVisualElement;
            ScreenPanel = Root.Q<VisualElement>("screenPanel");
        }
        
        public virtual void Show()
        {
            ScreenPanel.style.display = DisplayStyle.Flex;
        }
        
        public virtual void Hide()
        {
            ScreenPanel.style.display = DisplayStyle.None;
        }
    }
}