using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    protected abstract string Name { get; set; }

    protected abstract int Cost { get; set; }

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
