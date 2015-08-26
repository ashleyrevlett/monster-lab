using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

namespace Completed
	
{
	
	public class BoardManager : MonoBehaviour
	{

		public string introStatement = "Click a monster to interact with it. Run experiments to earn money.";

		public GameObject actionPanel; 
		private RectTransform actionPanelRect;

		public int columns = 8;                                         //Number of columns in our game board.
		public int rows = 4;                                            //Number of rows in our game board.
		public GameObject floorTile;

		public int maxCages = 5;
		public int maxTables = 5;

		public int numberCages = 3;
		public int numberMonsters = 2;
		public GameObject cage; // prefab sprite w/ mgr
		public float cagePrice = 50f;
		private List <CageManager> cages; // programmatically placed

		public GameObject[] monsters; // monster prefabs, later accessed through cages
		private List <MonsterManager> monsterManagers;
		public float monsterPrice = 150f;

		public GameObject playerPrefab; // prefab
		private GameObject player;

		public GameObject labTable;
		private List <TableManager> tables;
		public float tablePrice = 100f;

		private Transform boardHolder = null;     

		private ResourcesManager resources;
		public AlertPanelManager alertManager;

		public float foodPrice = 10f;

		private NotificationsManager notifManager;

		private MonsterManager focusedMonster; // monster selected by clicking cage


		void Start() {

			notifManager = GameObject.Find ("Notifications").GetComponent<NotificationsManager> ();						
			actionPanelRect = actionPanel.GetComponent<RectTransform> ();
			resources = gameObject.GetComponent<ResourcesManager> ();
			alertManager = GameObject.Find ("AlertPanel").GetComponent<AlertPanelManager>();
			boardHolder = new GameObject ("Board").transform;

			alertManager.ClosePanel();

			//DoIntro ();

		}

		public List<TableManager> GetTables() {
			return (tables);
		}

		public List<CageManager> GetCages() {
			return (cages);
		}

		void Update() {
		
			if (resources.money >= resources.playerLevel * 1000f) {
				resources.playerLevel += 1;
				alertManager.ShowMessage("LEVEL UP! Thanks to your efforts our knowledge of theses strange creatures has grown by leaps and bounds.");
			}

		}


		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{

			for(int y = 0; y < rows*2; y=y+2) {	
				for(int x = 0; x < columns*2; x=x+2) {

					GameObject instance = Instantiate (floorTile, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);

					// add cages to first few tiles
					if (cages.Count < numberCages) {
						GameObject c = Instantiate (cage, new Vector3 (x, y, -.1f), Quaternion.identity) as GameObject;
						c.transform.SetParent (boardHolder);

						// add monster to cage
						CageManager cm = c.GetComponent<CageManager>();
						if (monsterManagers.Count < numberMonsters) {
							int randomIndex = Random.Range(0,(monsters.Length - 1));
							GameObject randomMonster = monsters[randomIndex];
							GameObject m = Instantiate (randomMonster, new Vector3 (x, y, -.05f), Quaternion.identity) as GameObject;
							m.transform.SetParent(c.transform);
							MonsterManager mm = m.GetComponent<MonsterManager>();
							mm.cageManager = cm;
							monsterManagers.Add (mm);													
							cm.monster = m; // bookkeeping
						}

						// add cage to list
						cages.Add(cm);

					}

				}
			}	

			HideActionPanel ();

		}

		private void PlayerSetup() {		
			player = Instantiate (playerPrefab, new Vector3 (0f, (rows-1)*2f, -.2f), Quaternion.identity) as GameObject;
			player.transform.SetParent (boardHolder);
		}
			

		public void SetupScene (int level)
		{
			
			// init lists
			cages = new List<CageManager> ();		
			monsterManagers = new List<MonsterManager> ();
			tables = new List<TableManager> ();
			cages.Clear ();
			monsterManagers.Clear ();
			tables.Clear();

			StopAllCoroutines ();
			CancelInvoke ();

			if (boardHolder != null) {
				if(boardHolder.childCount > 0) {
					foreach (Transform childTransform in boardHolder) {
						Destroy(childTransform.gameObject);
					}
				}
			}

			// build board
			BoardSetup ();

			PlayerSetup ();

		}


		public void buildCage() {
		
			if (resources.money < cagePrice) {
				alertManager.ShowMessage("Not enough money to purchase!");
				return;
			}

			if (cages.Count >= maxCages) {
				alertManager.ShowMessage("Maximum number of cages for this level reached.");
				return;
			}

			resources.money -= cagePrice;

			numberCages++;
			CageManager lastCageMgr = cages [cages.Count - 1];

			int x = (int)lastCageMgr.gameObject.transform.position.x + 2;
			int y = (int)lastCageMgr.gameObject.transform.position.y;

			GameObject c = Instantiate (cage, new Vector3 (x, y, -.1f), Quaternion.identity) as GameObject;
			c.transform.SetParent (boardHolder);			
			CageManager cm = c.GetComponent<CageManager>();
			cages.Add(cm);

			string msg = string.Format("Purchased Cage for ${0}", cagePrice);
			notifManager.ShowNotice (msg);

		}


		public void FoodWaterRefill() {
			
			if (resources.money < foodPrice) {
				alertManager.ShowMessage("You need more money to purchase a Food and Water Refill.");
				return;
			}
			
			resources.money -= foodPrice;

			resources.food += 10;
			resources.water += 10;

			string msg = string.Format("Purchased 10 Food and 10 Water for ${0}", foodPrice);
			notifManager.ShowNotice (msg);

		}


		public void AddMonster() {

			HideActionPanel ();

			if (resources.money < monsterPrice) {
				alertManager.ShowMessage("You need more money to purchase a Monster.");
				return;
			}
						
			if (cages.Count <= monsterManagers.Count) {
				alertManager.ShowMessage("You need an empty cage before purchasing a new Monster.");
				return;
			}

			// find 1st empty cage
			CageManager emptyCageMgr = null;
			foreach (CageManager cm in cages) {
				if (cm.monster == null) {
					emptyCageMgr = cm;
					break;
				}			
			}

			if (emptyCageMgr == null) {
				alertManager.ShowMessage("No empty cages available!");
				return;
			}
			
			// add monster to cage
			numberMonsters++;

			GameObject c = emptyCageMgr.gameObject;

			int x = (int)emptyCageMgr.gameObject.transform.position.x;
			int y = (int)emptyCageMgr.gameObject.transform.position.y;

			int randomIndex = Random.Range(0,(monsters.Length - 1));
			GameObject randomMonster = monsters[randomIndex];
			GameObject m = Instantiate (randomMonster, new Vector3 (x, y, -.05f), Quaternion.identity) as GameObject;
			m.transform.SetParent(c.transform);
			MonsterManager mm = m.GetComponent<MonsterManager>();
			mm.cageManager = emptyCageMgr.GetComponent<CageManager> ();
			monsterManagers.Add (mm);	

			emptyCageMgr.monster = m; // bookkeeping
					
			string msg = string.Format("Purchased 1 Monster for ${0}", monsterPrice);
			notifManager.ShowNotice (msg);


		}


		public void AddLabTable() {
			
			if (resources.money < tablePrice) {
				alertManager.ShowMessage("You need more money to purchase a Lab Table ($100).");
				return;
			}

			if (tables.Count >= maxTables) {
				alertManager.ShowMessage("Maximum number of tables for this level reached.");
				return;
			}

			resources.money -= tablePrice;

			// 1st table's position
			float xpos = 1f;
			float ypos = (rows - 1) * 2f;

			// if there's already a table, position the new one to the right
			if (tables.Count > 0) {
				TableManager lastTable = null;
				lastTable = tables [tables.Count - 1];
				xpos = lastTable.transform.position.x + 2f;
				ypos = lastTable.transform.position.y;
			}

			GameObject t = Instantiate (labTable, new Vector3 (xpos, ypos, -.05f), Quaternion.identity) as GameObject;
			t.transform.SetParent(boardHolder.transform);
			tables.Add (t.GetComponent<TableManager> ());		
			
			string msg = string.Format("Purchased 1 Lab Table for ${0}", tablePrice);
			notifManager.ShowNotice (msg);

		}
			

		public void Feed() {
			
			HideActionPanel ();

			if (focusedMonster == null) {
				Debug.Log ("No focused monster?");
				return;
			}

			if (resources.food > 0) {
				focusedMonster.Feed ();
				resources.food = (int) Mathf.Max (0f, resources.food - 1f);
				string msg = string.Format("Fed {0}. Food Remaining: {1}", focusedMonster.monsterName, resources.food);
				notifManager.ShowNotice (msg);
			} else {
				alertManager.ShowMessage("No food left!");
			}

			focusedMonster = null; // monster no longer focused

		}


		public void Water() {
			
			HideActionPanel ();
						
			if (focusedMonster == null) {
				Debug.Log ("No focused monster?");
				return;
			}

			if (resources.water > 0) {
				focusedMonster.Water();
				resources.water = (int) Mathf.Max (0f, resources.water - 1f);
				string msg = string.Format("Watered {0}. Water Remaining: {1}.", focusedMonster.monsterName, resources.water);
				notifManager.ShowNotice (msg);
			} else {
				alertManager.ShowMessage("No water left!");
			}

			focusedMonster = null; // monster no longer focused

		}


		public void Experiment() {
					
			HideActionPanel ();
			
			// find empty lab table
			TableManager table = tables.Find (x => x.monster == null);
			
			// if there's no open table, can't experiment
			if (table == null) {
				alertManager.ShowMessage("You need an open lab table to conduct experiments!");
				return;
			}
			
			// assign monster to open lab table
			table.monster = focusedMonster.gameObject;
			
			// move position of monster sprite to lab table
			Vector3 newPos = table.gameObject.transform.position;
			newPos.y += .2f;
			focusedMonster.gameObject.transform.position = newPos;

			// deal damage and ++money
			StartCoroutine(focusedMonster.DealDamage (5f, 5, 1f));		
			StartCoroutine(resources.GiveMoney (10f, 5, 1f));		
		
			focusedMonster = null; // monster no longer focused

		}


		public void SetMonsterFocus(MonsterManager monster) {
			focusedMonster = monsterManagers.Find (x => x == monster);
			Debug.Log ("Focused Monster is: " + focusedMonster);
		}


		public void HideActionPanel() {
			actionPanel.SetActive (false);
		}

		public void ShowActionPanel(Vector3 position) {
			StartCoroutine(WaitFrameAndShow (position));
		}

		private IEnumerator WaitFrameAndShow(Vector3 position) {
			yield return 0;
			ShowPanel (position);
		}

		private void ShowPanel(Vector3 position) {
			Vector3 pos = Camera.main.WorldToScreenPoint(position);	
			actionPanelRect.position = new Vector2(pos.x, pos.y);
			actionPanel.SetActive (true);
		}

	}

}