using UnityEngine;
using VoxelSim.SceneLoading;

namespace VoxelSim
{
    public sealed class Engine : MonoBehaviour
    {
        private async void Awake()
        {
            Application.targetFrameRate = 60;

            await SceneLoader.LoadFrontendScene(FrontendScenes.GAME);
        }
    }
}