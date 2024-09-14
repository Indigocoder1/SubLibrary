using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace SubLibrary.SaveData;

internal class SerializableList<T> : IList<T>, ICollection<T>, IEnumerable<T>
{
    private Dictionary<T, List<object>> fields = new();
    private List<T> SerializedItems
    {
        get
        {
            if (_serializedItems == null)
            {
                _serializedItems = new List<T>();

                foreach (var kvp in fields)
                {
                    _serializedItems.Add((T)Activator.CreateInstance(typeof(T), kvp.Key));
                }
            }

            return _serializedItems;
        }
    }

    private List<T> _serializedItems;

    public T this[int index]
    {
        get
        {
            return SerializedItems[index];
        }
        set
        {
            SerializedItems[index] = value;
            UpdateFieldInfo(value);
        }
    }

    public int Count => SerializedItems.Count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        UpdateFieldInfo(item);
    }

    public void Clear()
    {
        SerializedItems.Clear();
        fields.Clear();
    }

    public bool Contains(T item)
    {
        return SerializedItems.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        SerializedItems.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return SerializedItems.GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return SerializedItems.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        SerializedItems.Insert(index, item);
    }

    public bool Remove(T item)
    {
        fields.Remove(item);
        return SerializedItems.Remove(item);
    }

    public void RemoveAt(int index)
    {
        fields.Remove(SerializedItems[index]);
        SerializedItems.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private void UpdateFieldInfo(T instance)
    {
        FieldInfo[] fields = typeof(T).GetFields();
        this.fields.Remove(instance);

        foreach (FieldInfo field in fields)
        {
            if (!this.fields.ContainsKey(instance))
            {
                this.fields.Add(instance, new List<object>() { field.GetValue(instance) });
            }
            else
            {
                this.fields[instance].Add(field.GetValue(instance));
            }
        }
    }
}
