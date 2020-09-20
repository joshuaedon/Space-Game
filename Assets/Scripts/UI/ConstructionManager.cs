using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour {
	public GameObject DeselectBlockButton;
    public GameObject Structures;

	public static GameObject selectedBlock;
	private Structure overStructure;
	private Vector2Int structIndex; // Index of overStructure where mouse is
	private bool[] adjacentBlocks;
	private float delay;

    void Update() {
    	this.delay = Mathf.Max(0f, this.delay - Time.deltaTime * 20f);
    }

    public void blockPlacement() {
    	// Placing block
    	Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    	// Find a structure that a block can be added to
    	if(this.overStructure != null) {
    		// Check if it can still be added to the structure it was over last frame
    		Vector2Int relIndex = this.overStructure.getRelativeIndex(pos);
			// If mouse inside structure's block array indices and adjacent to an existing block
			if(relIndex.x >= 0 && relIndex.y >= 0 && canPlace(this.overStructure, relIndex)) {
				this.structIndex = relIndex;
			} else {
				this.overStructure = null;
				selectedBlock.transform.SetParent(transform);
			}
    	}
    	if(this.overStructure == null) {
    		// Check if the mouse is adjacent to an exisiting structure which can be added to
    		foreach(Structure structure in Controller.structures) {
    			Vector2Int relIndex = structure.getRelativeIndex(pos);
    			// If mouse within structure's block array's indices
    			if(relIndex.x >= 0 && relIndex.y >= 0 && canPlace(structure, relIndex)) {
    				this.overStructure = structure;
    				this.structIndex = relIndex;
    			}
    		}
    	}

		rotateBlueprint();

    	// Set block blueprint position
    	if(this.overStructure == null) {
			selectedBlock.transform.position = new Vector3(pos.x, pos.y, 0);
		} else {
			selectedBlock.transform.SetParent(this.overStructure.transform);
			Vector2Int localPos = this.structIndex - this.overStructure.offset;
			selectedBlock.transform.localEulerAngles = new Vector3(0, 0, Mathf.Round(selectedBlock.transform.localEulerAngles.z / 90f) * 90f);
			selectedBlock.transform.localPosition = new Vector3(localPos.x, localPos.y, 0f);
		}

        // Tint red if colliding with existing structures
        if(selectedBlock.GetComponent<BoxCollider2D>().IsTouchingLayers())
        	selectedBlock.GetComponent<Renderer>().material.SetColor("Tint", Color.red);
        else {
        	// Tint green if it is being added to an existing structure
        	if(overStructure == null)
        		selectedBlock.GetComponent<Renderer>().material.SetColor("Tint", Color.white);
        	else
        		selectedBlock.GetComponent<Renderer>().material.SetColor("Tint", Color.green);
        	// Place the block
			if(Input.GetMouseButton(0) && this.delay == 0f && !Controller.IsPointerOverUIObject())
				placeBlock();
	    }

	    // Deselect block
        if(Input.GetMouseButtonDown(1))
        	deselectBlock();
    }

    public void selectBlock(GameObject block) {
    	if(selectedBlock != null)
        	Destroy(selectedBlock);
    	selectedBlock = (GameObject)Instantiate(block, transform);
		selectedBlock.GetComponent<Block>().setBlueprint(true);

		this.DeselectBlockButton.SetActive(true);

		if(ColonistManager.selectedColonist != null)
			ColonistManager.selectedColonist.turnOffThrust();
    }

    private bool canPlace(Structure structure, Vector2Int relIndex) {
    	this.adjacentBlocks = structure.adjacentBlocks(relIndex);
    	foreach(bool b in this.adjacentBlocks) {
    		if(b)
    			return true;
    	}
    	return false;
    }

    private void rotateBlueprint() {
        if(overStructure == null) {
            if(Input.GetKey(KeyCode.Q))
                selectedBlock.transform.Rotate(0f, 0f, Settings.s.rotationSpeed * Time.deltaTime, Space.Self);
            if(Input.GetKey(KeyCode.E))
                selectedBlock.transform.Rotate(0f, 0f, -Settings.s.rotationSpeed * Time.deltaTime, Space.Self);
        } else {
            float dir = 90f;
            if(Input.GetKeyDown(KeyCode.Q))
                selectedBlock.transform.Rotate(0f, 0f, 90f, Space.Self);
            if(Input.GetKeyDown(KeyCode.E)) {
                selectedBlock.transform.Rotate(0f, 0f, -90f, Space.Self);
                dir = -90f;
            }

            // Ensure block is rotated to connect to structure
            Block block = selectedBlock.GetComponent<Block>();
            for(int k = 0; k < 4; k++) {
                for(int i = 0; i < adjacentBlocks.Length; i++) {
                    // If one of the connection sides is adjacent to an availavle block in the structure, return (don't rotate)
                    if(block.type.connections[Mathf.RoundToInt(block.transform.localEulerAngles.z / 90f - i + 4) % 4] && adjacentBlocks[i])
                        return;
                }
                selectedBlock.transform.Rotate(0f, 0f, dir, Space.Self);
            }
        }
    }

    private void placeBlock() {
    	this.delay = 1f;
		// Create new strucute object
		if(this.overStructure == null) {
    		GameObject structureObj = (GameObject)Instantiate(Resources.Load("prefabs/Structure"), Structures.transform);
    		structureObj.transform.position = selectedBlock.transform.position;
    		structureObj.transform.eulerAngles = selectedBlock.transform.eulerAngles;
    		this.overStructure = structureObj.GetComponent<Structure>();
    		Controller.structures.Add(this.overStructure);
    		this.structIndex = new Vector2Int(1, 1);

    		// Object.attractors.Add(this.overStructure);
		}
		// Create new block object and add it as a child to its structure object
    	GameObject blockObj = (GameObject)Instantiate(selectedBlock, transform);
    	Block block = blockObj.GetComponent<Block>();
		block.setBlueprint(false);
    	this.overStructure.addBlock(block, this.structIndex.x, this.structIndex.y);
    	block.transform.eulerAngles = selectedBlock.transform.eulerAngles;
    }

    public void deselectBlock() {
    	Destroy(selectedBlock);
    	selectedBlock = null;

		this.DeselectBlockButton.SetActive(false);
    }

}
