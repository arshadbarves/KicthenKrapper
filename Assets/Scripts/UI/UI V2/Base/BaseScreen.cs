using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace KitchenKrapper
{
    public enum ScreenType { None, FullScreen, Popup, Overlay }
    public abstract class BaseScreen : MonoBehaviour
    {
        [SerializeField] protected string screenName;
        [SerializeField] protected ScreenType screenType;

        [Header("UI Management")]
        [SerializeField] protected MainMenuUIManager mainMenuUIManager;
        [SerializeField] protected UIDocument document;
        [SerializeField] protected BaseScreen parentScreen = null;

        protected VisualElement screen;
        protected VisualElement root;

        public event Action ScreenStarted;
        public event Action ScreenEnded;

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(screenName))
                screenName = this.GetType().Name;
        }

        protected virtual void Awake()
        {
            if (mainMenuUIManager == null)
                mainMenuUIManager = GetComponent<MainMenuUIManager>();

            if (document == null)
                document = GetComponent<UIDocument>();

            if (document == null && mainMenuUIManager != null)
                document = mainMenuUIManager.GetMainDocument();

            if (document == null)
            {
                Debug.LogWarning("MenuScreen " + screenName + ": missing UIDocument. Check Script Execution Order.");
                return;
            }
            else
            {
                SetVisualElements();
                RegisterButtonCallbacks();
            }
        }

        protected virtual void OnDestroy()
        {
            UnregisterButtonCallbacks();
        }
        protected virtual void SetVisualElements()
        {
            if (document != null)
                root = document.rootVisualElement;

            screen = GetVisualElement(screenName);
        }

        protected virtual void RegisterButtonCallbacks()
        {
            // Add button event registrations here.
        }

        protected virtual void UnregisterButtonCallbacks()
        {
            // Remove button event registrations here.
        }

        public bool IsVisible()
        {
            if (screen == null)
                return false;
            return screen.style.display == DisplayStyle.Flex;
        }

        public static void ShowVisualElement(VisualElement visualElement, bool state)
        {
            if (visualElement == null)
                return;

            visualElement.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public VisualElement GetVisualElement(string elementName)
        {
            if (string.IsNullOrEmpty(elementName) || root == null)
                return null;

            return root.Q(elementName);
        }

        public virtual void Show()
        {
            if (IsVisible())
                return;

            ShowVisualElement(screen, true);
            ScreenStarted?.Invoke();
        }

        public virtual void Hide()
        {
            if (IsVisible())
            {
                ShowVisualElement(screen, false);
                ScreenEnded?.Invoke();
            }
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
