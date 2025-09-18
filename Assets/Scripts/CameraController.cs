using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    Vector3 offest;

    private void Awake()
    {
        offest = transform.position - target.position;
    }

    private void LateUpdate()
    {
        transform.position = target.position + offest;
    }
}
