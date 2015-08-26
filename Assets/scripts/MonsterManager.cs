using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Completed
	
{

	public class MonsterManager : MonoBehaviour {

		public Sprite deadSprite;

		public string monsterName = "unnamed";
		private float damage = 0f;
		private float hunger = 0f;
		private float thirst = 0f;
		public bool isAlive = true;

		public float hungerDamage = 2f;
		public float thirstDamage = 5f;
		public float healRate = .1f;


		public Vector2 appetiteMinMax = new Vector2(.3f, 3f);
		private float appetite = 0;

		public Canvas statsCanvas;
		public Text nameText;
		public Text healthText;
		public Text thirstText;
		public Text hungerText;

		private RectTransform statsRT;

		public string[] firstNames;
		public string[] lastNames;
		public string[] possibleFacts; // input in editor

		private List<string> unknownFacts; // converted to list for ease
		private List<string> knownFacts;

		private BoardManager boardManager;
		public CageManager cageManager; // hook up from bm

		private NotificationsManager notifManager;



		// Use this for initialization
		void Start () {

			knownFacts = new List<string> ();
			unknownFacts = new List<string> (possibleFacts);

			boardManager = GameObject.Find ("GameManager").GetComponent<BoardManager> ();
			notifManager = GameObject.Find ("Notifications").GetComponent<NotificationsManager> ();

			statsRT = statsCanvas.GetComponent<RectTransform> ();

			int randomFirstIdx = Random.Range (0, firstNames.Length);
			int randomLastIdx = Random.Range (0, lastNames.Length);

			monsterName = firstNames [randomFirstIdx] + lastNames [randomLastIdx];

			appetite = Random.Range (appetiteMinMax.x, appetiteMinMax.y);

			InvokeRepeating ("Tick", 0f, 5f);

		}

		
		// Update is called once per frame
		void Update () {

			// position labels
			Vector3 pos = Camera.main.WorldToScreenPoint(cageManager.gameObject.transform.position);	
			statsRT.position = new Vector2(pos.x + 30f, pos.y - 70f);
				
			// don't update text if dead
			if (!isAlive)
				return;

			// die if damaged
			if (damage >= 100f)
				Die ();

			nameText.text = monsterName;
			healthText.text = "DAMAGE: " + damage.ToString ("F0") + "%";
			thirstText.text = "THIRST: " + thirst.ToString ("F0") + "%";
			hungerText.text = "HUNGER: " + hunger.ToString ("F0") + "%";


		}

		void Die() {
			isAlive = false;
			CancelInvoke("Tick");

			gameObject.GetComponent<SpriteRenderer> ().sprite = deadSprite;
			healthText.text = "DEAD";
			thirstText.text = "";
			hungerText.text = "";

			string msg = string.Format("{0} has DIED!", monsterName);
			notifManager.ShowNotice (msg);

		}

		void Tick() {	

			if (isAlive) {

				Debug.Log ("Thirst 1: " + thirst);
				hunger = Mathf.Min (100f, hunger + appetite);
				thirst = Mathf.Min (100f, thirst + appetite * 7f);

				if (hunger >= 100f) 
					damage += (Time.deltaTime * hungerDamage);
				
				if (thirst >= 100f)
					damage += (Time.deltaTime * thirstDamage);
				
				damage = Mathf.Max(0, damage - healRate);
							
				Debug.Log ("Thirst 2: " + thirst);
			}
		}

		public void DiscoverFact() { 
		
			if (unknownFacts.Count <= 0) {
				boardManager.alertManager.ShowMessage ("We've discovered everything possible about this creature with the current technology.");
				return;
			}

			int randomIdx = Random.Range (0, unknownFacts.Count);
			string fact = unknownFacts [randomIdx];
			unknownFacts.RemoveAt (randomIdx);
			knownFacts.Add (fact);

			string msg = "You're discovered something new about " + monsterName + "!\n";
			msg += " " + fact;
			boardManager.alertManager.ShowMessage (msg);

		}


		public IEnumerator DealDamage(float amount, int numberTimes, float delayTime ) {		
			for (int i = 0; i < numberTimes; i++) {					
				damage = Mathf.Min (100f, damage + amount);	
				string msg = string.Format("Damaged {0}! Health Left: {1}", monsterName, (100f - damage).ToString("F2"));
				notifManager.ShowNotice (msg);
				yield return new WaitForSeconds (delayTime);	
			}

			DiscoverFact (); // show new fact notice
			EndExperiment (); // move monster back to cage

		}

		
		public void EndExperiment() {
			
			// find monster's home cage 
			CageManager cage = boardManager.GetCages().Find (x=> x.monster == gameObject);
			TableManager table = boardManager.GetTables().Find (x=> x.monster == gameObject);
			
			Vector3 newPos = new Vector3 (cage.gameObject.transform.position.x, cage.gameObject.transform.position.y, -.05f);
			gameObject.transform.position = newPos;
			
			table.monster = null;

			notifManager.ShowNotice ("Experiment Complete!");


		}

		public void Water() {
			thirst = 0f;
			Debug.Log ("Watered " + monsterName + ", thirst: " + thirst);
		}

		public void Feed() {
			hunger = 0f;
			Debug.Log ("Fed " + monsterName + ", hunger: " + hunger);
		}


		// mouse click on monster -> show action panel	
		void OnMouseDown() {

			if (!isAlive) 
				return;

			// don't respond if alert is already visible
			if (boardManager.alertManager.gameObject.activeInHierarchy == true)
				return;

			// don't respond if action panel is already visible
			if (boardManager.actionPanel.gameObject.activeInHierarchy == true)
				return;
					
			// show action panel and set current monster to this one
			boardManager.ShowActionPanel ( cageManager.gameObject.transform.position );
			boardManager.SetMonsterFocus ( this);

		}

	}




}