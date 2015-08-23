using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertPanelManager : MonoBehaviour {

	public Text errorText;

	void Start () {	
		ClosePanel ();
	}

	public void ClosePanel() {
		gameObject.SetActive (false);
	}

	public void OpenPanel() {
		gameObject.SetActive (true);
	}

	public void ShowMessage(string message) {
		errorText.text = message;
		OpenPanel ();
	}


}
