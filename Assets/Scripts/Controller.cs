using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour {
	public GameObject DebugText;

	public static BlockDictionary blockDict;

	public static List<Structure> structures;

	public static GameObject selectedBlock;
	private Structure overStructure;
	private Vector2Int structIndex;
	private float delay;

	void Start() {
		blockDict = GameObject.Find("BlocksPanel").GetComponent<BlockDictionary>();

		GameData data = SaveSystem.LoadGame("save");

		structures = new List<Structure>();
		if(data != null) {
			GameObject structureObjRef = (GameObject)Instantiate(Resources.Load("prefabs/Structure"));
			foreach(StructureData structureData in data.structures) {
				GameObject structureObj = (GameObject)Instantiate(structureObjRef, transform);
	        	Structure structure = structureObj.GetComponent<Structure>();
	        	structure.load(structureData);
	        	structures.Add(structure);
			}
			Destroy(structureObjRef);
		}
	}	

    void Update() {
    	// Debug.Log(this.delay);
    	this.delay = Mathf.Max(0f, this.delay - Time.deltaTime * 20f);

    	// Toggle debug text
        if(Input.GetKeyDown(KeyCode.F2))
            DebugText.SetActive(!DebugText.activeInHierarchy);

        // Placing blocks
        if(selectedBlock != null) {
        	Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        	// Find a structure that a block can be added to
        	if(this.overStructure != null) {
        		Vector2Int relIndex = this.overStructure.getRelativeIndex(pos);
    			// If mouse outside structure's block array indices or not adjacent to an existing block
    			if(relIndex.x < 0 || relIndex.y < 0 || !this.overStructure.canPlace(relIndex)) {
    				this.overStructure = null;
    				selectedBlock.transform.SetParent(transform);
    			} else {
    				this.structIndex = relIndex;
    			}
        	}
        	if(this.overStructure == null) {
        		// Check if the mouse is adjacent to an exisiting structure which can be added to
        		foreach(Structure structure in structures) {
        			Vector2Int relIndex = structure.getRelativeIndex(pos);
        			// If mouse within structure's block array's indices
        			if(relIndex.x >= 0 && relIndex.y >= 0 && structure.canPlace(relIndex)) {
        				this.overStructure = structure;
        				this.structIndex = relIndex;
        			}
        		}
        	}

			// Rotate block
			if(overStructure == null) {
				if(Input.GetKey(KeyCode.Q))
	            	selectedBlock.transform.Rotate(0f, 0f, Settings.rotationSpeed * Time.deltaTime, Space.Self);
	            if(Input.GetKey(KeyCode.E))
	            	selectedBlock.transform.Rotate(0f, 0f, -Settings.rotationSpeed * Time.deltaTime, Space.Self);
            } else {
            	if(Input.GetKeyDown(KeyCode.Q))
	            	selectedBlock.transform.Rotate(0f, 0f, 90f, Space.Self);
	            if(Input.GetKeyDown(KeyCode.E))
	            	selectedBlock.transform.Rotate(0f, 0f, -90f, Space.Self);
            }

        	// Set block blueprint position
        	if(this.overStructure == null) {
				selectedBlock.transform.position = new Vector3(pos.x, pos.y, 0);
			} else {
				selectedBlock.transform.SetParent(this.overStructure.transform);
				Vector2Int localPos = this.structIndex - this.overStructure.offset;
				selectedBlock.transform.localEulerAngles = new Vector3(0, 0, Mathf.Round(selectedBlock.transform.localEulerAngles.z / 90f) * 90f);
				selectedBlock.transform.localPosition = new Vector3(localPos.x, localPos.y, 0f);
				// Debug.Break();
			}

            // Tint red if colliding with existing structures
            if(selectedBlock.GetComponent<BoxCollider2D>().IsTouchingLayers())
            	selectedBlock.GetComponent<Renderer>().material.SetColor("Tint", Color.red);
            else {
            	if(overStructure == null)
            		selectedBlock.GetComponent<Renderer>().material.SetColor("Tint", Color.white);
            	else
            		selectedBlock.GetComponent<Renderer>().material.SetColor("Tint", Color.green);

				if(Input.GetMouseButton(0) && this.delay == 0f && !IsPointerOverUIObject()) {
					this.delay = 1f;

					// Create new single-block strucute
					if(this.overStructure == null) {
		        		GameObject structureObj = (GameObject)Instantiate(Resources.Load("prefabs/Structure"), transform);
		        		structureObj.transform.position = selectedBlock.transform.position;
		        		structureObj.transform.eulerAngles = selectedBlock.transform.eulerAngles;
		        		this.overStructure = structureObj.GetComponent<Structure>();
		        		structures.Add(this.overStructure);
		        		this.structIndex = new Vector2Int(1, 1);
					}

		        	GameObject blockObj = (GameObject)Instantiate(selectedBlock, transform);
			    	Block block = blockObj.GetComponent<Block>();
					block.setBlueprint(false);
			    	this.overStructure.addBlock(block, this.structIndex.x, this.structIndex.y);
			    	block.transform.eulerAngles = selectedBlock.transform.eulerAngles;
			    	// Debug.Break();
		        }
		    }
		    // Deselect block
	        if(Input.GetMouseButtonDown(1)) {
	        	Destroy(selectedBlock);
	        	selectedBlock = null;
	        }
        }
    }

    public void selectBlock(GameObject block) {
    	if(selectedBlock != null)
        	Destroy(selectedBlock);
    	selectedBlock = (GameObject)Instantiate(block, transform);
		selectedBlock.GetComponent<Block>().setBlueprint(true);
    }

    public void clearStructures() {
    	foreach(Structure structure in structures)
    		Destroy(structure.gameObject);
		structures = new List<Structure>();
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
	public StructureData[] structures;

	public GameData() {
		this.structures = new StructureData[Controller.structures.Count];
		for(int i = 0; i < this.structures.Length; i++)
			this.structures[i] = new StructureData(Controller.structures[i]);
	}
}