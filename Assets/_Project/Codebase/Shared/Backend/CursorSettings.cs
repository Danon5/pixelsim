using UnityEngine;

namespace PixelSim.Shared.Backend
{
    public sealed class CursorSettings : MonoBehaviour
    {
        [SerializeField] private Texture2D _cursorDefaultTex;

        private static CursorSettings _singleton;

        private void Awake()
        {
            _singleton = this;
            
            SetCursorType(CursorType.Default);
        }

        public static void SetCursorType(CursorType cursorType)
        {
            Texture2D tex;

            switch (cursorType)
            {
                case CursorType.Default:
                    tex = _singleton._cursorDefaultTex;
                    break;
                default:
                    return;
            }
            
            Vector2 texSize = new Vector2(tex.width, tex.height);
            Cursor.SetCursor(_singleton._cursorDefaultTex, texSize / 2f, CursorMode.ForceSoftware);
        }
    }
}