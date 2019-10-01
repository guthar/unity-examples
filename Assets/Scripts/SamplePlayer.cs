using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePlayer : MonoBehaviour
{
    public ScoreManager scoreManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(0, 0, 1000);
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(0, 30, -300);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        scoreManager.Collect(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        scoreManager.Collect(other.gameObject);
    }
}
