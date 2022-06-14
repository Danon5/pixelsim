using UnityEngine;
using UnityEngine.EventSystems;

namespace PixelSim.Client.CUI
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class CUI_Element : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, 
        IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
    {
        /// <summary>
        /// The primary rect of the CUI element.
        /// </summary>
        public RectTransform Rect { get; private set; }
        
        /// <summary>
        /// Returns whether or not the mouse is hovering over the primary rect.
        /// </summary>
        public bool IsMouseOver { get; private set; }

        private void Awake()
        {
            Initialize();
        }

        protected virtual void OnValidate() { }

        protected virtual void Initialize()
        {
            Rect = GetComponent<RectTransform>();
        }

        protected virtual void OnEnable() { }
        public virtual void OnPointerEnter(PointerEventData eventData) => IsMouseOver = true;
        public virtual void OnPointerExit(PointerEventData eventData) => IsMouseOver = false;
        public virtual void OnPointerDown(PointerEventData eventData) { }
        public virtual void OnPointerUp(PointerEventData eventData) { }
        public virtual void OnPointerClick(PointerEventData eventData) { }
    }
}