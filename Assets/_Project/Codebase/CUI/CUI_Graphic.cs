using UnityEngine;
using UnityEngine.UI;

namespace PixelSim.CUI
{
    [RequireComponent(typeof(Graphic))]
    public class CUI_Graphic : CUI_Element
    {
        [field: SerializeField] public Color DefaultColor { get; private set; } = Color.white;
        [field: SerializeField] public Color HoveredColor { get; private set; } = Color.white;
        [field: SerializeField] public Color DisabledColor { get; private set; } = Color.white;

        public Graphic Graphic { get; private set; }

        public Color Color
        {
            get => Graphic.color;
            set => Graphic.color = value;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Graphic = GetComponent<Graphic>();
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (Graphic == null)
                Graphic = GetComponent<Graphic>();
            Graphic.color = DefaultColor;
        }

        public Color GetColorFromType(CUI_GraphicColorType colorType)
        {
            switch (colorType)
            {
                case CUI_GraphicColorType.Default:
                    return DefaultColor;
                case CUI_GraphicColorType.Hovered:
                    return HoveredColor;
                case CUI_GraphicColorType.Disabled:
                    return DisabledColor;
                default:
                    return Color.white;
            }
        }
    }
}