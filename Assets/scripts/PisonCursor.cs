using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


namespace Pison
{
	public class PisonCursor : MonoBehaviour
	{

		public GameObject cubeBusterPrefab;

		private Vector3 lastPosition_;
		private Vector3 velocity_;
		private bool busterActivated_ = false;

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				ResetCubeBuster();
				ActivateCubeBuster();
			}

		}

		public void FixedUpdate()
		{			
			velocity_ = (transform.position - lastPosition_)/Time.deltaTime;			
			lastPosition_ = transform.position;
		}
		
		public void ActivateCubeBuster()
		{
			if (!busterActivated_)
			{
				var buster = GameObject.Instantiate(cubeBusterPrefab,
					this.gameObject.transform.position,
					this.gameObject.transform.rotation);
				busterActivated_ = true;
				var cubeBuster = buster.GetComponent<CubeBuster>();
				cubeBuster.cursor = this;
				cubeBuster.transform.localScale = Vector3.one;
			}
		}

		public void ResetCubeBuster()
		{
			busterActivated_ = false;
		}		
	}
}