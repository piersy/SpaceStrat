using UnityEngine;

public abstract class PausableComponent : MonoBehaviour
{

    void Awake()
    {
        Game.OnPaused += OnPaused;
    }
                      
    void OnDestroy()
    {
        Game.OnPaused -= OnPaused;
    }

    protected abstract void OnPaused(bool paused);
}
