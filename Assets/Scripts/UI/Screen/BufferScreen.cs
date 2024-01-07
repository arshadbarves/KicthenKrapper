using UI.Base;
using UnityEngine.UIElements;
using UnityEngine;

namespace KitchenKrapper
{
    public class BufferScreen : BaseScreen
    {
        private const string BUFFER_ICON_NAME = "buffer__icon";

        private VisualElement bufferIcon;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            bufferIcon = Root.Q<VisualElement>(BUFFER_ICON_NAME);
            Screen.pickingMode = PickingMode.Ignore;
        }

        private void Update()
        {
            if (IsVisible())
            {
                bufferIcon.style.opacity = Mathf.PingPong(Time.time, 0.5f);
            }
        }
    }
}