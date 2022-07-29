using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private void Awake()
    {
        this.SetAngleToCamera();
    }

    private void Update()
    {
        this.SetAngleToCamera();
    }

    private void SetAngleToCamera()
    {
        this.transform.rotation = Camera.main.transform.rotation;
    }
}
