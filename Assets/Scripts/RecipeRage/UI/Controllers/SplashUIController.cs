using System;
using RecipeRage.UI.Base;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecipeRage.UI.Controllers
{
    public class SplashUIController : UIScreen
    {
        public static event Action OnSplashPanelActiveTransitionEnd;
        
        private VisualElement _splashPanel;
        private const string SplashPanelActiveClass = "splashPanel--active";

        protected override void Awake()
        {
            base.Awake();
            _splashPanel = Root.Q<VisualElement>("splashPanel");
        }

        public override void Show()
        {
            base.Show();

            AnimateToggleClass(_splashPanel, SplashPanelActiveClass, () =>
            {
                AnimateToggleClass(_splashPanel, SplashPanelActiveClass, () =>
                {
                    OnSplashPanelActiveTransitionEnd?.Invoke(); 
                });
            });
        }
    }
}