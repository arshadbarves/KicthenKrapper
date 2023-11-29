using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace KitchenKrapper
{
    public enum ScreenType { None, Modal, Overlay, PopUp }
    public abstract class Screen : MonoBehaviour
    {
        [Tooltip("String ID from the UXML for this menu panel/screen.")]
        [SerializeField] protected string screenName;
        [Tooltip("Type of screen (Modal, Overlay, PopUp).")]
        [SerializeField] protected ScreenType screenType;

        [Header("UI Management")]
        [Tooltip("Set the Main Menu here explicitly (or get automatically from the current GameObject).")]
        [SerializeField] protected MainMenuUIManager mainMenuUIManager;
        [Tooltip("Set the UI Document here explicitly (or get automatically from the current GameObject).")]
        [SerializeField] protected UIDocument document;

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
                document = mainMenuUIManager.MainMenuDocument;

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

            visualElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public VisualElement GetVisualElement(string elementName)
        {
            if (string.IsNullOrEmpty(elementName) || root == null)
                return null;

            return root.Q(elementName);
        }

        public virtual void ShowScreen()
        {
            ShowVisualElement(screen, true);
            ScreenStarted?.Invoke();
        }

        public virtual void HideScreen()
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
    }
}
