using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    // Don't destroy gameObjects this script is attached to on game awake.
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
