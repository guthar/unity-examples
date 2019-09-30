using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bankomat : MonoBehaviour
{
    // ********** Konfigurierbare Werte **********
    /// <summary>
    /// Liste der Positionen, an denen die Geldsymbole erzeugt werden.
    /// </summary>
    public Transform[] spawnPositions;

    /// <summary>
    /// Objekt, das als Münze instalziert wird.
    /// </summary>
    public GameObject coinGameObject;

    // Objekt, das als Geldschein instanziert wird.
    public GameObject noteGameObject;

    /// <summary>
    ///  Anzahl der Sekunden, alle die ein Geldobjekt erstellt wird.
    /// </summary>
    public int spawnDelay = 5;

    // ********** Interne Werte **********
    /// <summary>
    /// Liste der Geldobjekte an den jeweils definierten Punkten.
    /// </summary>
    private GameObject[] spawnedScoreObjects;
    private List<GameObject> spawnedScoreObjectsList;

    private int spawnIndex = 0;
    private bool spawnItem = false;
    private DateTime lastSpawnTime = DateTime.Now;
    private TimeSpan spawnDelayTimeSpan = TimeSpan.Zero;

    // Start is called before the first frame update
    void Start()
    {
        // Initialisierung der internen Werte
        spawnedScoreObjects = new GameObject[spawnPositions.Length];
        spawnDelayTimeSpan = new TimeSpan(0, 0, spawnDelay);
    }

    // Update is called once per frame
    void Update()
    {
        // Geldobjekte instanzieren
        if ((lastSpawnTime + spawnDelayTimeSpan) < DateTime.Now)
        {
            // wenn an der aktuellen Stelle kein Objekt existiert, die Instanzierung fortsetzen
            if (spawnedScoreObjects[spawnIndex] == null)
            {
                // Auswahl des Geldobjekttyps
                GameObject spawnItemGameObject = (spawnItem ? coinGameObject : noteGameObject);
                spawnItem = !spawnItem;

                // Instanzierung
                spawnedScoreObjects[spawnIndex] = GameObject.Instantiate<GameObject>(spawnItemGameObject, spawnPositions[spawnIndex]);

                spawnIndex++;
                if (spawnIndex >= spawnPositions.Length)
                {
                    spawnIndex = 0;
                }
            }

            lastSpawnTime = DateTime.Now;
        }
    }

    public void OnCollected(GameObject collectedGameObject)
    {
        // GameObject aus der Liste entfernen
        if (spawnedScoreObjects != null)
        {
            removeSpawnedScoreObject(collectedGameObject);
        }
    }

    void removeSpawnedScoreObject(GameObject spawnedScoreObject)
    {
        for (int index = 0; index < spawnedScoreObjects.Length; index++)
        {
            spawnedScoreObjects[index] = null;
        }
    }
}
