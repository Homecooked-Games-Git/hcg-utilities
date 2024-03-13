using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitySerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] 
    private List<KeyValue> pairs = new();
    
    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Clear();
        foreach (var pair in pairs)
        {
            try
            {
                TKey key;
                if (typeof(TKey).IsEnum)
                {
                    var enumType = Nullable.GetUnderlyingType(typeof(TKey)) ?? typeof(TKey);

                    if (!string.IsNullOrEmpty(pair.key) && Enum.TryParse(enumType, pair.key, out var enumValueObj) && enumValueObj != null)
                    {
                        key = (TKey)enumValueObj;
                    }
                    else
                    {
                        Debug.LogError($"Failed to parse enum value from string: {pair.key}");
                        continue;
                    }
                }
                else
                {
                    key = (TKey)Convert.ChangeType(pair.key, typeof(TKey));
                }

                if (key != null)
                {
                    Add(key, pair.value);
                }
                else
                {
                    Debug.LogError($"Failed to convert key to type {typeof(TKey)}: {pair.key}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Exception in OnAfterDeserialize: {ex.Message}");
            }
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        pairs.Clear();
        foreach (var item in this)
        {
            pairs.Add(new KeyValue{
                key = item.Key.ToString(),
                value = item.Value
            });
        }
    }
    
    [Serializable]
    private struct KeyValue
    {
        public string key;
        public TValue value;
    }
}

