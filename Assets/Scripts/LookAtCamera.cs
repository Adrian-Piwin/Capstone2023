using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public string cameraName;
    private Transform lookTarget;

    void Start()
    {
        lookTarget = GameObject.Find(cameraName).transform;

        Vector3 direction = lookTarget.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = lookTarget.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
    }
}
