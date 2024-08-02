using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DespawnObjectByDistance : MonoBehaviour
{
    [SerializeField] protected float distanceLimit = 2f;
    [SerializeField] protected float distance = 0f;
    protected Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        Despawn();
    }

    protected void Despawn()
    {
        if (CanDespawn())
        {
            Destroy(gameObject);
        }
    }

    protected bool CanDespawn()
    {
        distance = Vector3.Distance(transform.position, startPos);
        if (distance > distanceLimit) return true;
        return false;
    }
}
