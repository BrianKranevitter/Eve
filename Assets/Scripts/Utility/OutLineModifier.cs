using UnityEngine;

namespace Game.Utility
{
    [RequireComponent(typeof(Outline))]
    public sealed class OutLineModifier : MonoBehaviour
    {
        [Header("Selected")]
        [SerializeField, Tooltip("The colors that outline will swap between while selected")]
        private Color colorAselected;

        [SerializeField, Tooltip("The colors that outline will swap between while selected")]
        private Color colorBselected;

        [SerializeField, Tooltip("The frecuency between each color variation")]
        private float changeFrecuencySelected;

        [SerializeField, Range(0, 1), Tooltip("The saturation range that each color can reach")]
        private float changeSizeSelected;

        [SerializeField, Tooltip("The range of the outline while selected")]
        private float outlineRangeSelected;

        [Header("Unselected")]
        [SerializeField, Tooltip("The colors that outline will swap between while unselected")]
        private Color colorAunselected;

        [SerializeField, Tooltip("The colors that outline will swap between while unselected")]
        private Color colorBunselected;

        [SerializeField, Tooltip("The frecuency between each color variation while selected")]
        private float changeFrecuencyUnselected;

        [SerializeField, Range(0, 1), Tooltip("The saturation range that each color can reach while selected")]
        private float changeSizeUnselected;

        [SerializeField, Tooltip("The range of the outline while unselected")]
        private float outlineRangeUnselected;

        private Outline outline;

        private bool isInSight;
        private bool isHightlight;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Awake() => outline = GetComponent<Outline>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by Unity.")]
        private void Update()
        {
            outline.enabled = isInSight || isHightlight;
            if (isHightlight)
            {
                outline.OutlineWidth = outlineRangeSelected;
                outline.OutlineColor = Color.Lerp(colorAselected, colorBselected, (Mathf.Sin(Time.time * changeFrecuencySelected) + 1) / 2) * changeSizeSelected;
            }
            else
            {
                outline.OutlineWidth = outlineRangeUnselected;
                outline.OutlineColor = Color.Lerp(colorAunselected, colorBunselected, (Mathf.Sin(Time.time * changeFrecuencyUnselected) + 1) / 2) * changeSizeUnselected;
            }
        }

        public void OutOfSight() => isInSight = false;

        public void InSight() => isInSight = true;

        public void Unhighlight() => isHightlight = false;

        public void Highlight() => isHightlight = true;
    }
}