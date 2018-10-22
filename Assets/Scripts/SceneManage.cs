using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManage : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Scalable() {
        SceneManager.LoadScene("ScalableFormation");
    }
    public void Emergent() {
        SceneManager.LoadScene("SampleScene");
    }
    public void Level2() {
        SceneManager.LoadScene("Level2");
    }

}
