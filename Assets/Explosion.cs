﻿using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField]
    private float destroyTime = 3;

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
