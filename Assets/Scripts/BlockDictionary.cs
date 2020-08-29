using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockDictionary : MonoBehaviour {
	public Controller controller;

    public BlockType[] blockTypes;

    public void OnValidate() {
    	for(int i = 0; i < blockTypes.Length; i++) {
    		if(blockTypes[i] != null)
	    		blockTypes[i].id = i;
    	}
    }

    public void OnEnable() {
    	foreach(Transform child in transform)
		     Destroy(child.gameObject);

    	for(int i = 0; i < blockTypes.Length; i++) {
    		if(blockTypes[i] != null) {
				GameObject blockButton = (GameObject)Instantiate(Resources.Load("prefabs/UI/BlockButton"), transform);
				BlockType temp = blockTypes[i];
				blockButton.GetComponent<Image>().sprite = temp.sprite;
				blockButton.GetComponent<Button>().onClick.AddListener(() => {
					controller.selectBlock(temp.prefab);
				});
			}
    	}
    }
}
