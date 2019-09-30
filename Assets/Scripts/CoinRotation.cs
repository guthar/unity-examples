using UnityEngine;

public class CoinRotation : MonoBehaviour
{
    /// <summary>
    /// Stärke der Rotation.
    /// </summary>
    public float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }
}
