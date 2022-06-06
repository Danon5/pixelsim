using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelSim.DataContainers
{
    [Serializable]
    public sealed class SerializedDictionary<TKey, TValue>
    {
        [SerializeField] private List<DatabaseEntry> _dataEntries = new List<DatabaseEntry>();

        private Dictionary<TKey, TValue> _data;

        public TValue this[TKey key]
        {
            get
            {
                InitializeIfRequired();
                return _data[key];
            }
            set
            {
                InitializeIfRequired();
                _data[key] = value;
            }
        }

        public bool ContainsKey(TKey key)
        {
            InitializeIfRequired();
            return _data.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            InitializeIfRequired();
            return _data.ContainsValue(value);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            InitializeIfRequired();
            return _data.TryGetValue(key, out value);
        }

        public bool TryAdd(TKey key, TValue value)
        {
            InitializeIfRequired();

            if (_data.ContainsKey(key)) return false;
            
            _data.Add(key, value);
            return true;
        }

        public void Add(TKey key, TValue value)
        {
            InitializeIfRequired();
            _data.Add(key, value);
        }

        public void Remove(TKey key)
        {
            InitializeIfRequired();
            _data.Remove(key);
        }

        private void InitializeIfRequired()
        {
            if (_data != null) return;
            
            _data = new Dictionary<TKey, TValue>();
            
            foreach (DatabaseEntry entry in _dataEntries)
                _data.Add(entry.id, entry.data);
        }

        [Serializable]
        private struct DatabaseEntry
        {
            public TKey id;
            public TValue data;
        }
    }
}