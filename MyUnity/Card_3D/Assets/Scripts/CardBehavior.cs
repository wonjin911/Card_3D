using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class CardBehavior : MonoBehaviour {

	public Text TurnText;
	public Text VictoryText;

	private bool ready = false;
	private int turn = 0;
	private int hint = 0;
	private int cardNum = 0;
	private GameObject firstCard;
	private GameObject secondCard;

	void Start () {
		
		ready = false;
		GameObject[] gameobjects = GameObject.FindGameObjectsWithTag ("Card");
		cardNum = gameobjects.Length;
			
		ShuffleCard (gameobjects);
		AddCardMesh (gameobjects);
		foreach (GameObject go in gameobjects){
			AddCardAnimation (go);
		}

		hint = 2;
		Invoke ("TurnAllOff", 2f);

	}

	void TurnAllOn(){
		ready = false;
		GameObject[] gameobjects = GameObject.FindGameObjectsWithTag ("Card");
		foreach (GameObject go in gameobjects) {
			go.transform.Rotate (new Vector3 (180, 0, 0));
		}
	}
	void TurnAllOff(){
		GameObject[] gameobjects = GameObject.FindGameObjectsWithTag ("Card");
		foreach (GameObject go in gameobjects) {
			go.transform.Rotate (new Vector3 (180, 0, 0));
		}
		ready = true;
	}

	void ShuffleCard(GameObject[] gameobjects){
		GameObject[] tmp = new GameObject[gameobjects.Length];
		for (int i = 0; i < gameobjects.Length; i++) {
			tmp [0] = gameobjects [i];
			int r = Random.Range (i, gameobjects.Length);
			gameobjects [i] = gameobjects [r];
			gameobjects [r] = tmp [0];
		}
	}

	void ShuffleMesh(Mesh[] meshes){
		Mesh[] tmp = new Mesh[meshes.Length];
		for (int i = 0; i < meshes.Length; i++) {
			tmp [0] = meshes [i];
			int r = Random.Range (0, meshes.Length);
			meshes [i] = meshes [r];
			meshes [r] = tmp [0];
		}
	}

	void AddCardMesh(GameObject[] gameobjects){
		string path = "Free_Playing_Cards";
		Mesh[] meshes = Resources.LoadAll<Mesh>(path);

		ShuffleMesh(meshes);
		for (int i = 0; i < gameobjects.Length/2; i++){
			MeshFilter mFilter = gameobjects[i].GetComponent<MeshFilter> ();
			mFilter.mesh = meshes[i];
			MeshFilter mFilter2 = gameobjects[i+gameobjects.Length/2].GetComponent<MeshFilter> ();
			mFilter2.mesh = meshes[i];
		}
	}

	void AddCardAnimation(GameObject gameobject){
		Animation anim = gameobject.GetComponent<Animation>();
		AnimationCurve curve_x;
		AnimationCurve curve_y;
		AnimationCurve curve_z;

		AnimationClip clip = new AnimationClip ();
		clip.legacy = true;

		Vector3 pos = gameobject.transform.localPosition;

		Keyframe[] keys;
		keys = new Keyframe[3];
		keys [0] = new Keyframe (0, pos.y);
		keys [1] = new Keyframe (0.5f, pos.y+5);
		keys [2] = new Keyframe (1, pos.y);
		curve_y = new AnimationCurve (keys);
		curve_x = AnimationCurve.Linear(0,pos.x,1,pos.x);
		curve_z = AnimationCurve.Linear(0,pos.z,1,pos.z);

		clip.SetCurve ("", typeof(Transform), "localPosition.x", curve_x);
		clip.SetCurve ("", typeof(Transform), "localPosition.y", curve_y);
		clip.SetCurve ("", typeof(Transform), "localPosition.z", curve_z);

		anim.AddClip (clip, "Card Turn");
		Debug.Log ("AddAnimation " + gameobject.name);
	}

	// Update is called once per frame
	void Update () {
		TurnText.text = turn.ToString () + " Turns";

		if (ready && !(firstCard && secondCard)
			&& (Input.touchCount > 0 || Input.GetMouseButtonDown(0))) {

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

				if (go.tag == "Card") {
					if (!firstCard) {
						firstCard = go;
						CardTurn (go);
					} else if (firstCard.name != go.name) {
						secondCard = go;

						turn = turn + 1;
						CardTurn (go);
					}
				} else if ((!firstCard && !secondCard) && go.name == "Hint") {
					hint--;
					if (hint == 0) {
						Destroy (go);
					}
					TurnAllOn ();
					Invoke ("TurnAllOff", 2f);
				} else if (go.name == "Return") {
					SceneReset ();
				}
			}
		}
	}

	void CardTurn(GameObject go){
		Animation animation = go.GetComponent<Animation> ();
		animation.Play ("Card Turn");	
		go.transform.Rotate (new Vector3 (180, 0, 0));
		Invoke("CardCheck", 1f);
	}

	void CardCheck(){
		if (firstCard && secondCard) {
			if (firstCard.GetComponent<MeshFilter> ().mesh.name
				== secondCard.GetComponent<MeshFilter> ().mesh.name) {
				Debug.Log (firstCard.name);
				Debug.Log (secondCard.name);
				Debug.Log (firstCard.GetComponent<MeshFilter> ().mesh.name);
				Destroy (firstCard);
				cardNum--;
				Destroy (secondCard);
				cardNum--;

				if (cardNum <= 0) {
					VictoryText.text = "You won by " + turn.ToString () + " Turns!";
					Invoke ("SceneReset", 2f);
				}
			} else {
				firstCard.transform.Rotate (new Vector3 (180, 0, 0));
				secondCard.transform.Rotate (new Vector3 (180, 0, 0));
			}

			firstCard = null;
			secondCard = null;
		}
	}

	void SceneReset(){
		SceneManager.LoadScene ("SceneStart");
	}
}
