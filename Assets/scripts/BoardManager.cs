using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;       //Allows us to use Lists.
using Random = UnityEngine.Random;      //Tells Random to use the Unity Engine random number generator.

namespace Completed
	
{
	
	public class BoardManager : MonoBehaviour
	{

		public int columns = 8;                                         //Number of columns in our game board.
		public int rows = 4;                                            //Number of rows in our game board.
		public GameObject floorTile;

		public int numberCages = 3;
		public int numberMonsters = 2;
		public GameObject cage; // prefab sprite w/ mgr
		public float cagePrice = 50f;
		private List <CageManager> cages; // programmatically placed

		public GameObject[] monsters; // monster prefabs, later accessed through cages
		private List <MonsterManager> monsterManagers;

		public GameObject playerPrefab; // prefab
		private GameObject player;

		public GameObject labTable;
		private List <TableManager> tables;
		public float tablePrice = 100f;

		private Transform boardHolder;     

		private ResourcesManager resources;
		private AlertPanelManager alertManager;
		
		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{

			resources = gameObject.GetComponent<ResourcesManager> ();
			alertManager = GameObject.Find ("AlertPanel").GetComponent<AlertPanelManager>();

			boardHolder = new GameObject ("Board").transform;

			for(int y = 0; y < rows*2; y=y+2) {	
				for(int x = 0; x < columns*2; x=x+2) {

					GameObject instance = Instantiate (floorTile, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (boardHolder);

					// add cages to first few tiles
					if (cages.Count < numberCages) {
						GameObject c = Instantiate (cage, new Vector3 (x, y, -.1f), Quaternion.identity) as GameObject;
						c.transform.SetParent (boardHolder);

						CageManager cm = c.GetComponent<CageManager>();

						// add monster to cage
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

			// build board
			BoardSetup ();

			PlayerSetup ();

		}


		public void buildCage() {
		
			if (resources.money < cagePrice) 
				return;

			resources.money -= cagePrice;

			numberCages++;
			CageManager lastCageMgr = cages [cages.Count - 1];

			int x = (int)lastCageMgr.gameObject.transform.position.x + 2;
			int y = (int)lastCageMgr.gameObject.transform.position.y;

			GameObject c = Instantiate (cage, new Vector3 (x, y, -.1f), Quaternion.identity) as GameObject;
			c.transform.SetParent (boardHolder);			
			CageManager cm = c.GetComponent<CageManager>();
			cages.Add(cm);

		
		}

		public void AddMonster() {

			// find 1st empty cage
			CageManager emptyCageMgr = null;
			foreach (CageManager cm in cages) {
				if (cm.monster == null) {
					emptyCageMgr = cm;
					break;
				}			
			}

			if (emptyCageMgr == null)
				return;

			
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
		
		}


		public void AddLabTable() {
			
			if (resources.money < tablePrice) 
				return;
			
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


		}

		public void DoExperiment(MonsterManager mm) {

			GameObject monster = mm.gameObject;

			// find open lab table
			TableManager tm = tables.Find (x => x.monster == null);

			// if there's no open table, can't experiment
			if (tm == null) {
				Debug.Log ("No table to experiment on!");
				alertManager.ShowMessage("You need a lab table to conduct experiments!");
				return;
			}

			// assign monster to open lab table
			tm.monster = monster;

			// move position of monster sprite to lab table
			monster.transform.position = tm.gameObject.transform.position;
			StartCoroutine(mm.DealDamage (1f, 10, 1f));		
			StartCoroutine(resources.GiveMoney (1f, 10, 1f));		

		}

		public void EndExperiment(MonsterManager mm) {
		
			// find monster's home cage 
			CageManager cage = cages.Find (x=> x.monster == mm.gameObject);
			TableManager table = tables.Find (x=> x.monster == mm.gameObject);

			table.monster = null;
			mm.gameObject.transform.position = cage.gameObject.transform.position;

		
		}

	}

}