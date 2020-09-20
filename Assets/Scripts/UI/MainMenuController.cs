using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    public void newGame() {
    	SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void loadGame() {
		SaveSystem.LoadGame("save");

    	SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }
}
