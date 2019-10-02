using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System;

namespace Assets.Icaros.Mobile.Scripts.UI
{
    public class UIHighscoreList : MonoBehaviour
    {
        public static UIHighscoreList Instance = null;

        private Transform entryContainer;
        private Transform entryTemplate;

        private static HighscoreEntry newHighscoreEntry;
        private List<Tuple<Text, string>> flashingText;

        private List<Transform> highscoreEntryTransformList;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            UpdateHighscoreList();
        }

        private void UpdateHighscoreList(Highscores highscores = null)
        {
            entryContainer = transform.Find("highscoreEntryContainer");
            entryTemplate = entryContainer.Find("highscoreEntryTemplate");

            entryTemplate.gameObject.SetActive(false);
            if (highscores == null)
            {
                var jsonString = PlayerPrefs.GetString("highscoreTable");
                highscores = JsonUtility.FromJson<Highscores>(jsonString);
            }
            // Cleanup
            if (highscoreEntryTransformList != null)
            {
                foreach (var entry in highscoreEntryTransformList)
                {
                    Destroy(entry.gameObject);
                }
            }

            if (newHighscoreEntry == null) { flashingText = null; }

            highscoreEntryTransformList = new List<Transform>();
            foreach (var highscoreEntry in highscores.highscoreEntryList.OrderByDescending(elem => elem.score).Take(10).ToList())
            {
                CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList, (highscoreEntry == newHighscoreEntry));
            }
            newHighscoreEntry = null;
        }

        private void CreateHighscoreEntryTransform(HighscoreEntry entry, Transform container, List<Transform> transformList, bool isNewHighscoreEntry)
        {
            float templateHeight = 30f;
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
            entryTransform.gameObject.SetActive(true);

            int rank = transformList.Count + 1;
            var posText = entryTransform.Find("posText").GetComponent<Text>();
            posText.text = rank.ToString();

            string name = entry.name;
            var nameText = entryTransform.Find("nameText").GetComponent<Text>();
            nameText.text = name;
            

            float score = entry.score;
            var scoreText = entryTransform.Find("scoreText").GetComponent<Text>();
            scoreText.text = score.ToString();

            if (isNewHighscoreEntry)
            {
                posText.color = Color.yellow;
                nameText.color = Color.yellow;
                scoreText.color = Color.yellow;
                //Coroutine not working
                //flashingText = new List<Tuple<Text, string>>();
                //flashingText.Add(new Tuple<Text, string>(posText, rank.ToString()));
                //flashingText.Add(new Tuple<Text, string>(nameText, name));
                //flashingText.Add(new Tuple<Text, string>(scoreText, score.ToString()));
                //StartCoroutine(nameof(BlinkText));
            }


            transformList.Add(entryTransform);
        }

        private class Highscores
        {
            public List<HighscoreEntry> highscoreEntryList;
        }


        [System.Serializable]
        private class HighscoreEntry
        {
            public string name;
            public float score;
        }

        public static void AddScoreBoardEntry(string name, float score)
        {
            Highscores highscores = null;
            var highscoreEntry = new HighscoreEntry() { name = name, score = score };

            var jsonString = PlayerPrefs.GetString("highscoreTable");
            if (string.IsNullOrEmpty(jsonString)) {
                highscores = new Highscores() {
                    highscoreEntryList = new List<HighscoreEntry>()
                };
            }
            else {
                highscores = JsonUtility.FromJson<Highscores>(jsonString);
            }
            
            
            newHighscoreEntry = highscoreEntry;
            
            highscores.highscoreEntryList.Add(highscoreEntry);
           
            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("highscoreTable", json);
            PlayerPrefs.Save();
            Debug.Log(PlayerPrefs.GetString("highscoreTable"));

            Instance.UpdateHighscoreList(highscores);
        }

        private IEnumerator BlinkText()
        {
            while (flashingText != null)
            {
                foreach (var entry in flashingText)
                {
                    entry.Item1.text = "";
                }
                yield return new WaitForSeconds(.5f);
                foreach (var entry in flashingText)
                {
                    entry.Item1.text = entry.Item2;
                }
                yield return new WaitForSeconds(.5f);
            }
        }
    }
}
