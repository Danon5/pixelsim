using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PixelSim.CUI
{
    public class CUI_Button : CUI_Element
    {
        /// <summary>
        /// The color changing graphics that indicates whether the mouse is over it.
        /// </summary>
        [field: SerializeField] public CUI_Graphic[] Graphics { get; private set; }

        /// <summary>
        /// Invoked when the button is clicked.
        /// </summary>
        [field: SerializeField] public UnityEvent OnClick { get; private set; }

        public bool Disabled
        {
            get => _disabled;
            set
            {
                if (_disabled != value)
                    TweenGraphicsToColor(GetHighestPriorityColorType());
                _disabled = value;
            }
        }
        private bool _disabled;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            TweenGraphicsToColor(GetHighestPriorityColorType());
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (eventData.button != PointerEventData.InputButton.Left) return;

            OnClick.Invoke();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            
            if (Graphics == null) return;

            TweenGraphicsToColor(GetHighestPriorityColorType());
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            
            if (Graphics == null) return;

            TweenGraphicsToColor(GetHighestPriorityColorType());
        }

        private void TweenGraphicsToColor(CUI_GraphicColorType type)
        {
            foreach (CUI_Graphic graphic in Graphics)
            {
                if (graphic == null) continue;
                
                DOTween.To(
                    () => graphic.Color,
                    value => graphic.Color = value,
                    GetColorFromType(graphic, type),
                    .1f);
            }
        }

        private Color GetColorFromType(CUI_Graphic graphic, CUI_GraphicColorType colorType)
        {
            return graphic.GetColorFromType(colorType);
        }

        private CUI_GraphicColorType GetHighestPriorityColorType()
        {
            if (_disabled)
                return CUI_GraphicColorType.Disabled;
            if (IsMouseOver)
                return CUI_GraphicColorType.Hovered;
            return CUI_GraphicColorType.Default;
        }
    }
}