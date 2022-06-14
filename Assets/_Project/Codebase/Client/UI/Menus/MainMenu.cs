using PixelSim.Shared.SceneLoading;
using UnityEngine;

namespace PixelSim.Client.UI.Menus
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