using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButton(2))
        {
            this.gameObject.transform.position += Vector3.left * Input.GetAxis("Mouse X") / 50f;
            this.gameObject.transform.position = new Vector3(Mathf.Clamp(this.gameObject.transform.position.x, -0.75f, 0.75f), this.gameObject.transform.position.y, this.gameObject.transform.position.z);
        }
    }
}
