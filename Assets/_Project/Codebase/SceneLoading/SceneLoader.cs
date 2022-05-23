using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelSim.SceneLoading
{
    public static class SceneLoader
    {
        private static readonly List<string> _loadedFrontendScenes = new List<string>();
        private static readonly List<string> _loadedBackendScenes = new List<string>();
        private static string _loadedMapScene = string.Empty;
        
        public static async Task LoadFrontendScene(string sceneName, bool setAsActiveScene = false)
        {
            await LoadSceneAsync(sceneName, _loadedFrontendScenes, setAsActiveScene);
        }

        public static async Task UnloadAllFrontendScenes()
        {
            await UnloadScenesAsync(_loadedFrontendScenes);
        }

        public static async Task LoadBackendScene(string sceneName)
        {
            await LoadSceneAsync(sceneName, _loadedBackendScenes);
        }
        
        public static async Task UnloadAllBackendScenes()
        {
            await UnloadScenesAsync(_loadedBackendScenes);
        }

        public static async Task LoadMapScene(string sceneName)
        {
            await LoadSceneAsync(sceneName);
        }
        
        public static async Task UnloadMapScene()
        {
            if (_loadedMapScene.Equals(string.Empty)) return;
            
            _loadedMapScene = string.Empty;

            await UnloadSceneAsync(_loadedMapScene);
        }

        private static async Task LoadSceneAsync(string sceneName, List<string> sceneList = null, bool setAsActiveScene = false)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            sceneList?.Add(sceneName);

            while (!operation.isDone)
                await Task.Yield();

            if (setAsActiveScene)
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }
        
        private static async Task UnloadSceneAsync(string sceneName, List<string> sceneList = null)
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
            sceneList?.Remove(sceneName);

            while (!operation.isDone)
                await Task.Yield();
        }

        private static async Task UnloadScenesAsync(List<string> scenes)
        {
            if (scenes == null || scenes.Count == 0) return;
            
            AsyncOperation[] operations = new AsyncOperation[scenes.Count];

            for (int i = 0; i < scenes.Count; i++)
                operations[i] = SceneManager.UnloadSceneAsync(scenes[i]);

            bool allCompleted = false;

            while (!allCompleted)
            {
                foreach (AsyncOperation operation in operations)
                {
                    allCompleted = true;

                    if (!operation.isDone)
                    {
                        allCompleted = false;
                        break;
                    }
                }

                await Task.Yield();
            }
        }
    }
}