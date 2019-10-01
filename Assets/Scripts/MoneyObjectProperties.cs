using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyObjectProperties : MonoBehaviour
{
    /// <summary>
    /// Punktefaktor des Geldobjekts.
    /// </summary>
    [Tooltip("Faktor, mit dem die aktuelle Höhe multipliziert wird um den Wert zu bestimmen.")]
    [Range(0f, 100f)]
    public float scoreFactor = 100f;

    /// <summary>
    /// Gewicht des Geldobjekts.
    /// </summary>
    [Tooltip("Gewicht des Geldobjekts.")]
    [Range(0f, 10f)]
    public float weight = 10;

    /// <summary>
    /// Audio Clip, der beim Einsammeln abgespielt wird.
    /// </summary>
    [Tooltip("Ton, der beim Einsammeln abgespielt wird.")]
    public AudioClip collectAudioClip;

    /// <summary>
    /// Gibt den Wert des Geldobjekts zurück.
    /// </summary>
    public float ScoreValue
    {
        get
        {
            float result = Mathf.Abs(gameObject.transform.position.y * scoreFactor);
            return result;
        }
    }
}
