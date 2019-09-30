using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Icaros.Mobile.Localization {
    public class LocalizationManager : MonoBehaviour {
        public const string EMPTY_STRING = "<NULL>";
        public const string KEY_SAVED_LANGUAGE = "saved_language";

        public static LocalizationManager Instance = null;

        void Awake() {
            if (Instance != null) {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            string savedLanguagePreference = PlayerPrefs.GetString(KEY_SAVED_LANGUAGE);
            setLanguage(savedLanguagePreference);
            if (currentLanguage == null)
                setLanguage(defaultLanguage);
            if (currentLanguage == null)
                Debug.LogError("Selected default language does not exist!");
        }

        public string[] localizationFileNames = new string[1]{"Localization.csv"};
        public string defaultLanguage = "EN"; 

        [SerializeField]
        private Language[] Languages = new Language[0];
        [SerializeField]
        private Language currentLanguage = null;

        public static void SetLanguage(string languageID) {
            if (Instance == null)
                return;
            Instance.setLanguage(languageID);
        }
        internal void setLanguage(string languageID) {
            foreach (Language language in Languages) {
                if (language.id == languageID) {
                    currentLanguage = language;
                    PlayerPrefs.SetString(KEY_SAVED_LANGUAGE, languageID);
                    return;
                }
            }
            currentLanguage = null;
        }

        public static string Get(string tokenID) {
            if (Instance == null)
                return EMPTY_STRING;
            return Instance.get(tokenID);
        }
        internal string get(string tokenID) {
            if (currentLanguage == null)
                return EMPTY_STRING;

            foreach (KVP kvp in currentLanguage.tokens) {
                if (kvp.key.Equals(tokenID))
                    return kvp.value;
            }

            return EMPTY_STRING;
        }

        //this needs to be saved in UTF-8 for characters like ö,ä,ü to work
        public void loadFiles() {
            foreach (string file in localizationFileNames)
                loadFile(file);
        }

        void loadFile(string localizationFileName) {
                string path = Path.Combine(Application.dataPath, Path.Combine(Path.Combine(Path.Combine("Icaros", "Mobile"), "Localization"), localizationFileName));

            if (!File.Exists(path))
                return;

            string[] lines = File.ReadAllLines(path,System.Text.Encoding.UTF8);

            //line 1 will be all the language identifiers!
            string[] languageIDs = lines[0].Split(';');
            Languages = new Language[languageIDs.Length - 1];

            for (int i = 0; i < languageIDs.Length - 1; i++) {
                Languages[i] = new Language() { id = languageIDs[i + 1], tokens = new KVP[lines.Length] };
            }

            //parse all tokens for their respective language
            for (int line = 0; line < lines.Length; line++) {
                Debug.Log(lines[line]);
                string[] columns = lines[line].Split(';');

                for (int column = 1; column < columns.Length; column++) {
                    Languages[column - 1].tokens[line] = new KVP() { value = columns[column], key = columns[0] };
                }
            }

            setLanguage(defaultLanguage);
        }

        [System.Serializable]
        private class Language {
            public string id;
            public KVP[] tokens;
        }

        [System.Serializable]
        private class KVP {
            public string key;
            public string value;
        }
    }
}