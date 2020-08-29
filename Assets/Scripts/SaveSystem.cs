using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem {
    public static void SaveGame(string saveName) {
    	BinaryFormatter formatter = new BinaryFormatter();

    	if(!Directory.Exists(Application.persistentDataPath + "/saves"))
    		Directory.CreateDirectory(Application.persistentDataPath + "/saves");

    	string path = Application.persistentDataPath + "/saves/" + saveName + ".save";
    	FileStream stream = new FileStream(path, FileMode.Create);

    	GameData data = new GameData();

    	formatter.Serialize(stream, data);
    	stream.Close();
    }

    public static GameData LoadGame(string saveName) {
    	string path = Application.persistentDataPath + "/saves/" + saveName + ".save";

    	if(File.Exists(path)) {
    		BinaryFormatter formatter = new BinaryFormatter();
    		FileStream stream = new FileStream(path, FileMode.Open);

    		GameData data = null;
    		try {
    			data = formatter.Deserialize(stream) as GameData;
    		} catch(Exception e) {Debug.Log(e);}
    		stream.Close();

    		return data;
		} else {
			Debug.Log("Save '" + saveName + "'' not found in " + Application.persistentDataPath + "/saves/");
			return null;
		}
    }
}
