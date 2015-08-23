using UnityEngine;
using System.Collections;

public class ClosePanel : MonoBehaviour {

	public GameObject actionPanel;

	void OnMouseDown() {
		actionPanel.SetActive (false);	
	}
}
