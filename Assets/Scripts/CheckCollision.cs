using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CheckCollision : MonoBehaviour
{
    public string targetTag;
    public Callback callback;
    public Vector3 dir;
    public float speed;

    private Rigidbody body;
    public delegate void Callback();

    private void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        body.AddForce(dir * speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Call the callback method if it's set
        if (callback != null && collision.transform.CompareTag(targetTag))
        {
            callback();
        }

        Destroy(gameObject);
    }
}
