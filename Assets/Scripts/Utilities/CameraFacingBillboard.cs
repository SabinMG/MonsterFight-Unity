using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    private Camera _camera;

    void LateUpdate()
    {
        if(_camera == null) _camera = Camera.main; // caching main camera
        transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward, _camera.transform.rotation * Vector3.up);
    }
}