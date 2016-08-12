using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartBehavior : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount > 0 || Input.GetMouseButtonDown(0)) {
			Vector2 pos;
			if (Input.touchCount > 0) {
				pos = Input.GetTouch (0).position;
			} else {
				pos = Input.mousePosition;
			}

			Ray ray = Camera.main.ScreenPointToRay (pos);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				GameObject go = hit.collider.gameObject;
				Debug.Log (go.name);

				if (go.name == "Easy") {
					Invoke ("EasyStart", 1);
				} else if (go.name == "Hard") {
					Invoke ("HardStart", 1);
				}else if (go.name == "Quit") {
					Application.Quit ();
				}
			}
		}
	}

	void EasyStart(){
		SceneManager.LoadScene ("Scene10");
	}
	void HardStart(){
		SceneManager.LoadScene ("Scene20");
	}
}