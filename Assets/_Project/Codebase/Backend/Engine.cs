using System.Threading.Tasks;
using PixelSim.SceneLoading;
using UnityEngine;

namespace PixelSim.Backend
{
    public sealed class Engine : MonoBehaviour
    {
        private async void Awake()
        {
            Application.targetFrameRate = 60;

            Task[] tasks = new Task[2];
            
            tasks[0] = SceneLoader.LoadFrontendScene(FrontendScenes.MAIN_MENU_BACKGROUND);
            tasks[1] = SceneLoader.LoadFrontendScene(FrontendScenes.MAIN_MENU_UI);

            await Task.WhenAll(tasks);
        }
    }
}