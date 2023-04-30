using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            instance ??= FindAnyObjectByType<T>();
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning($"Found duplicate instance of {typeof(T)} in game object {gameObject.name}");
            return;
        }
        instance = Instance;
        SingletonAwake();
    }

    protected void SingletonAwake() {}
}
