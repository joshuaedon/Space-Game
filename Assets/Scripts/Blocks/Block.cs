using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour {
    public BlockType type;

    public void load(BlockData blockData) {
    	this.transform.localEulerAngles = new Vector3(0, 0, blockData.rotation);
    }

    public void setBlueprint(bool b) {
        foreach(Collider2D c in GetComponents<Collider2D>())
            c.isTrigger = b;

    	if(b)
    		GetComponent<Renderer>().material = (Material)Resources.Load("Materials/Blueprint");
    	else
    		GetComponent<Renderer>().material = (Material)Resources.Load("Materials/Default");
    }
}

[System.Serializable]
public class BlockData {
	public int typeID;
	public float rotation;

	public BlockData(Block block) {
		this.typeID = block.type.id;

		this.rotation = block.transform.localEulerAngles.z;
	}
}