using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public abstract string Name { get; set; }

    public abstract int Hitpoints { get; set; }

    protected abstract void Initialize();

    private void Start()
    {
        Initialize();
    }

    protected abstract void UpdateState();

    private void Update()
    {
        UpdateState();
    }
}
