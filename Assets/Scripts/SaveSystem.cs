using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

public static class SaveSystem {
    public static GameData toLoad;

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

    public static void LoadGame(string saveName) {
		toLoad = null;

    	string path = Application.persistentDataPath + "/saves/" + saveName + ".save";

    	if(File.Exists(path)) {
    		BinaryFormatter formatter = new BinaryFormatter();
    		FileStream stream = new FileStream(path, FileMode.Open);

    		try {
    			toLoad = formatter.Deserialize(stream) as GameData;
    		} catch(Exception e) {Debug.Log(e);}
    		stream.Close();
		} else {
			Debug.Log("Save '" + saveName + "'' not found in " + Application.persistentDataPath + "/saves/");
		}
    }
}
