using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBehaviour : MonoBehaviour
{
    private float currentRotation = 90;

    public float Speed = 2;
    public float RotationDelta = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentRotation == 360)
        {
            currentRotation = 0;
        }

        currentRotation = currentRotation + RotationDelta * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        transform.Translate(0, 0, Speed * Time.deltaTime);
    }
}
