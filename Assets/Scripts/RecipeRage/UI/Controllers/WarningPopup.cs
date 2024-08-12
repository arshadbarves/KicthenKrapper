using System;
using RecipeRage.UI.Base;
using UnityEngine.UIElements;

namespace RecipeRage.UI.Controllers
{
    public class WarningPopupController : PopupScreen
    {
        private Label _warningTitleLabel;
        private Label _warningContentLabel;
        private Button _continueButton;
        private Button _closeButton;

        public static event Action<bool> OnContinue;

        protected override void Awake()
        {
            base.Awake();

            _warningTitleLabel = Root.Q<Label>("warningTitleLabel");
            _warningContentLabel = Root.Q<Label>("warningContentLabel");
            _continueButton = Root.Q<Button>("continueButton");
            _closeButton = Root.Q<Button>("closeButton");

            _continueButton.clicked += OnContinueButtonClicked;
            _closeButton.clicked += OnCloseButtonClicked;
        }

        private void OnContinueButtonClicked()
        {
            OnContinue?.Invoke(true);
        }

        private void OnCloseButtonClicked()
        {
            OnContinue?.Invoke(false);
        }

        public void SetWarningText(string warningText, string title, bool showCancelButton, string cancelButtonText, string continueButtonText)
        {
            _warningTitleLabel.text = title;
            _warningContentLabel.text = warningText;
            _closeButton.text = cancelButtonText;
            _continueButton.text = continueButtonText;

            _closeButton.style.display = showCancelButton ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}