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
            if(_serializedItems == null)
            {
                _serializedItems = new List<T>();

                foreach (var kvp in this.fields)
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


        }
    }

    public int Count => throw new System.NotImplementedException();

    public bool IsReadOnly => throw new System.NotImplementedException();

    public void Add(T item)
    {
        UpdateFieldInfo(item);
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public bool Contains(T item)
    {
        throw new System.NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    public int IndexOf(T item)
    {
        throw new System.NotImplementedException();
    }

    public void Insert(int index, T item)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(T item)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
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
