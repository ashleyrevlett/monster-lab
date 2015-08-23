using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Completed
	
{

	public class MonsterManager : MonoBehaviour {

		public string monsterName = "unnamed";
		public float damage = 0f;
		public float hunger = 0f;
		public float thirst = 0f;
		public bool isAlive = true;

		public float hungerDamage = 2f;
		public float thirstDamage = 5f;

		public Vector2 appetiteMinMax = new Vector2(.1f, 2f);
		private float appetite = 0;

		public Canvas statsCanvas;
		public Canvas buttonCanvas;
		public Text nameText;
		public Text healthText;
		public Text thirstText;
		public Text hungerText;

		private RectTransform buttonRT;
		private RectTransform statsRT;

		public string[] firstNames;
		public string[] lastNames;

		public GameObject actionPanel;

		private ResourcesManager resources;


		// Use this for initialization
		void Start () {

			resources = GameObject.Find ("GameManager").GetComponent<ResourcesManager> ();

			buttonRT = buttonCanvas.GetComponent<RectTransform> ();
			statsRT = statsCanvas.GetComponent<RectTransform> ();

			int randomFirstIdx = Random.Range (0, firstNames.Length);
			int randomLastIdx = Random.Range (0, lastNames.Length);

			monsterName = firstNames [randomFirstIdx] + lastNames [randomLastIdx];

			appetite = Random.Range (appetiteMinMax.x, appetiteMinMax.y);

			InvokeRepeating ("Tick", 5f, 5f);

			actionPanel.SetActive (false);

		}
		
		// Update is called once per frame
		void Update () {

			// position labels
			Vector3 pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);	
			statsRT.position = new Vector2(pos.x + 30f, pos.y - 70f);
			buttonRT.position = new Vector2(pos.x, pos.y);

			if (!isAlive)
				return;

			nameText.text = monsterName;
			healthText.text = "DAMAGE: " + damage.ToString ("F1") + "%";
			thirstText.text = "THIRST: " + thirst.ToString ("F1") + "%";
			hungerText.text = "HUNGER: " + hunger.ToString ("F1") + "%";


			if (damage >= 100f)
				Die ();

			if (hunger >= 100f)
				damage -= (Time.deltaTime * hungerDamage);

			if (thirst >= 100f)
				damage -= (Time.deltaTime * thirstDamage);
		
		}

		void Die() {
			isAlive = false;
			CancelInvoke("Tick");

			// change sprite
		}

		void Tick() {	
			hunger += appetite;
			thirst += appetite*1.5f;
		}

		public void Feed() {
			hunger = 0f;	
			resources.food = (int) Mathf.Max (0f, resources.food - 10f);
			actionPanel.SetActive (false);
			Debug.Log ("Fed");
		}

		public void Water() {
			thirst = 0f;	
			resources.water = (int) Mathf.Max (0f, resources.water - 10f);
			actionPanel.SetActive (false);
			Debug.Log ("Watered");

		}

		public void Experiment() {
			damage += 20f;	
			resources.science += 40;
			resources.money += 100;
			actionPanel.SetActive (false);
			Debug.Log ("Experimented");

		}

		public void ClosePanel() {
			actionPanel.SetActive (false);
		}

		// mouse click on monster -> show action panel

		void OnMouseDown() {
			actionPanel.SetActive (true);
		}

	}




}