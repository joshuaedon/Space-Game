using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColonistPanel : MonoBehaviour {
    public Image ColonistSprite;
    public Image Highlight;
    private Colonist colonist;

    public void setColonist(Colonist colonist) {
    	this.colonist = colonist;
    	this.ColonistSprite.sprite = colonist.gameObject.GetComponent<SpriteRenderer>().sprite;
    }

    public void setHighlight(bool b) {
    	this.Highlight.enabled = b;
    }

    public void selectColonist() {
    	ColonistManager ColonistManager = GameObject.FindWithTag("ColonistManager").GetComponent<ColonistManager>();
    	ColonistManager.selectColonist(this.colonist);
    }
}
