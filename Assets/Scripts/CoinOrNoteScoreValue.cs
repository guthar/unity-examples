using UnityEngine;

public class CoinOrNoteScoreValue : MonoBehaviour
{
    /// <summary>
    /// Punktefaktor
    /// </summary>
    public int scoreFactor = 100;

    /// <summary>
    /// Gibt den Wert des Geldobjekts zurück.
    /// </summary>
    public int ScoreValue
    {
        get
        {
            int result = Mathf.Abs((int)(gameObject.transform.position.y * scoreFactor));
            return result;
        }
    }
}
