using System;
using System.Collections.Generic;
using RecipeRage.UI.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecipeRage.Managers
{
    [RequireComponent(typeof(UIDocument))]
    public class UIManager : MonoSingleton<UIManager>
    {
        private UIDocument _uiDocument;
        private LoginUIController _loginUIController;
        private WarningPopupController _warningPopupController;
        private LoadingUIController _loadingUIController;
        private SplashUIController _splashUIController;
        
        private readonly Stack<VisualElement> _uiStack = new Stack<VisualElement>();

        protected override void Awake()
        {
            base.Awake();

            _uiDocument = GetComponent<UIDocument>();
            
            _loginUIController = GetComponent<LoginUIController>();
            _warningPopupController = GetComponent<WarningPopupController>();
            _loadingUIController = GetComponent<LoadingUIController>();
            _splashUIController = GetComponent<SplashUIController>();
            
            _uiDocument.rootVisualElement.Clear();
        }
        
        private void PushUI(VisualElement ui)
        {
            if (_uiStack.Count > 0)
            {
                _uiStack.Peek().style.display = DisplayStyle.None;
            }
            _uiStack.Push(ui);
            _uiDocument.rootVisualElement.Add(ui);

        }
        
        private void PopUI(VisualElement ui = null)
        {
            if (_uiStack.Count == 0) return;
            if (ui != null)
            {
                _uiStack.Pop();
                _uiDocument.rootVisualElement.Remove(ui);
            }
            else
            {
                var poppedUI = _uiStack.Pop();
                _uiDocument.rootVisualElement.Remove(poppedUI);
            }
            if (_uiStack.Count > 0)
            {
                _uiStack.Peek().style.display = DisplayStyle.Flex;
            }
        }
        
        public void ShowWarningPopup(string warningText, Action<bool> onContinue = null, string title = "Warning!", bool showCancelButton = true, string cancelButtonText = "Cancel", string continueButtonText = "Continue")
        {
            PushUI(_warningPopupController.Root);
            _warningPopupController.SetWarningText(warningText, title, showCancelButton, cancelButtonText, continueButtonText);
            WarningPopupController.OnContinue += onContinue;
            _warningPopupController.Show();
        }

        public void HideWarningPopup(Action<bool> onContinue = null)
        {
            PopUI(_warningPopupController.Root);
            _warningPopupController.Hide();
            WarningPopupController.OnContinue -= onContinue;
        }
        
        public bool IsWarningPopupShown()
        {
            return _warningPopupController.IsShown();
        }

        public void ShowLoginScreen()
        {
            PushUI(_loginUIController.Root);
            _loginUIController.Show();
        }

        public void HideLoginScreen()
        {
            PopUI(_loginUIController.Root);
            _loginUIController.Hide();
        }

        public void ShowLoadingScreen(string loadingText = "Loading...")
        {
            PushUI(_loadingUIController.Root);
            _loadingUIController.SetLoadingText(loadingText);
            _loadingUIController.Show();
        }
        
        public void HideLoadingScreen()
        {
            PopUI(_loadingUIController.Root);
            _loadingUIController.Hide();
        }

        public void ShowMainMenuScreen()
        {
            Debug.Log("Show main menu screen");
        }
        
        public void HideMainMenuScreen()
        {
            Debug.Log("Hide main menu screen");
        }

        public void ShowGameplayUI()
        {
            Debug.Log("Show gameplay UI");
        }
        
        public void HideGameplayUI()
        {
            Debug.Log("Hide gameplay UI");
        }

        public void ShowGameOverScreen()
        {
            Debug.Log("Show game over screen");
        }
        
        public void HideGameOverScreen()
        {
            Debug.Log("Hide game over screen");
        }

        public void ShowSplashScreen()
        {
            PushUI(_splashUIController.Root);
            _splashUIController.Show();
        }
        
        public void HideSplashScreen()
        {
            PopUI(_splashUIController.Root);
            _splashUIController.Hide();
        }
    }
}