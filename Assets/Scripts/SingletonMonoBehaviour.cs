using System;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Type t = typeof(T);
                _instance = FindObjectOfType(t) as T;
                Debug.Assert(_instance != null, t + "がオブジェクトにアタッチされていません");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            OnAwake();
            return;
        }

        if (_instance != this)
        {
            Destroy(this);
            Debug.LogError(typeof(T) + "は既に存在しています");
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            OnDispose();
        }
    }

    protected virtual void OnAwake() { }
    protected virtual void OnDispose() { }
}
