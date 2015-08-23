using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Completed
	
{

	public class MonsterManager : MonoBehaviour {

		public Sprite deadSprite;

		public string monsterName = "unnamed";
		public float damage = 0f;
		public float hunger = 0f;
		public float thirst = 0f;
		public bool isAlive = true;

		public float hungerDamage = 2f;
		public float thirstDamage = 5f;
		public float healRate = .1f;


		public Vector2 appetiteMinMax = new Vector2(.1f, 2f);
		private float appetite = 0;

		public Canvas statsCanvas;
		public Text nameText;
		public Text healthText;
		public Text thirstText;
		public Text hungerText;

		private RectTransform statsRT;

		public string[] firstNames;
		public string[] lastNames;

		private ResourcesManager resources;
		private BoardManager boardManager;
		public CageManager cageManager; // hook up from bm

		private NotificationsManager notifManager;



		// Use this for initialization
		void Start () {

			resources = GameObject.Find ("GameManager").GetComponent<ResourcesManager> ();
			boardManager = GameObject.Find ("GameManager").GetComponent<BoardManager> ();
			notifManager = GameObject.Find ("Notifications").GetComponent<NotificationsManager> ();

			statsRT = statsCanvas.GetComponent<RectTransform> ();

			int randomFirstIdx = Random.Range (0, firstNames.Length);
			int randomLastIdx = Random.Range (0, lastNames.Length);

			monsterName = firstNames [randomFirstIdx] + lastNames [randomLastIdx];

			appetite = Random.Range (appetiteMinMax.x, appetiteMinMax.y);

			InvokeRepeating ("Tick", 5f, 5f);

		}

		
		// Update is called once per frame
		void Update () {

			if (cageManager == null) 
				return;
			
			// position labels
			Vector3 pos = Camera.main.WorldToScreenPoint(cageManager.gameObject.transform.position);	
			statsRT.position = new Vector2(pos.x + 30f, pos.y - 70f);
				

			if (!isAlive)
				return;

			nameText.text = monsterName;
			healthText.text = "DAMAGE: " + damage.ToString ("F1") + "%";
			thirstText.text = "THIRST: " + thirst.ToString ("F1") + "%";
			hungerText.text = "HUNGER: " + hunger.ToString ("F1") + "%";


			if (damage >= 100f)
				Die ();

			if (hunger >= 100f) {
				damage -= (Time.deltaTime * hungerDamage);
			}

			if (thirst >= 100f)
				damage -= (Time.deltaTime * thirstDamage);
		

		}

		void Die() {
			isAlive = false;
			CancelInvoke("Tick");

			gameObject.GetComponent<SpriteRenderer> ().sprite = deadSprite;
			healthText.text = "DAMAGE: DEAD";
			thirstText.text = "";
			hungerText.text = "";

			string msg = string.Format("{0} has DIED!", monsterName);
			notifManager.ShowNotice (msg);



		}

		void Tick() {	
			if (isAlive) {
				hunger += appetite;
				thirst += appetite * 1.5f;
				damage = Mathf.Max(0, damage - healRate);
			}
		}

		public void Feed() {
			hunger = 0f;	
			resources.food = (int) Mathf.Max (0f, resources.food - 1f);		
			string msg = string.Format("Fed {0}. Food Remaining: {1}", monsterName, resources.food);
			notifManager.ShowNotice (msg);
			Debug.Log ("Fed");
		}

		public void Water() {
			thirst = 0f;	
			resources.water = (int) Mathf.Max (0f, resources.water - 1f);
			string msg = string.Format("Watered {0}. Water Remaining: {1}", monsterName, resources.water);
			notifManager.ShowNotice (msg);
			Debug.Log ("Watered");

		}

		public IEnumerator DealDamage(float amount, int numberTimes, float delayTime ) {		
			for (int i = 0; i < numberTimes; i++) {					
				damage = Mathf.Min (100f, damage + amount);	
				string msg = string.Format("Damaged {0}! Health Left: {1}", monsterName, (100f - damage).ToString("F2"));
				notifManager.ShowNotice (msg);
				yield return new WaitForSeconds (delayTime);	
			}
			boardManager.EndExperiment (this);

		}

		// mouse click on monster -> show action panel	
		void OnMouseDown() {

			if (!isAlive) 
				return;

			// don't respond if alert is already visible
			if (boardManager.alertManager.gameObject.activeInHierarchy == true)
				return;

			boardManager.ShowActionPanel ( cageManager.gameObject.transform.position );
			boardManager.activeMonster = this.gameObject;
		}

	}




}