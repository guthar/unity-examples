using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    /// <summary>
    /// Aktueller Punktestand des Spielers.
    /// </summary>
    public int score = 0;

    /// <summary>
    /// Geldausgabe.
    /// </summary>
    public Bankomat bankomat;

    /// <summary>
    /// Gibt den Punktewert des Geldobjekts auf Basis der Höhe zurück.
    /// </summary>
    /// <param name="collectedGameObject"></param>
    public void Collect(GameObject collectedGameObject)
    {
        Debug.Log(
            "[ScoreManager.Collect] " + collectedGameObject.name,
            this);

        // Sammelton abspielen
        CoinOrNoteAudioClip audioClipComponent =
            collectedGameObject.GetComponent<CoinOrNoteAudioClip>();
        if (audioClipComponent != null)
        {
            Debug.Log(
                "[ScoreManager.Collect] Coin Or Note Audio Clip Component " + audioClipComponent.name,
                audioClipComponent);
            if (audioClipComponent.collectAudioClip != null)
            {
                AudioSource.PlayClipAtPoint(audioClipComponent.collectAudioClip, collectedGameObject.transform.position);
            }
        }

        // benötigte Komponente vom GameObject holen
        CoinOrNoteScoreValue coinOrNoteScoreValueComponent =
            collectedGameObject.GetComponent<CoinOrNoteScoreValue>();
        if (coinOrNoteScoreValueComponent != null)
        {
            Debug.Log(
                "[ScoreManager.Collect] Coin Or Note Score Value Component " + coinOrNoteScoreValueComponent.name,
                coinOrNoteScoreValueComponent);
            score += coinOrNoteScoreValueComponent.ScoreValue;
        }

        Debug.Log(
            "[ScoreManager.Collect] Score = " + score.ToString(),
            this);

        // gesammeltes Geld markieren
        if (bankomat != null)
        {
            bankomat.OnCollected(collectedGameObject);
        }

        // Objekt zerstören
        if (coinOrNoteScoreValueComponent != null)
        {
            collectedGameObject.SetActive(false);
            UnityEngine.Object.Destroy(collectedGameObject);
        }
    }

}
