using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private static readonly Vector3 DEFAULT_VECTOR = new Vector3(-1f, 1f, 0f).normalized;

    private Vector3 lastPosition;

    void Update()
    {
        Vector3 delta = (transform.position - lastPosition).normalized;

        this.transform.rotation = Quaternion.FromToRotation(DEFAULT_VECTOR, delta);

        this.lastPosition = this.transform.position;
    }
}
