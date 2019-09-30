using UnityEngine;

public class CoinRotation : MonoBehaviour
{
    /// <summary>
    /// Stärke der Rotation.
    /// </summary>
    public float rotationSpeed = 50f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }
}
