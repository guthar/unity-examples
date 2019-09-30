using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteRotation : MonoBehaviour
{
    /// <summary>
    /// Stärke der Rotation.
    /// </summary>
    public float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        float rotationSpeedAdjusted = rotationSpeed * Time.deltaTime;
        transform.Rotate(rotationSpeedAdjusted, rotationSpeedAdjusted * 2, 0);
    }
}
