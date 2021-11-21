using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem
{
    const string SAVEFILE = "save.bin";

    public static void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + SAVEFILE;

        FileStream stream = new FileStream(path, FileMode.Create);

        try
        {
            SaveData data = new SaveData();
            formatter.Serialize(stream, data);
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            stream.Close();
        }
    }

    public static SaveData Load()
    {
        string path = Application.persistentDataPath + "/" + SAVEFILE;
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = null;

            try
            {
                data = formatter.Deserialize(stream) as SaveData;    
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                stream.Close();
            }

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
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
