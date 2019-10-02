using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlentosManager : MonoBehaviour {
    #region Einstellungen: Bank/Weltspartag
    /// <summary>
    /// Audio Clip, der das Einzahlen darstellt.
    /// </summary>
    [Header("Bank/Weltspartag")]
    [Tooltip("Ton, der abgespielt wird, wenn das Geld eingezahlt wird.")]
    public AudioClip accountAudioClip;

    /// <summary>
    /// Objekt zur Anzeige des Raiffeisen Logos auf der Sonne.
    /// </summary>
    [Tooltip("Enthält das Objekt, das zur Anzeige des Raiffeisen Logos auf der Sonne verwendet wird.")]
    public GameObject sunRaiffeisenLogo;

    /// <summary>
    /// Zinsenfaktor.
    /// Um diesen Faktor erhält man mehr Punkte, wenn man das gesammelte Geld auf der Bank einzahlt.
    /// </summary>
    [Tooltip("Definiert die Zinsen, die man beim Einzahlen in der Bank erhält.")]
    [Range(0.0f, 0.9f)]
    public float accountingInterest = 0.5f;

    /// <summary>
    /// Bestimmt das Intervall in Sekunden, innerhalb dessen ein Weltspartag begonnen wird.
    /// </summary>
    [Tooltip("Bestimmt das Intervall in Sekunden, innerhalb dessen zufällig ein Weltspartag begonnen wird.")]
    [Range(10, 200)]
    public int weltspartagInterval = 90;

    /// <summary>
    /// Dauer des Weltspartags in Sekunden.
    /// </summary>
    [Tooltip("Bestimmt die Dauer des Weltspartags in Sekunden.")]
    [Range(10, 30)]
    public int weltspartagDuration = 20;

    /// <summary>
    /// Weltspartagsfaktor.
    /// Um diesen Faktor erhält man am Weltspartag mehr Zinsen als an einem normalen Tag.
    /// </summary>
    [Tooltip("Definiert den Faktor, um den man am Weltspartag mehr Zinsen erhält.")]
    [Range(0.0f, 3.0f)]
    public float weltspartagFactor = 2.0f;
    #endregion
    #region Einstellungen: Geldobjekte
    /// <summary>
    /// Liste der Positionen, an denen die Geldsymbole erzeugt werden.
    /// </summary>
    [Header("Geldobjekte")]
    [Tooltip("Liste der Punkte in der Spiellandschaft, an denen Geldobjekte dargestellt werden können.")]
    public Transform[] moneyObjectPositions;

    /// <summary>
    /// Objekt, das als Münze instanziert wird.
    /// </summary>
    [Tooltip("Objekt, das als Münze dargestellt wird.")]
    public GameObject coinGameObject;

    /// <summary>
    /// Objekt, das als Geldschein instanziert wird.
    /// </summary>
    [Tooltip("Objekt, das als Geldschein dargestellt wird.")]
    public GameObject noteGameObject;

    /// <summary>
    ///  Anzahl der Sekunden, alle die ein Geldobjekt erstellt wird.
    /// </summary>
    [Tooltip("Definiert das Intervall, in dem Geldobjekte angelegt werden.")]
    public int moneyObjectCreationInterval = 5;

    /// <summary>
    /// Definiert die Höhe, in der Geldobjekte maximal angezeigt werden.
    /// </summary>
    [Tooltip("Definiert die Höhe, in der Geldobjekte maximal erstellt werden.")]
    public float moneyObjectMaximumHeight = 50f;
    #endregion
    #region Einstellungen: Anleitung
    [Tooltip("Hinweis, die Sonne zu beobachten.")]
    public AudioClip hintLookAtSunAudioClip;
    [Tooltip("Hinweis, aus dem Wasser zu springen.")]
    public AudioClip hintJumpOverWaterAudioClip;
    [Tooltip("Hinweis, Geld an der Bank abzuliefern.")]
    public AudioClip hintAccountAtBankAudioClip;
    [Tooltip("Hinweis, Wasserspiegel steigen lassen.")]
    public AudioClip hintRaiseWaterAudioClip;
    #endregion
    #region Einstellungen: Wasserstand
    /// <summary>
    /// Stellt die Wasseroberfläche dar.
    /// </summary>
    [Tooltip("Wasseroberfläche.")]
    public GameObject aqua;

    /// <summary>
    /// Definiert den Wasserstand in der Stadt.
    /// </summary>
    [Tooltip("Wasserstand.")]
    public float aquaLevel = 20.0f;

    public float minAquaLevel = 10.0f;
    public float maxAquaLevel = 40.0f;
    public float floodAndDrainAmount = 5.0f;
    #endregion
    #region Einstellungen: Spielzeit
    /// <summary>
    /// Gesamte Spielzeit in Sekunden.
    /// </summary>
    [Tooltip("Gesamte Spielzeit in Sekunden.")]
    [Range(30, 600)]
    public int totalGameTime = 300;

    [Tooltip("Ton, der zum Start des Spiels abgespielt wird.")]
    public AudioClip startAudioClip;
    [Tooltip("Ton, der zur Hälfte des Spiels abgespielt wird.")]
    public AudioClip halfTimeOverAudioClip;
    [Tooltip("Ton, der nahe dem Ende des Spiels abgespielt wird.")]
    public AudioClip nearEndAudioClip;
    [Tooltip("Ton, der zum Ende des Spiels abgespielt wird.")]
    public AudioClip endAudioClip;
    #endregion

    public AudioSource GlobalAudioSource;

    /// <summary>
    /// Kennzeichen, ob das Spiel läuft oder nicht.
    /// </summary>
    private bool isPlaying = true;

    /// <summary>
    /// Gesamtspielstand
    /// </summary>
    private float totalScore = 0f;

    /// <summary>
    /// Kennzeichen, ob der Weltspartag gerade aktiv ist.
    /// </summary>
    private bool isWeltspartag = false;

    // Start is called before the first frame update
    void Start() {
        // Spielstand zurücksetzen
        totalScore = 0f;

        // Coroutinen starten
        StartCoroutine(nameof(weltspartagCoroutine));
        StartCoroutine(nameof(moneyObjectsCoroutine));
        StartCoroutine(nameof(audioOutputCoroutine));
    }

    // Update is called once per frame
    void Update() {
        // Wasserstand berechnen
        if ((isFlooding) || (isDraining)) {
            updateAquaLevel();
        }
    }

    #region Ereignisse: Bank/Weltspartag
    /// <summary>
    /// Sammelt das Geldobjekt ein.
    /// </summary>
    /// <param name="moneyObject">[in] gesammeltes Geldobjekt</param>
    /// <param name="moneyObjectProperties">[in] Informationen zum eingesammelten Geldobjekt.</param>
    public void CollectMoneyObject(
        GameObject moneyObject,
        MoneyObjectProperties moneyObjectProperties) {
        // Spielstand übernehmen
        totalScore += moneyObjectProperties.ScoreValue;

        // Geldobjekt zerstören
        destroyMoneyObject(moneyObject);
    }

    /// <summary>
    /// Gibt das gesammelte Geld an der Bank ab.
    /// </summary>
    /// <param name="scoreValue">[in] Wert des abzugebenden Geldes.</param>
    public void AccountMoney(float scoreValue, Vector3 position) {
        // Einzahlungston abspielen
        if (scoreValue > 0f) {
            AudioSource.PlayClipAtPoint(accountAudioClip, position);
        }

        // Wert übernehmen
        totalScore += scoreValue;
        float interest = scoreValue * accountingInterest;
        if (isWeltspartag) {
            interest += (interest * weltspartagFactor);
        }
        totalScore += interest;
    }
    #endregion
    #region Ereignisse: Wasserstand
    private bool isFlooding = false;
    private bool isDraining = false;
    /// <summary>
    /// Lässt den Wasserstand steigen.
    /// </summary>
    public void Flood() {
        isFlooding = true;
        isDraining = false;
    }

    /// <summary>
    /// Lässt den Wasserstand wieder ablaufen.
    /// </summary>
    public void Drain() {
        isFlooding = false;
        isDraining = true;
    }

    /// <summary>
    /// Verändert den Wasserspiegel.
    /// </summary>
    private void updateAquaLevel() {
        float newAquaLevel = aquaLevel;
        if (isFlooding) {
            newAquaLevel = Mathf.Min(aquaLevel + (floodAndDrainAmount * Time.deltaTime), maxAquaLevel);
            if (newAquaLevel == maxAquaLevel) {
                isFlooding = false;
            }
        }
        else if (isDraining) {
            newAquaLevel = Mathf.Max(aquaLevel - (floodAndDrainAmount * Time.deltaTime), minAquaLevel);
            if (newAquaLevel == minAquaLevel) {
                isDraining = false;
            }
        }

        if (aquaLevel != newAquaLevel) {
            aquaLevel = newAquaLevel;
            aqua.transform.position.Set(
                aqua.transform.position.x,
                aquaLevel,
                aqua.transform.position.z);
        }
    }
    #endregion

    // zeitgesteuerte Routinen

    #region Zeitsteuerung: Weltspartag
    /// <summary>
    /// Führt zeitgesteuert das Aktivieren/Deaktivieren des Weltspartages durch.
    /// </summary>
    /// <returns></returns>
    private IEnumerator weltspartagCoroutine() {
        // Initialisierung der Weltspartagslogik
        isWeltspartag = false;
        DateTime toggleWeltspartag = getNextWeltspartagStart();

        while (isPlaying) {
            // alle Sekunden die Verarbeitung starten
            yield return new WaitForSeconds(1.0f);

            // Zeit auswerten
            if (isWeltspartag) {
                // wenn der Endezeitpunkt erreicht ist, den Weltspartag beenden
                if (toggleWeltspartag < DateTime.Now) {
                    setWeltspartag(false);
                    toggleWeltspartag = getNextWeltspartagStart();
                }
            }
            else {
                // wenn der Startzeitpunkt erreicht ist, den Weltspartag beginnen
                if (toggleWeltspartag < DateTime.Now) {
                    setWeltspartag(true);
                    toggleWeltspartag = getCurrentWeltspartagEnd();
                }
            }
        }
    }

    /// <summary>
    /// Ermittelt vom aktuellen Zeitpunkt aus die nächste Aktivierung des Weltspartages
    /// innerhalb der definierten Anzahl an Sekunden.
    /// </summary>
    /// <returns>Beginnzeitpunkt des nächsten Weltspartages.</returns>
    private DateTime getNextWeltspartagStart() {
        // zufällige Anzahl an Sekunden ermitteln und Zeitpunkt berechnen
        DateTime result = getTimeAfterRandomSeconds((float)weltspartagInterval);
        return result;
    }

    /// <summary>
    /// Ermittelt vom aktuellen Zeitpunkt aus das Ende des aktiven Weltspartages
    /// innerhalb der definierten Anzahl an Sekunden.
    /// </summary>
    /// <returns>Endezeitpunkt des aktiven Weltspartages.</returns>
    private DateTime getCurrentWeltspartagEnd() {
        // zufällige Anzahl an Sekunden ermitteln und Zeitpunkt berechnen
        DateTime result = getTimeAfterRandomSeconds((float)weltspartagDuration);
        return result;
    }

    /// <summary>
    /// Aktiviert bzw. deaktiviert den Weltspartag.
    /// </summary>
    /// <param name="active">[in] Kennzeichen, ob der Weltspartag aktiv ist.</param>
    void setWeltspartag(bool active) {
        isWeltspartag = active;
        if (sunRaiffeisenLogo != null) {
            sunRaiffeisenLogo.SetActive(active);
        }
    }
    #endregion
    #region Zeitsteuerung: Geldobjekte
    /// <summary>
    /// Liste der erzeugten Geldobjekte.
    /// </summary>
    private GameObject[] createdMoneyObjects;

    /// <summary>
    /// Führt zeitgesteuert das Anzeigen von Geldobjekten durch.
    /// </summary>
    /// <returns></returns>
    private IEnumerator moneyObjectsCoroutine() {
        // Initialisierung der Anzeigelogik
        createdMoneyObjects = new GameObject[moneyObjectPositions.Length];
        DateTime nextCreationTime = getNextCreationTime();

        while (isPlaying) {
            // alle Sekunden die Verarbeitung starten
            yield return new WaitForSeconds(1.0f);

            // Zeit auswerten
            if (nextCreationTime < DateTime.Now) {
                // freien, zufälligen Index der Position bestimmen
                int creationIndex = getCreationIndex();
                if (creationIndex >= 0) {
                    // zufällig das Geldobjekt bestimmen, das erzeugt werden soll
                    GameObject moneyObjectTemplate = getMoneyObjectTemplate();

                    // zufällig die Position bestimmen
                    Transform createPosition = getCreatePosition(moneyObjectPositions[creationIndex]);

                    // Instanzierung durchführen
                    createdMoneyObjects[creationIndex] =
                        GameObject.Instantiate<GameObject>(moneyObjectTemplate, createPosition);
                }

                // nächsten Zeitpunkt für das Erzeugen bestimmen
                nextCreationTime = getNextCreationTime();
            }
        }
    }

    /// <summary>
    /// Ermittelt vom aktuellen Zeitpunkt aus das nächste Erzeugen eines Geldobjektes
    /// innerhalb der definierten Anzahl an Sekunden.
    /// </summary>
    /// <returns>Beginnzeitpunkt des nächsten Erzeugens.</returns>
    private DateTime getNextCreationTime() {
        // zufällige Anzahl an Sekunden ermitteln und Zeitpunkt berechnen
        DateTime result = getTimeAfterRandomSeconds((float)moneyObjectCreationInterval);
        return result;
    }

    /// <summary>
    /// Ermittelt den Index, an dem das Geldobjekt erzeugt werden soll.
    /// </summary>
    /// <returns>Index des Objektslots, oder -1 wenn keiner mehr frei ist.</returns>
    private int getCreationIndex() {
        // Schritt 1: 5 zufällige Indexe ermitteln und freie Slots suchen
        int result = -1;
        int retries = 5;
        while (retries > 0) {
            // zufälligen Index ermitteln
            int randomIndex =
                (int)(UnityEngine.Random.value * ((float)moneyObjectPositions.Length));
            if (createdMoneyObjects[randomIndex] == null) {
                result = randomIndex;
                break;
            }

            // nächster Versuch
            retries--;
        }

        // Schritt 2: ersten freien Index suchen
        if (result < 0) {
            for (int index = 0; index < createdMoneyObjects.Length; index++) {
                if (createdMoneyObjects[index] == null) {
                    result = index;
                    break;
                }
            }
        }

        return result;
    }

    /// <summary>
    /// Ermittelt das zu erzeugende Geldobjekt.
    /// </summary>
    /// <returns>Zu erzeugendes Geldobjekt.</returns>
    private GameObject getMoneyObjectTemplate() {
        // zufällig das zu erzeugende Geldobjekt bestimmen
        GameObject result;
        if ((coinGameObject != null) && (noteGameObject != null)) {
            result =
                ((UnityEngine.Random.value < 0.5f) ?
                coinGameObject :
                noteGameObject);
        }
        else if ((coinGameObject != null) && (noteGameObject == null)) {
            result = coinGameObject;
        }
        else if ((coinGameObject == null) && (noteGameObject != null)) {
            result = noteGameObject;
        }
        else {
            result = null;
        }

        return result;
    }

    /// <summary>
    /// Berechnet die Position, an der das Geldobjekt erzeugt wird.
    /// </summary>
    /// <param name="definedPosition">[in] Definierte Position.</param>
    /// <returns>Errechnete Position.</returns>
    private Transform getCreatePosition(Transform definedPosition) {
        // Position anpassen
        Vector3 position = new Vector3(
            definedPosition.position.x,
            UnityEngine.Random.value * moneyObjectMaximumHeight,
            definedPosition.position.z);
        Transform result = definedPosition;
        result.SetPositionAndRotation(
            position,
            definedPosition.rotation);
        return result;
    }

    /// <summary>
    /// Zerstört und entfernt das Geldobjekt.
    /// </summary>
    /// <param name="moneyObject">[in] zu entfernendes Geldobjekt</param>
    private void destroyMoneyObject(GameObject moneyObject) {
        // GameObject entfernen
        if (createdMoneyObjects != null) {
            for (int index = 0; index < createdMoneyObjects.Length; index++) {
                if (System.Object.Equals(createdMoneyObjects[index], moneyObject)) {
                    createdMoneyObjects[index] = null;
                    break;
                }
            }
        }

        moneyObject.SetActive(false);
        UnityEngine.Object.Destroy(moneyObject);
    }
    #endregion
    #region Zeitsteuerung: Audioausgabe
    /// <summary>
    /// Führt zeitgesteuert die Audioausgabe durch.
    /// </summary>
    /// <returns></returns>
    private IEnumerator audioOutputCoroutine() {


        // Zeitpunkte berechnen

        // Berechnung für 120 Sekunden
        // 000-004  Start
        // 020-030  Jump over water
        // 040-048  Account at bank
        // 060-063  Half of time over
        // 080-088  Look at sun
        // 090-101  Raise water
        // 105-108  Near end
        // 120-124  End


        int secondsElapsed = 0;
        int halfTimeOverSeconds = totalGameTime / 2;
        int nearEndSeconds = totalGameTime - 15;
        int endSeconds = totalGameTime;

        int hintJumpOverWaterSeconds = (int)(((float)totalGameTime) * 1f / 6f);   // nach 1/6 der Spielzeit
        int hintAccountAtBankSeconds = (int)(((float)totalGameTime) * 1f / 3f);   // nach 1/3 der Spielzeit
        int hintLookAtSunSeconds = (int)(((float)totalGameTime) * 2f / 3f);       // nach 2/3 der Spielzeit
        int hintRaiseWaterSeconds = (int)(((float)totalGameTime) * 3f / 4f);      // nach 3/4 der Spielzeit

        while (isPlaying) {
            yield return new WaitForSeconds(1.0f);
            if (secondsElapsed++ <= 0) {
                // Ausgabe der Audio Clips durchführen
                playAudioClip(startAudioClip);
            }

            if (halfTimeOverSeconds < secondsElapsed) {
                playAudioClip(halfTimeOverAudioClip);
                halfTimeOverSeconds = int.MaxValue;
            }
            else if (nearEndSeconds < secondsElapsed) {
                playAudioClip(nearEndAudioClip);
                nearEndSeconds = int.MaxValue;
            }
            else if (endSeconds < secondsElapsed) {
                playAudioClip(endAudioClip);
                endSeconds = int.MaxValue;
            }
            else if (hintLookAtSunSeconds < secondsElapsed) {
                playAudioClip(hintLookAtSunAudioClip);
                hintLookAtSunSeconds = int.MaxValue;
            }
            else if (hintJumpOverWaterSeconds < secondsElapsed) {
                playAudioClip(hintJumpOverWaterAudioClip);
                hintJumpOverWaterSeconds = int.MaxValue;
            }
            else if (hintAccountAtBankSeconds < secondsElapsed) {
                playAudioClip(hintAccountAtBankAudioClip);
                hintAccountAtBankSeconds = int.MaxValue;
            }
            else if (hintRaiseWaterSeconds < secondsElapsed) {
                playAudioClip(hintRaiseWaterAudioClip);
                hintRaiseWaterSeconds = int.MaxValue;
            }

            if (totalGameTime < secondsElapsed) {
                isPlaying = false;
            }
        }
    }
    #endregion

    /// <summary>
    /// Gibt einen Zeitpunkt zurück, der innerhalb der angegebenen Sekunden zufällig
    /// nach dem aktuellen Zeitpunkt liegt.
    /// </summary>
    /// <param name="randomSecondAmount">[in] Anzahl der Sekunden, die der ermittelte Zeitpunkt maximal nach dem aktuellen Zeitpunkt liegen soll.</param>
    /// <returns>Ermittelter Zeitpunkt.</returns>
    private DateTime getTimeAfterRandomSeconds(float randomSecondAmount) {
        // zufällige Anzahl an Sekunden ermitteln und Zeitpunkt berechnen
        int randomSeconds =
            (int)(UnityEngine.Random.value * randomSecondAmount);
        DateTime result = DateTime.Now + new TimeSpan(0, 0, randomSeconds);
        return result;
    }

    /// <summary>
    /// Spielt einen Audio Clip ab, und prüft dabei ob einer verfügbar ist.
    /// </summary>
    /// <param name="audioClip">[in] Audio Clip, der abgespielt werden soll.</param>
    private void playAudioClip(AudioClip audioClip) {
        if (audioClip != null) {
            Debug.Log("[AtlentosManager].[playAudioClip] " + audioClip.name);
            //AudioSource.PlayClipAtPoint(audioClip, playerPosition.position);
            GlobalAudioSource.clip = audioClip;
            GlobalAudioSource.Play();
        }
        else {
            Debug.Log("[AtlentosManager].[playAudioClip] null");
        }
    }
}
