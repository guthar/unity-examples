using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// Aktueller Punktestand des Spielers.
    /// </summary>
    public int score = 0;

    /// <summary>
    /// Gibt den Punktewert des Geldobjekts auf Basis der Höhe zurück.
    /// </summary>
    /// <param name="collectedGameObject"></param>
    public void Collect(GameObject collectedGameObject)
    {
        // Sammelton abspielen
        AudioSource audioSourceComponent =
            collectedGameObject.GetComponent<AudioSource>();
        if (audioSourceComponent != null)
        {
            audioSourceComponent.Play();
        }

        // benötigte Komponente vom GameObject holen
        CoinOrNoteScoreValue coinOrNoteScoreValueComponent =
            collectedGameObject.GetComponent<CoinOrNoteScoreValue>();
        if (coinOrNoteScoreValueComponent != null)
        {
            score += coinOrNoteScoreValueComponent.ScoreValue;
        }
    }

}
