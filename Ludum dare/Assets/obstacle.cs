using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]

public class obstacle : MonoBehaviour
{
    public float bumpForceMultiplier;
    public float bumpForceHeight;
    public float resetDistance = 50.0f;
    private Rigidbody _rb;
    private bool hasCollide = false;
    private Vector3 initPos;
    private Quaternion initRotation;
    private Transform player;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        initPos = transform.position;
        initRotation = transform.rotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !hasCollide)
        {
            if (player == null) player = collision.transform;
            Vector3 dir = ((transform.position - collision.transform.position).normalized) * bumpForceMultiplier;
            dir.y = bumpForceHeight;
            _rb.AddForce(dir, ForceMode.Impulse);
            hasCollide = true;
        }
    }

    public void Reset()
    {
        transform.position = initPos;
        transform.rotation = initRotation;
        hasCollide = false;
    }
    private void Update()
    {
        if(hasCollide)
        {
            if (Vector3.Distance(initPos, player.position) >= resetDistance)
                Reset();
        }
    }

}
