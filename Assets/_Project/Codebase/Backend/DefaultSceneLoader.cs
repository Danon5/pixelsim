using System.Threading.Tasks;
using PixelSim.SceneLoading;
using UnityEngine;

namespace PixelSim.Backend
{
    public sealed class DefaultSceneLoader : MonoBehaviour
    {
        private async void Awake()
        {
            Task[] sceneLoadTasks = new Task[2];
            
            sceneLoadTasks[0] = SceneLoader.LoadFrontendScene(FrontendScenes.MAIN_MENU_BACKGROUND);
            sceneLoadTasks[1] = SceneLoader.LoadFrontendScene(FrontendScenes.MAIN_MENU_UI);

            await Task.WhenAll(sceneLoadTasks);
        }
    }
}