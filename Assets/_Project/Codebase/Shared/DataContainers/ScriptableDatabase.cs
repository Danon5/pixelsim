using UnityEngine;

namespace PixelSim.Shared.DataContainers
{
    public abstract class ScriptableDatabase<TKey, TValue> : ScriptableObject
    {
        [SerializeField] protected SerializedDictionary<TKey, TValue> _data = new SerializedDictionary<TKey, TValue>();

        public TValue GetData(TKey key) => _data[key];
    }
}