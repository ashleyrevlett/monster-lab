using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Completed	{

	public class GUIManager : MonoBehaviour {

		public Text date;
		public Text level;
		public Text food;
		public Text water;
		public Text monsterCount;
		public Text money;
		public Text science;

		private ResourcesManager resources;
		private BoardManager board;

		// Use this for initialization
		void Start () {
		
			resources = gameObject.GetComponent<ResourcesManager> ();
			board = gameObject.GetComponent<BoardManager> ();

		}
		
		// Update is called once per frame
		void Update () {
		
			date.text = "Day " + resources.daysPlayed;
			level.text = "Level " + resources.playerLevel;

			food.text = "FOOD: " + resources.food.ToString ();
			water.text = "WATER: " + resources.water.ToString ();
			monsterCount.text = "MONSTERS: " + board.numberMonsters + " / " + board.numberCages;
			money.text = "MONEY: $" + resources.money.ToString ("F2");
			science.text = "SCIENCE: " + resources.science;

		}
	}

}