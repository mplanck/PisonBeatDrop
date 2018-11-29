using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pison
{
    public class ActiveBucketSensor : MonoBehaviour
    {
        public int activeBucketColumn = 0;
        public float resetDelayInSeconds = 0.35f;
        private void OnTriggerEnter(Collider other)
        {
            var cubePawn = other.GetComponent<CubePawn>();
            if (cubePawn != null)
            {
                activeBucketColumn = cubePawn.bucketColumn;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var cubePawn = other.GetComponent<CubePawn>();
            if (cubePawn != null)
            {
                if (activeBucketColumn == cubePawn.bucketColumn)
                {
                    activeBucketColumn = 0;
                }
            }
        }

        public async void ResetActiveBucketColumn()
        {
            var wait = new WaitForSeconds(resetDelayInSeconds);
            await wait;
            activeBucketColumn = 0;
        }
        
        void OnDrawGizmos()
        {
            Vector3 pos = new Vector3(this.transform.position.x,
                                      this.transform.position.y,
                                      this.transform.position.z);
            Vector3 scale = new Vector3(this.transform.lossyScale.x, 
                                        this.transform.lossyScale.y, 
                                        this.transform.lossyScale.z);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(pos, scale);
        }
    }
}