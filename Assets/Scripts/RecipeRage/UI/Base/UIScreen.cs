using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace RecipeRage.UI.Base
{
    public abstract class UIScreen : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset;
        
        private Action _onTransitionEnd;
        
        public VisualElement Root { get; private set; }
        
        protected virtual void Awake()
        {
            if (visualTreeAsset != null)
            {
                Root = visualTreeAsset.Instantiate();
                SetRootStyles();
            }
            else
            {
                Debug.LogError("VisualTreeAsset is not assigned.");
            }
        }
        
        private void SetRootStyles()
        {
            Root.style.position = Position.Absolute;
            Root.style.left = 0;
            Root.style.right = 0;
            Root.style.top = 0;
            Root.style.bottom = 0;
        }
        
        public virtual void Show()
        {
            if (!IsShown())
            {
                SetDisplayStyle(DisplayStyle.Flex);
            }
        }

        public virtual void Hide()
        {
            if (IsShown())
            {
                SetDisplayStyle(DisplayStyle.None);
            }
        }
        
        private void SetDisplayStyle(DisplayStyle display)
        {
            if (Root != null)
            {
                Root.style.display = display;
            }
            else
            {
                Debug.LogError("Root VisualElement is null.");
            }
        }
        
        public bool IsShown()
        {
            return Root != null && Root.style.display == DisplayStyle.Flex;
        }
        
        protected void AnimateToggleClass(VisualElement element, string className, Action onTransitionEnd = null)
        {
            // Clear previous transition end event, if any
            _onTransitionEnd = null;
            
            _onTransitionEnd = onTransitionEnd;
            StartCoroutine(AnimateToggleClassCoroutine(element, className));
        }
        
        private IEnumerator AnimateToggleClassCoroutine(VisualElement element, string className)
        {
            yield return null; // Wait for the next frame to ensure we have previous state to animate from
            element?.ToggleInClassList(className);
            if (element != null)
            {
                element.RegisterCallback<TransitionEndEvent>(OnTransitionEndHandler);
            }
            else
            {
                Debug.LogError("VisualElement is null.");
            }
        }
        
        private void OnTransitionEndHandler(TransitionEndEvent evt)
        {
            if (evt.target is VisualElement element)
            {
                element.UnregisterCallback<TransitionEndEvent>(OnTransitionEndHandler);
                _onTransitionEnd?.Invoke();
            }
            else
            {
                Debug.LogError("TransitionEndEvent target is not a VisualElement.");
            }
        }
    }
}
