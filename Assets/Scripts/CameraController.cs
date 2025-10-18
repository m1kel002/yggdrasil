using System;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform target;
    [SerializeField]
    private float distanceFromPlayer = 15f;

    [SerializeField]
    private float height = 6f;

    [SerializeField]
    private float heightOffset = 1.5f;

    [SerializeField]
    private float rotationSpeed = 5f;

    [SerializeField]
    private float cameraTilt = -35f;

    public LayerMask collisionMask;

    void LateUpdate()
    {
        if (!target)
        {
            Debug.LogError("Target is missing");
        }

        Quaternion rotation = Quaternion.Euler(cameraTilt, 0, 0);
        Vector3 direction = rotation * Vector3.forward;
        transform.position = target.position + Vector3.up * height + direction * distanceFromPlayer;
        Vector3 lookPoint = target.position + Vector3.up;
        Quaternion lookRotation = Quaternion.LookRotation(lookPoint - transform.position);
        transform.rotation = lookRotation;
    }

}
