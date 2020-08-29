using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Block Type", menuName = "Block Type")]
public class BlockType : ScriptableObject {
	public int id;
	public GameObject prefab;
    public Sprite sprite;
    public float mass;
    // public float strength;
    // public float damage;
}
