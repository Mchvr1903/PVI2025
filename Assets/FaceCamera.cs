using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Arahkan text untuk selalu menghadap kamera
        transform.rotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position);
    }
}