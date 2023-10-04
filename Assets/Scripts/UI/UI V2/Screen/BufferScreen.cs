using UnityEngine.UIElements;
using UnityEngine;

namespace KitchenKrapper
{
    public class BufferScreen : Screen
    {
        private const string BufferIconName = "buffer__icon";

        private VisualElement bufferIcon;

        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            bufferIcon = root.Q<VisualElement>(BufferIconName);
            screen.pickingMode = PickingMode.Ignore;
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