using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Checkpoint : MonoBehaviour
{
    public float timeToReach = 20f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            RaceManager.Instance.NextCheckPoint(this);
    }
}
