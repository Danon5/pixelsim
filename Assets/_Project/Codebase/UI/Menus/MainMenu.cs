using PixelSim.SceneLoading;
using UnityEngine;

namespace PixelSim.UI.Menus
{
    public sealed class MainMenu : MonoBehaviour
    {
        public async void LoadGameScene()
        {
            await SceneLoader.UnloadAllFrontendScenes();
            await SceneLoader.LoadFrontendScene(FrontendScenes.GAME);
        }
    }
}