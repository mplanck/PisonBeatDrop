using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pison
{
    public class CubeBuster : MonoBehaviour
    {
        public Color startColor = Color.red;
        public Color endColor = Color.yellow;
        public float startScale = 2.0f;
        public float endScale = 4.0f;

        public new MeshRenderer renderer;
        public new Rigidbody rigidbody;
        public float lifespan = 0.5f;
        public PisonCursor cursor;

        private Material material_;
        private float lifetime = 0.0f;
        private float scalefactor = 1.0f;

        void Awake()
        {
            material_ = renderer.materials[0];
        }

        void Start()
        {
            lifetime = 0.0f;
        }

        void Update()
        {
            if (lifetime >= lifespan)
            {
                Destroy(this.gameObject);
            }

            lifetime += Time.deltaTime;
            float lifepercentage = lifetime / lifespan;

            scalefactor = endScale * (lifepercentage * lifepercentage) + startScale;

            Color scalecolor = Color.Lerp(startColor, endColor, lifepercentage);
            material_.SetColor("_Color", scalecolor);
        }

        void FixedUpdate()
        {
            this.transform.position = cursor.transform.position;
            this.transform.localScale = new Vector3(scalefactor, scalefactor, scalefactor);
        }

        void OnCollisionEnter(Collision inCollision)
        {
            var cubePawn = inCollision.gameObject.GetComponent<CubePawn>();
            if (cubePawn != null)
            {
                foreach (var contact in inCollision.contacts)
                {
                    cubePawn.ExplodeAt(contact.point, rigidbody.velocity);
                }
            }
        }
    }
}