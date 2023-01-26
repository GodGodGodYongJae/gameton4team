using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DualKeyDictionary<TKey1, TKey2, TValue> : Dictionary<TKey1, Dictionary<TKey2, TValue>>
{
    #region 인덱서 - this[key1, key2]
    /// <summary>
    /// 인덱서
    /// </summary>
    /// <param name="key1">첫번째 키</param>
    /// <param name="key2">두번째 키</param>
    /// <returns>값</returns>
    public TValue this[TKey1 key1, TKey2 key2]
    {
        get
        {
            if (!ContainsKey(key1) || !this[key1].ContainsKey(key2))
            {
                throw new ArgumentOutOfRangeException();
            }

            return base[key1][key2];
        }
        set
        {
            if (!ContainsKey(key1))
            {
                this[key1] = new Dictionary<TKey2, TValue>();
            }

            this[key1][key2] = value;
        }
    }

    #endregion
    #region 값 열거형 - Values

    /// <summary>
    /// 값 열거형
    /// </summary>
    public new IEnumerable<TValue> Values
    {
        get
        {
            return from baseDictionary in base.Values
                   from baseKey in baseDictionary.Keys
                   select baseDictionary[baseKey];
        }
    }

    #endregion

    //////////////////////////////////////////////////////////////////////////////////////////////////// Method
    ////////////////////////////////////////////////////////////////////////////////////////// Public

    #region �߰��ϱ� - Add(key1, key2, value)

    /// <summary>
    /// 추가하기
    /// </summary>
    /// <param name="key1">첫번째 키</param>
    /// <param name="key2">두번째 키</param>
    /// <param name="value">값</param>
    public void Add(TKey1 key1, TKey2 key2, TValue value)
    {
        if (!ContainsKey(key1))
        {
            this[key1] = new Dictionary<TKey2, TValue>();
        }

        this[key1][key2] = value;
    }

    #endregion
    #region 키 포함 여부 구하기 - ContainsKey(key1, key2)

    /// <summary>
    /// 키 포함 여부 구하기
    /// </summary>
    /// <param name="key1">첫번째 키</param>
    /// <param name="key2">두번 키</param>
    /// <returns>키 포함 여부</returns>
    public bool ContainsKey(TKey1 key1, TKey2 key2)
    {
        return base.ContainsKey(key1) && this[key1].ContainsKey(key2);
    }
    #endregion
}
public class MultiKeyDictionary<K1, K2, V> : Dictionary<K1, Dictionary<K2, V>>
{
    public V this[K1 key1, K2 key2]
    {
        get
        {
            if (!ContainsKey(key1) || !this[key1].ContainsKey(key2))
                throw new ArgumentOutOfRangeException();
            return base[key1][key2];
        }
        set
        {
            if (!ContainsKey(key1))
                this[key1] = new Dictionary<K2, V>();
            this[key1][key2] = value;
        }
    }
    public void Add(K1 key1, K2 key2, V value)
    {
        if (!ContainsKey(key1))
            this[key1] = new Dictionary<K2, V>();
        this[key1][key2] = value;
    }
    public bool ContainsKey(K1 key1, K2 key2)
    {
        return base.ContainsKey(key1) && this[key1].ContainsKey(key2);
    }
    public new IEnumerable<V> Values
    {
        get
        {
            return from baseDict in base.Values
                   from baseKey in baseDict.Keys
                   select baseDict[baseKey];
        }
    }
}

public class MultiKeyDictionary<K1, K2, K3, V> : Dictionary<K1, MultiKeyDictionary<K2, K3, V>>
{
    public V this[K1 key1, K2 key2, K3 key3]
    {
        get
        {
            return ContainsKey(key1) ? this[key1][key2, key3] : default(V);
        }
        set
        {
            if (!ContainsKey(key1))
                this[key1] = new MultiKeyDictionary<K2, K3, V>();
            this[key1][key2, key3] = value;
        }
    }
    public bool ContainsKey(K1 key1, K2 key2, K3 key3)
    {
        return base.ContainsKey(key1) && this[key1].ContainsKey(key2, key3);
    }
    public void Add(K1 key1, K2 key2, K3 key3, V value)
    {
        if (!ContainsKey(key1))
            this[key1] = new MultiKeyDictionary<K2, K3, V>();
        this[key1][key2, key3] = value;
    }
}