using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Completed
{
	public class NotificationsManager : MonoBehaviour {
		
		public Text noticeText;


		// Use this for initialization
		void Start () {
			noticeText.text = "";
		}

		public void ShowNotice(string notice) {
			StopAllCoroutines ();
			if (noticeText.text.Length > 0) {
				notice = noticeText.text + "\n" + notice;
			}
			noticeText.text = notice;
			StartCoroutine(HideInSeconds(3f));
		}

		private IEnumerator HideInSeconds(float sec) {
			yield return new WaitForSeconds (sec);
			noticeText.text = "";
		}


	}

}