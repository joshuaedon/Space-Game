using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : Object {
    public GameObject CenterOfMass;

    public List<List<Block>> blocks;
    public Vector2Int offset;

    void OnEnable() {
    	this.blocks = new List<List<Block>>();
    	for(int i = 0; i < 3; i++) {
    		this.blocks.Add(new List<Block>());
    		for(int j = 0; j < 3; j++)
    			this.blocks[i].Add(null);
    	}
    	this.offset = new Vector2Int(1, 1);
    }

    public void load(StructureData structureData) {
    	this.rb.position = new Vector2(structureData.pos[0], structureData.pos[1]);
    	this.rb.velocity = new Vector2(structureData.vel[0], structureData.vel[1]);
    	this.rb.rotation = structureData.rotation;
    	this.rb.angularVelocity = structureData.angularVel;
    	this.offset = new Vector2Int(structureData.offset[0], structureData.offset[1]);

    	this.blocks = new List<List<Block>>();
    	for(int i = 0; i < structureData.blocks.GetLength(0); i++) {
    		if(this.blocks.Count == i)
    			this.blocks.Add(new List<Block>());
			for(int j = 0; j < structureData.blocks.GetLength(1); j++) {
				this.blocks[i].Add(null);
				BlockData blockData = structureData.blocks[i,j];
				if(blockData != null) {
					GameObject blockObj = (GameObject)Instantiate(Controller.blockDict.blockTypes[blockData.typeID].prefab, transform);
			    	Block block = blockObj.GetComponent<Block>();
			    	block.load(blockData);
					// block.setBlueprint(false);
			    	addBlock(block, i, j);
				}
			}
		}
    }

    public Vector2Int getRelativeIndex(Vector2 pos) {
    	Vector3 relPos = pos - this.rb.position;

    	if(Mathf.Abs(relPos.x) + Mathf.Abs(relPos.y) > blocks.Count + blocks[0].Count)
    		return new Vector2Int(-1, -1);

    	float delta = -this.rb.rotation * Mathf.Deg2Rad;
    	Vector2Int relIndex = new Vector2Int(
	        Mathf.FloorToInt(relPos.x * Mathf.Cos(delta) - relPos.y * Mathf.Sin(delta) + 0.5f),
	        Mathf.FloorToInt(relPos.x * Mathf.Sin(delta) + relPos.y * Mathf.Cos(delta) + 0.5f)
	    ) + this.offset;

        // Return (-1, -1) if position is out of array bounds
	    if(relIndex.x >= blocks.Count || relIndex.y >= blocks[0].Count)
	    	return new Vector2Int(-1, -1);

	    return relIndex;
    }

    public bool[] adjacentBlocks(Vector2Int relIndex) {
    	// Debug.Log("rel: " + relIndex + " - off: " + offset + ", array: (" + blocks.Count + "," + blocks[0].Count + ")");
        /*check if block can be placed on this side of that block (with rotation in account) (function of that block) &&*/
        if(this.blocks[relIndex.x][relIndex.y] != null)
            return new bool[4];
        else
            return new bool[] {
    		    (relIndex.y + 1 < this.blocks[0].Count && this.blocks[relIndex.x    ][relIndex.y + 1] != null) && isConnection(relIndex.x    , relIndex.y + 1, 2),
    		    (relIndex.x - 1 >= 0                   && this.blocks[relIndex.x - 1][relIndex.y    ] != null) && isConnection(relIndex.x - 1, relIndex.y    , 3),
    		    (relIndex.y - 1 >= 0                   && this.blocks[relIndex.x    ][relIndex.y - 1] != null) && isConnection(relIndex.x    , relIndex.y - 1, 0),
                (relIndex.x + 1 < this.blocks.Count    && this.blocks[relIndex.x + 1][relIndex.y    ] != null) && isConnection(relIndex.x + 1, relIndex.y    , 1)
            };
    }

    private bool isConnection(int x, int y, int dir) {
        Block block = this.blocks[x][y];
        return block.type.connections[Mathf.RoundToInt(block.transform.localEulerAngles.z / 90f - dir + 4) % 4];
    }

    public void addBlock(Block block, int i, int j) {
    	// Debug.Log("before - ij: (" + i + "," + j + "), off: " + offset + ", array: (" + blocks.Count + "," + blocks[0].Count + ")");

    	this.blocks[i][j] = block;
    	// Set new block's relative position and rotation
    	block.transform.SetParent(this.transform);
		block.transform.localPosition = new Vector3(i - this.offset.x, j - this.offset.y, 0);
    	// Set new com and mass
        this.rb.centerOfMass = (this.rb.centerOfMass * this.rb.mass + ((Vector2)block.transform.localPosition) * block.type.mass) / (this.rb.mass + block.type.mass);
        this.CenterOfMass.transform.localPosition = this.rb.centerOfMass;
    	this.rb.mass += block.type.mass;

    	// Expand block lists
    	if(i == 0) {
    		this.blocks.Insert(0, new List<Block>());
    		foreach(Block b in this.blocks[1])
    			this.blocks[0].Add(null);
    		this.offset.x = this.offset.x + 1;
    	} else if(i == this.blocks.Count-1) {
    		this.blocks.Add(new List<Block>());
    		foreach(Block b in this.blocks[0])
    			this.blocks[this.blocks.Count-1].Add(null);	
    	}
    	if(j == 0) {
    		foreach(List<Block> list in this.blocks)
    			list.Insert(0, null);
    		this.offset.y = this.offset.y + 1;
    	} else if(j == this.blocks[0].Count-1) {
    		foreach(List<Block> list in this.blocks)
    			list.Add( null);
    	}
    }
}

[System.Serializable]
public class StructureData {
	public float[] pos;
	public float[] vel;
	public float rotation;
	public float angularVel;
	public BlockData[,] blocks;
	public int[] offset;
	// Save mass and CoM???

	public StructureData(Structure structure) {
		this.pos = new float[2];
		this.pos[0] = structure.rb.position.x;
		this.pos[1] = structure.rb.position.y;
		
		this.vel = new float[2];
		this.vel[0] = structure.rb.velocity.x;
		this.vel[1] = structure.rb.velocity.y;

		this.rotation = structure.rb.rotation;

		this.angularVel = structure.rb.angularVelocity;

		this.blocks = new BlockData[structure.blocks.Count, structure.blocks[0].Count];
		for(int i = 0; i < structure.blocks.Count; i++) {
			for(int j = 0; j < structure.blocks[0].Count; j++) {
				if(structure.blocks[i][j] != null)
					this.blocks[i, j] = new BlockData(structure.blocks[i][j]);
			}
		}

		this.offset = new int[2];
		this.offset[0] = structure.offset.x;
		this.offset[1] = structure.offset.y;
	}
}