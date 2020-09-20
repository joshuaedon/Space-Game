using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour {
	// GUI
	public GameObject DebugText;
	// BlockDictionary
	public static BlockDictionary blockDict;
	// Objects
	public ColonistManager ColonistManager;
	public ConstructionManager ConstructionManager;
	public static List<Structure> structures;

	void Start() {
		blockDict = GameObject.Find("BlocksPanel").GetComponent<BlockDictionary>();

		structures = new List<Structure>();

		if(SaveSystem.toLoad != null) {
			load(SaveSystem.toLoad);
			SaveSystem.toLoad = null;
		}

		ColonistManager.addColonist();
	}

	public void load(GameData data) {
		// Load camera
		Camera.main.GetComponent<CameraController>().load(data.cameraData);

		// Load structures
		GameObject structureObjRef = (GameObject)Instantiate(Resources.Load("prefabs/Structure"));
		foreach(StructureData structureData in data.structures) {
			GameObject structureObj = (GameObject)Instantiate(structureObjRef, transform);
        	Structure structure = structureObj.GetComponent<Structure>();
        	structure.load(structureData);
        	structures.Add(structure);
		}
		Destroy(structureObjRef);
	}

    void Update() {
        if(Input.GetKeyDown(KeyCode.F2)) // Toggle debug text
            DebugText.SetActive(!DebugText.activeInHierarchy);
        if(Input.GetKeyDown(KeyCode.F3)) // Take a screenshot
        	takeScreenshot();
        if(Input.GetKeyDown(KeyCode.F5)) // Quicksave
        	SaveSystem.SaveGame("save");
            
        if(ConstructionManager.selectedBlock != null) {
        	ConstructionManager.blockPlacement();
        } else {
        	// If not placing block, select/deselect colonist
    		if(Input.GetMouseButtonDown(0) && !IsPointerOverUIObject()) {
    			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
		        if(hit.transform != null && hit.transform.gameObject.tag == "Colonist")
		        	ColonistManager.selectColonist(hit.transform.gameObject.GetComponent<Colonist>());
		        else if(ColonistManager.colonistSelected())
		        	ColonistManager.deselectColonist();
    		}
        }
    }

    private void takeScreenshot() {
    	if(!Directory.Exists(Application.persistentDataPath + "/screenshots"))
    		Directory.CreateDirectory(Application.persistentDataPath + "/screenshots");

    	string path = Application.persistentDataPath + "/screenshots/Screenshot" + System.DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss.fff") + ".png";
    	ScreenCapture.CaptureScreenshot(path);
    }

    public void clearStructures() {
    	foreach(Structure structure in structures)
    		Destroy(structure.gameObject);
		structures = new List<Structure>();
    }

    public void quitToMainMenu() {
    	SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    void OnApplicationQuit() {
    	SaveSystem.SaveGame("save");
    }

    public static bool IsPointerOverUIObject() {
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		List<RaycastResult> results = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		return results.Count > 0;
	}
}

[System.Serializable]
public class GameData {
	public CameraData cameraData;
	public StructureData[] structures;

	public GameData() {
		this.cameraData = new CameraData(Camera.main.GetComponent<CameraController>());

		this.structures = new StructureData[Controller.structures.Count];
		for(int i = 0; i < this.structures.Length; i++)
			this.structures[i] = new StructureData(Controller.structures[i]);
	}
}