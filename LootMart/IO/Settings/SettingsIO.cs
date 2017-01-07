using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LootMart.IO.Settings
{
    public static class SettingsIO
    {
        public static GameSettings LoadGameSettings()
        {
            GameSettings newSettings = new GameSettings();
            try
            {
                Directory.CreateDirectory(Constants.IO.GameSettings.SettingsDirectory);
                if (!File.Exists(Constants.IO.GameSettings.CurrentSettings))
                {
                    ResetGameSettings();
                }
                XmlSerializer serializer = new XmlSerializer(newSettings.GetType());
                using (FileStream fileStream = new FileStream(Constants.IO.GameSettings.CurrentSettings, FileMode.Open))
                {
                    newSettings = (GameSettings)serializer.Deserialize(fileStream);
                }
            }
            catch
            {
                ResetGameSettings();
                newSettings = LoadGameSettings();
            }
            newSettings.HasChanges = true;
            return newSettings;
        }

        public static void SaveGameSettings(ref GameSettings gameSettings)
        {
            Directory.CreateDirectory(Constants.IO.GameSettings.SettingsDirectory);
            XmlSerializer serializer = new XmlSerializer(gameSettings.GetType());
            using (StreamWriter streamWriter = new StreamWriter(Constants.IO.GameSettings.CurrentSettings))
            {
                serializer.Serialize(streamWriter, gameSettings);
            }
            gameSettings.HasChanges = true;
        }

        public static void ResetGameSettings()
        {
            try
            {
                Directory.CreateDirectory(Constants.IO.GameSettings.SettingsDirectory);
                if (!File.Exists(Constants.IO.GameSettings.DefaultGameSettings))
                {
                    CreateDefaultSettingsFile();
                }
                File.Copy(Constants.IO.GameSettings.DefaultGameSettings, Constants.IO.GameSettings.CurrentSettings, true);
            }
            catch
            {
                CreateDefaultSettingsFile();
                ResetGameSettings();
            }
        }

        public static void CreateDefaultSettingsFile()
        {
            Directory.CreateDirectory(Constants.IO.GameSettings.SettingsDirectory);
            GameSettings gameSettings = new GameSettings()
            {
                Borderless = false,
                HasChanges = false,
                Resolution = new Vector2(800, 600),
                Vsync = true
            };
            XmlSerializer serializer = new XmlSerializer(gameSettings.GetType());
            using (StreamWriter streamWriter = new StreamWriter(Constants.IO.GameSettings.DefaultGameSettings))
            {
                serializer.Serialize(streamWriter, gameSettings);
            }
        }
    }
}
