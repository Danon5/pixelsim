using UnityEngine;

namespace PixelSim.Shared.Backend
{
    public sealed class Engine : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }
    }
}