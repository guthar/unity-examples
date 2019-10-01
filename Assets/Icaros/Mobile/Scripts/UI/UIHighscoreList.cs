using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace Assets.Icaros.Mobile.Scripts.UI
{
    public class UIHighscoreList : MonoBehaviour
    {
        private Transform entryContainer;
        private Transform entryTemplate;

        private List<Transform> highscoreEntryTransformList;
        private void Awake()
        {
            entryContainer = transform.Find("highscoreEntryContainer");
            entryTemplate = entryContainer.Find("highscoreEntryTemplate");

            entryTemplate.gameObject.SetActive(false);

            var jsonString = PlayerPrefs.GetString("highscoreTable");
            Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

            highscoreEntryTransformList = new List<Transform>();
            foreach (var highscoreEntry in highscores.highscoreEntryList.OrderByDescending(elem => elem.score))
            {
                CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
            }
        }

        private void CreateHighscoreEntryTransform(HighscoreEntry entry, Transform container, List<Transform> transformList)
        {
            float templateHeight = 30f;
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
            entryTransform.gameObject.SetActive(true);

            int rank = transformList.Count + 1;
            entryTransform.Find("posText").GetComponent<Text>().text = rank.ToString();

            string name = entry.name;
            entryTransform.Find("nameText").GetComponent<Text>().text = name;

            int score = entry.score;
            entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

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
            public int score;
        }

        public static void AddScoreBoardEntry(string name, int score)
        {
            var highscoreEntry = new HighscoreEntry() { name = name, score = score };

            var jsonString = PlayerPrefs.GetString("highscoreTable");
            Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

            highscores.highscoreEntryList.Add(highscoreEntry);
           
            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("highscoreTable", json);
            PlayerPrefs.Save();
            Debug.Log(PlayerPrefs.GetString("highscoreTable"));
        }
    }
}
