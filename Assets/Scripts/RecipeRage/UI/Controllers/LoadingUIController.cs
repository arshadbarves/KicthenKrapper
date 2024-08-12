using RecipeRage.UI.Base;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecipeRage.UI.Controllers
{
    public class LoadingUIController : UIScreen
    {
        private Label _tipLabel;
        private Label _loadingLabel;
        private Label _recipeNameLabel;
        private Label _rageNameLabel;
        
        private const string RecipeLogoActiveClass = "recipeLogoLabel--active";
        private const string RageLogoActiveClass = "rageLogoLabel--active";
        

        protected override void Awake()
        {
            base.Awake();
            _tipLabel = Root.Q<Label>("tipLabel");
            _loadingLabel = Root.Q<Label>("loadingLabel");
            _recipeNameLabel = Root.Q<Label>("recipeLogoLabel");
            _rageNameLabel = Root.Q<Label>("rageLogoLabel");
        }
        
        public void SetLoadingText(string text)
        {
            _loadingLabel.text = text;
        }
        
        private static string GetRandomTip()
        {
            string[] tips = GameConstants.Tips;
            return "Tip: " + tips[Random.Range(0, tips.Length)];
        }

        public override void Show()
        {
            base.Show();
            
            _tipLabel.text = GetRandomTip();
            AnimateToggleClass(_recipeNameLabel, RecipeLogoActiveClass);
            AnimateToggleClass(_rageNameLabel, RageLogoActiveClass);
        }
        
        public override void Hide()
        {
            base.Hide();
            
            AnimateToggleClass(_recipeNameLabel, RecipeLogoActiveClass);
            AnimateToggleClass(_rageNameLabel, RageLogoActiveClass);
        }
    }
}