using System;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Base
{
    public enum ScreenType { None, FullScreen, Popup, Overlay }
    public abstract class BaseScreen : MonoBehaviour
    {
        [SerializeField] protected string screenName;
        [SerializeField] protected ScreenType screenType;

        [Header("UI Management")] 
        [SerializeField] protected UIDocument document;
        [SerializeField] protected BaseScreen parentScreen = null;
        
        private static MainMenuUIManager _mainMenuUIManager;
        protected VisualElement Screen;
        protected VisualElement Root;

        public event Action ScreenStarted;
        public event Action ScreenEnded;

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(screenName))
                screenName = GetType().Name;
        }

        protected virtual void Awake()
        {
            if (document == null)
                document = GetComponent<UIDocument>();

            if (document == null && _mainMenuUIManager != null)
                document = _mainMenuUIManager.GetMainDocument();

            if (document == null)
            {
                Debug.LogWarning("MenuScreen " + screenName + ": missing UIDocument. Check Script Execution Order.");
                return;
            }

            SetVisualElements();
            RegisterButtonCallbacks();
        }

        private void Start()
        {
            if (_mainMenuUIManager == null)
                _mainMenuUIManager = MainMenuUIManager.Instance;
        }

        protected virtual void OnDestroy()
        {
            UnregisterButtonCallbacks();
        }
        protected virtual void SetVisualElements()
        {
            if (document != null)
                Root = document.rootVisualElement;

            Screen = GetVisualElement(screenName);
        }

        protected virtual void RegisterButtonCallbacks()
        {
            // Add button event registrations here.
        }

        protected virtual void UnregisterButtonCallbacks()
        {
            // Remove button event registrations here.
        }

        protected bool IsVisible()
        {
            if (Screen == null)
                return false;
            return Screen.style.display == DisplayStyle.Flex;
        }

        private static void ShowVisualElement(VisualElement visualElement, bool state)
        {
            if (visualElement == null)
                return;

            visualElement.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private VisualElement GetVisualElement(string elementName)
        {
            if (string.IsNullOrEmpty(elementName) || Root == null)
                return null;

            return Root.Q(elementName);
        }

        public virtual void Show()
        {
            if (IsVisible())
                return;

            ShowVisualElement(Screen, true);
            ScreenStarted?.Invoke();
        }

        public virtual void Hide()
        {
            if (!IsVisible()) return;
            ShowVisualElement(Screen, false);
            ScreenEnded?.Invoke();
        }

        public ScreenType GetScreenType()
        {
            return screenType;
        }

        public BaseScreen GetParentScreen()
        {
            return parentScreen;
        }
    }
}
