using UnityEngine;

namespace PixelSim.Backend
{
    public sealed class Engine : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;
        }
    }
}