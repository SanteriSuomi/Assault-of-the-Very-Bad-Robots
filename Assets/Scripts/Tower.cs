using UnityEngine;

public abstract class Tower : MonoBehaviour
{
    public abstract int Id  { get; set; }

    public abstract string Name { get; set; }

    public abstract int Cost { get; set; }

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
