using PixelSim.ECS;
using PixelSim.Utility;
using UnityEngine;

namespace PixelSim.DeveloperTools
{
    public sealed class DevController : MonoBehaviour
    {
        [SerializeField] private GameObject _physicsCirclePrefab;
        [SerializeField] private GameObject _physicsBoxPrefab;
        
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            Vector2 worldMousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetKey(KeyCode.Mouse0))
                World.Current.SetPixelCircleAtPos(worldMousePos, 25, PixelId.Air);
            else if (Input.GetKey(KeyCode.Mouse1))
                World.Current.SetPixelCircleAtPos(worldMousePos, 25, PixelId.Dirt);

            if (Input.GetKeyDown(KeyCode.Alpha1))
                ECSManager.InstantiateEntityFromPrefab(_physicsCirclePrefab, worldMousePos, Quaternion.identity);
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                ECSManager.InstantiateEntityFromPrefab(_physicsBoxPrefab, worldMousePos, Quaternion.identity);

            Vector2 moveAxis = new Vector2(
                Input.GetAxisRaw("Horizontal"), 
                Input.GetAxisRaw("Vertical")).normalized;

            transform.Translate(moveAxis * ((Input.GetKey(KeyCode.LeftShift) ? 25f : 10f) * Time.deltaTime));
        }

        private void OnGUI()
        {
            if (!Application.isPlaying) return;

            Vector2 mousePos = Input.mousePosition;

            if (!World.Current.TryGetPixelAtWorldPos(_camera.ScreenToWorldPoint(mousePos), out Pixel voxel)) return;
            
            Vector2 guiMousePos = SpaceConversions.ScreenToSreenGUICorrected(mousePos);
            GUI.Label(new Rect(guiMousePos.x + 15f, guiMousePos.y - 25f, 500f, 500f), $"{voxel.id}");
        }
    }
}