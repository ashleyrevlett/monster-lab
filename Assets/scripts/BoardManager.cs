using UnityEngine;
using System;
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
		public float cagePrice = 100f;
		private List <CageManager> cages; // programmatically placed

		public GameObject[] monsters; // monster prefabs, later accessed through cages
		private List <MonsterManager> monsterManagers;

		public GameObject playerPrefab; // prefab
		private GameObject player;

		private Transform boardHolder;     

		private ResourcesManager resources;

		
		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{

			resources = gameObject.GetComponent<ResourcesManager> ();

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

			cages = new List<CageManager> ();		

			monsterManagers = new List<MonsterManager> ();

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
			monsterManagers.Add (mm);													
			emptyCageMgr.monster = m; // bookkeeping
		
		}

	}

}