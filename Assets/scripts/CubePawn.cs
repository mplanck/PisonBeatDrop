using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pison
{
  public class CubePawn : TerminalPawn
  {
    public new MeshRenderer renderer;
    public new Rigidbody    rigidbody;
    public     GameObject   fracturedCubePrefab;
    public     float        initialVelocityDown = 1.0f;
    public     PisonGameManager  spawner;
    public     int          bucketColumn;

    private bool     exploded_ = false;
    private Material material_;
    private Color    materialColor_;

    void Awake()
    {
      material_ = renderer.material;
    }

    void Start()
    {
      rigidbody.velocity = initialVelocityDown * Vector3.down;
      rigidbody.angularVelocity = new Vector3(Random.Range(-Mathf.PI, Mathf.PI),
                                              Random.Range(-Mathf.PI, Mathf.PI),
                                              Random.Range(-Mathf.PI, Mathf.PI));
    }

    public void UpdateColor(Color inColor)
    {
      materialColor_ = inColor;
      material_.SetColor("_EmissionColor", materialColor_);
      material_.SetColor("_Color",         materialColor_);
    }

    public void ExplodeAt(Vector3 inPosition, Vector3 inImpactVelocity)
    {
      if (exploded_)
      {
        return;
      }

      spawner.PulseColumn(bucketColumn, Color.white);
      spawner.AddToScore(1);
      spawner.sensor.ResetActiveBucketColumn();
      var fracturedCube =
        GameObject.Instantiate(fracturedCubePrefab, this.transform.position, this.transform.rotation);
      var fracturedCubePawn = fracturedCube.GetComponent<FracturedCubePawn>();
      if (fracturedCubePawn == null)
      {
        return;
      }

      fracturedCubePawn.UpdateRigidBodyToMatch(this.rigidbody);
      fracturedCubePawn.ExplodeAt(inPosition, inImpactVelocity);
      fracturedCubePawn.UpdateColor(materialColor_);
      Destroy(this.gameObject);
      exploded_ = true;
    }
  }
}