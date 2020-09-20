using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColonistManager : MonoBehaviour {
	public Image SelectedSprite;
	public GameObject ColonistsPanel;
	public GameObject ColonistInfoPanel;
	public Toggle RCSToggle;
	public GameObject Colonists;

	public static Colonist selectedColonist;
	public List<Colonist> colonists;

	void Start() {
		this.colonists = new List<Colonist>();		
	}

    void Update() {
    	if(selectedColonist != null && ConstructionManager.selectedBlock == null) {
    		// Control selected colonist
    		selectedColonist.control();

    		if(Input.GetKeyDown(KeyCode.R))
				RCSToggle.isOn = !RCSToggle.isOn;
    	}
    }

	public void addColonist() {
		GameObject colonistObj = (GameObject)Instantiate(Resources.Load("prefabs/Colonist"), Colonists.transform);
		Colonist colonist = colonistObj.GetComponent<Colonist>();

		this.colonists.Add(colonist);

		ColonistPanel colonistPanel = ((GameObject)Instantiate(Resources.Load("prefabs/UI/ColonistPanel"), ColonistsPanel.transform)).GetComponent<ColonistPanel>();
		colonistPanel.setColonist(colonist);
		colonist.colonistPanel = colonistPanel;
	}

	public void selectColonist(Colonist colonist) {
		if(selectedColonist != null)
			selectedColonist.colonistPanel.setHighlight(false);
		colonist.colonistPanel.setHighlight(true);

		selectedColonist = colonist;

		// Set the sprite in the ViewSelectedPanel
    	this.SelectedSprite.enabled = true;
    	this.SelectedSprite.sprite = colonist.gameObject.GetComponent<SpriteRenderer>().sprite;
    	// Show info panel
    	this.ColonistInfoPanel.SetActive(true);

		// Follow the selected colonist with the camera
    	Camera.main.GetComponent<CameraController>().follow(selectedColonist);
	}

	public void deselectColonist() {
		selectedColonist.colonistPanel.setHighlight(false);

		selectedColonist.turnOffThrust();
    	selectedColonist = null;

    	this.SelectedSprite.enabled = false;
    	// Hide info panel
    	this.ColonistInfoPanel.SetActive(false);

    	// Unfollow the selected colonist with the camera
    	Camera.main.GetComponent<CameraController>().unfollow();
	}

	public bool colonistSelected() {
		return selectedColonist != null;
	}

	public void setRCS() {
		selectedColonist.setRCS(RCSToggle.isOn);
	}
}
