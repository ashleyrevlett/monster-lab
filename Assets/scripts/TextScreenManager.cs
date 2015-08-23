using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;



namespace Completed	{
	
	public class TextScreenManager : MonoBehaviour {

		public string nextSceneName;
		public string[] statements;
		public Text textBox; 
		public float secondsPerStatement = 5f;
		private List<string> statementsToShow;

		// Use this for initialization
		void Start () {

			statementsToShow = new List<string> (statements);

			if (statementsToShow.Count > 0) {
				StartCoroutine (ShowStatements ());
			}
				
		}

		IEnumerator ShowStatements() {
			while (statementsToShow.Count > 0) {		
				string t = statementsToShow[0];
				statementsToShow.RemoveAt (0);
				textBox.text = t;
				yield return new WaitForSeconds(secondsPerStatement);			
			}
		}


		public void StartGame() {
			Application.LoadLevel(nextSceneName);
		}



	}

}