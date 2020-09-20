using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugText : MonoBehaviour {
    void Update() {
        GetComponent<UnityEngine.UI.Text>().text = "DeltaTime: " + Time.deltaTime
        						  			 	 + "\nStrucutres: " + Controller.structures.Count; 
    }
}
