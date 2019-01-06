using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// This class is responsible for all language localization.
    /// For example, we may give an item an internal name of apple,
    /// but if we want to support multiple languages, we should display 'apple' in
    /// that language that is currently selected.
    /// </summary>
    public static class LocalizationHandler
    {
        /// <summary>
        /// This dictionary contains all the localization information for the currently selected language.
        /// TKey = unlocalized text
        /// TValue = localized text
        /// </summary>
        private static Dictionary<string, string> localizationDefinitions = new Dictionary<string, string>();

        /// <summary>
        /// Removes all the localization definitions that are currently loaded into the game.
        /// </summary>
        public static void ClearLocalizationDefinitions()
        {
            localizationDefinitions.Clear();
        }

        /// <summary>
        /// Goes through every line in the languageAsset and adds it to the localization dictionary.
        /// Every line in the languageAsset should follow the format:
        /// unlocalized name=localized name
        /// If a line doesn't follow that format, it is ignored and a warning is logged.
        /// </summary>
        /// <param name="languageAsset">A text asset containing localization data.</param>
        public static void IncludeLanguageFile(TextAsset languageAsset)
        {
            //Every line in the text asset.
            string[] lines = languageAsset.text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                //Process the current line as a potential localization term.
                AddLanguageDefinition(lines[i]);
            }
        }
        /// <summary>
        /// Goes through every line in the languageStream and adds it to the localization dictionary.
        /// Every line in the languageStream should follow the format:
        /// unlocalized name=localized name
        /// If a line doesn't follow that format, it is ignored and a warning is logged.
        /// </summary>
        /// <param name="languageStream">A stream of text containing localization data.</param>
        public static void IncludeLanguageFile(Stream languageStream)
        {
            //Create a stream reader to read individual lines from the language stream...
            StreamReader reader = new StreamReader(languageStream);

            //Read until at the end of the stream...
            while (!reader.EndOfStream)
            {
                //Read the current line of text and process it as a potential localization term.
                AddLanguageDefinition(reader.ReadLine());
            }
        }

        /// <summary>
        /// Splits a line of text into a key and value. The key is the unlocalized text and the value is the localized text.
        /// If the line starts with '//', the line is a comment so the line will not be added as a language localization term.
        /// </summary>
        /// <param name="localizationDefinition">The key and value pair, connected by '=', to add as a localization term.</param>
        private static void AddLanguageDefinition(string localizationDefinition)
        {
            //If the localization definition starts with '//', it is a comment and should be ignored.
            if (localizationDefinition.StartsWith("//"))
                return;

            string[] args = localizationDefinition.Split('=');
            //If the localization definition only contains one '=' character, the localization definition is valid and can be used for localization...
            if (args.Length == 2)
                localizationDefinitions[args[0]] = args[1];
            else
                Debug.LogWarning("Expected: \'unlocalized_name=localized_name\'\nGot: \'" + localizationDefinition + "\'");
        }

        /// <summary>
        /// Localizes a piece of text and if no localization was found, the unlocalized text is returned.
        /// </summary>
        /// <param name="unlocalizedText">The unlocalized text to localize.</param>
        /// <returns>Localized text for the unlocalized text or the unlocalized text if no localization was found.</returns>
        public static string GetLocalizedText(string unlocalizedText)
        {
            string localizedText;
            //If the localizationDictionary contains the key-value-pair for unlocalizedText, return the localized text.
            if (localizationDefinitions.TryGetValue(unlocalizedText, out localizedText))
                return localizedText;
            //If the localizationDictionary doesn't contain the key-value-pair, return the unlocalized text.
            return unlocalizedText;
        }
    }
}
