using PixelSim.ECS;
using UnityEngine;

namespace PixelSim
{
    public sealed class RenderTester : MonoBehaviour
    {
        [SerializeField] private GameObject _chunkPrefab;
        
        private void Start()
        {
            ECSManager.InstantiateEntityFromPrefab(_chunkPrefab);
        }
    }
}