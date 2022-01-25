using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using UnityEngine.Assertions;

namespace pf
{
    public static class SaveSystem
    {
        const string SAVEFILE = "save.bin";

        private static SaveData loadedData = null;

        public static void Save()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/" + SAVEFILE;

            FileStream stream = new FileStream(path, FileMode.Create);

            try
            {
                SaveData data = CompareSaves(loadedData, new SaveData());
                formatter.Serialize(stream, data);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                stream.Close();
            }
        }

        private static SaveData CompareSaves(SaveData oldData, SaveData newData)
        {
            // Sanity checks if older levels are played again so the progress won't disappear
            if(oldData != null && oldData.currentLevel > newData.currentLevel)
            {
                newData.currentLevel = oldData.currentLevel;
                newData.selectedCharacter = oldData.selectedCharacter;
                newData.health = oldData.health;
            }

            // Shouldn't never trigger but let's put it in just in case...
            Assert.IsTrue(newData.currentLevel == -1 || newData.currentLevel >= (int)LevelLoader.Scenes.StartLevel);

            return newData;
        }

        public static SaveData Load()
        {
            string path = Application.persistentDataPath + "/" + SAVEFILE;
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                loadedData = null;

                try
                {
                    loadedData = formatter.Deserialize(stream) as SaveData;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
                finally
                {
                    stream.Close();
                }

                return loadedData;
            }
            else
            {
                Debug.LogWarning("Save file not found in " + path);
                return null;
            }
        }

        public static void Reset()
        {
            string path = Application.persistentDataPath + "/" + SAVEFILE;
            try
            {
                File.Delete(path);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}
