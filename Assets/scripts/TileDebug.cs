using UnityEngine;
using System.Collections;

public class TileDebug : MonoBehaviour {

	private float tileHeight;


	// Use this for initialization
	void Start () {
		SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer> ();
		tileHeight = sprite.bounds.max.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		if (Application.isEditor)  // or check the app debug flag
		{
//			Vector3 pos = Camera.main.WorldToViewportPoint(gameObject.transform.position);
			Vector3 pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
			// screen inverts y coord, so...
			pos.y = Screen.height - pos.y;
//			Vector3 pos = gameObject.transform.position;
			Rect screenRect = new Rect(pos.x - (tileHeight), pos.y - (tileHeight/2), 50, 100);
			string debugText = "(" + gameObject.transform.position.x.ToString() + ", " + gameObject.transform.position.y.ToString() + ")";
			GUI.Label(screenRect, debugText);
		}
	}

}
