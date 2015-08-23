using UnityEngine;
using System.Collections;


namespace Completed {

	public class ResourcesManager : MonoBehaviour {
		
		public int playerLevel = 1;
		public int food = 100;
		public int water = 100;
		public int money = 0;
		public int science = 0;
		public float timePlayed = 0f;	
		public int secondsPerDay = 600; // 10 min / day
		public int daysPlayed = 0;
		
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
			timePlayed += Time.deltaTime;

			daysPlayed = Mathf.FloorToInt(timePlayed / secondsPerDay);

		}
	}

}