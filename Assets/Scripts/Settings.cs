using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {
    public static SettingsProfile s;

    void OnEnable() {
    	s = (SettingsProfile)Resources.Load("Settings/Default");
    }
}
