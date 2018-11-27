using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pison
{
    public class PlaneConstrainer : MonoBehaviour
    {

        public float maxConstraint = 0.1f;
        public float minConstraint = -0.1f;
        
        private void FixedUpdate()
        {
            transform.position = new Vector3(transform.position.x,
                transform.position.y,
                Mathf.Clamp(transform.position.z, minConstraint, maxConstraint));
        }
    }
}
