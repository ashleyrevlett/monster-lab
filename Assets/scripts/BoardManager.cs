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
		private List <CageManager> cages; // programmatically placed

		public GameObject[] monsters; // monster prefabs, later accessed through cages
		private List <MonsterManager> monsterManagers;

		public GameObject playerPrefab; // prefab
		private GameObject player;

		private Transform boardHolder;     

		
		//Sets up the outer walls and floor (background) of the game board.
		void BoardSetup ()
		{
			//Instantiate Board and set boardHolder to its transform.
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
				
		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{

			cages = new List<CageManager> ();		
			monsterManagers = new List<MonsterManager> ();

			//Creates the outer walls and floor.
			BoardSetup ();

			PlayerSetup ();

			//Instantiate the exit tile in the upper right hand corner of our game board
			// Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
		}
	}
}