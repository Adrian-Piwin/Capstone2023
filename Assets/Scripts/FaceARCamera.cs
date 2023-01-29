using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceARCamera : MonoBehaviour
{
    public Transform facing;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(new Vector3(facing.position.x, transform.position.y, facing.position.z));
    }
}
