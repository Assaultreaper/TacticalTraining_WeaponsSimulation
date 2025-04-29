using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance 
    {
        get 
        {
            if (_instance == null)
            {
                // Try to find an existing instance
                _instance = FindObjectOfType<T>();

                // If still null, create a new GameObject
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // If an instance already exists and it's not this, destroy this
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning("Duplicate Instance Found, Removing Excess");
            return;
        }

        _instance = this as T;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"Instance Set To {gameObject.name}");
    }
}
