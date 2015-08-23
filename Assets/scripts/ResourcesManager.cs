using UnityEngine;
using System.Collections;


namespace Completed {

	public class ResourcesManager : MonoBehaviour {
		
		public int playerLevel = 1;
		public int food = 100;
		public int water = 100;
		public float money = 0;
		public int science = 0;
		public float timePlayed = 0f;	
		public int secondsPerDay = 600; // 10 min / day
		public int daysPlayed = 0;

		private NotificationsManager notifManager;

		void Start() {
			notifManager = GameObject.Find ("Notifications").GetComponent<NotificationsManager> ();
		}

		void Update () {		
			timePlayed += Time.deltaTime;
			daysPlayed = Mathf.FloorToInt(timePlayed / secondsPerDay);
		}

		public IEnumerator GiveMoney(float amount, int numberTimes, float delayTime ) {		
			for (int i = 0; i < numberTimes; i++) {					
				money = Mathf.Max (0f, money + amount);	

				string msg = string.Format("Earned ${0}", amount.ToString("F2"));
				notifManager.ShowNotice (msg);

				yield return new WaitForSeconds (delayTime);	
			}
		}


	}

}