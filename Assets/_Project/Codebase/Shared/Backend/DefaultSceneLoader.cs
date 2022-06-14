using Cysharp.Threading.Tasks;
using PixelSim.Shared.SceneLoading;
using UnityEngine;

namespace PixelSim.Shared.Backend
{
    public sealed class DefaultSceneLoader : MonoBehaviour
    {
        private async void Awake()
        {
            UniTask[] sceneLoadTasks = new UniTask[2];
            
            sceneLoadTasks[0] = SceneLoader.LoadFrontendScene(FrontendScenes.MAIN_MENU_BACKGROUND);
            sceneLoadTasks[1] = SceneLoader.LoadFrontendScene(FrontendScenes.MAIN_MENU_UI);

            await UniTask.WhenAll(sceneLoadTasks);
        }
    }
}