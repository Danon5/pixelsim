using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelSim.Shared.SceneLoading
{
    public static class SceneLoader
    {
        private static readonly List<string> _loadedFrontendScenes = new List<string>();
        private static readonly List<string> _loadedBackendScenes = new List<string>();
        private static string _loadedMapScene = string.Empty;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            _loadedFrontendScenes?.Clear();
            _loadedBackendScenes?.Clear();
            _loadedMapScene = string.Empty;
        }
        
        public static async UniTask LoadFrontendScene(string sceneName, bool setAsActiveScene = false)
        {
            await LoadSceneAsync(sceneName, _loadedFrontendScenes, setAsActiveScene);
        }

        public static async UniTask UnloadAllFrontendScenes()
        {
            await UnloadScenesAsync(_loadedFrontendScenes);
        }

        public static async UniTask LoadBackendScene(string sceneName)
        {
            await LoadSceneAsync(sceneName, _loadedBackendScenes);
        }
        
        public static async UniTask UnloadAllBackendScenes()
        {
            await UnloadScenesAsync(_loadedBackendScenes);
        }

        public static async UniTask LoadMapScene(string sceneName)
        {
            await LoadSceneAsync(sceneName);
        }
        
        public static async UniTask UnloadMapScene()
        {
            if (_loadedMapScene.Equals(string.Empty)) return;
            
            _loadedMapScene = string.Empty;

            await UnloadSceneAsync(_loadedMapScene);
        }

        private static async UniTask LoadSceneAsync(string sceneName, List<string> sceneList = null, bool setAsActiveScene = false)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            sceneList?.Add(sceneName);

            while (!operation.isDone)
                await UniTask.Yield();

            if (setAsActiveScene)
                SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
        }
        
        private static async UniTask UnloadSceneAsync(string sceneName, List<string> sceneList = null)
        {
            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
            sceneList?.Remove(sceneName);

            while (!operation.isDone)
                await UniTask.Yield();
        }

        private static async UniTask UnloadScenesAsync(List<string> scenes)
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

                await UniTask.Yield();
            }
        }
    }
}