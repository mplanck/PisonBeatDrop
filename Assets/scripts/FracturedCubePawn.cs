using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pison
{
  public class FracturedCubePawn : TerminalPawn
  {
    public float           impactForce    = 10.0f;
    public float           velocityForce  = 10.0f;
    public List<Rigidbody> rigidbodies    = new List<Rigidbody>();
    public float           upwardModifier = 0.0f;
    public AudioSource     audioSource;
    public List<AudioClip> audioClips = new List<AudioClip>();
    public float pulseCooldown = 0.2f;
    
    private Dictionary<Material, Color> pulseColorsPerMaterial_ = new Dictionary<Material, Color>();
    private Dictionary<GameObject, Material> pieceMaterials_ = new Dictionary<GameObject, Material>();
    private Color baseColor_;
    
    public void Awake()
    {
      foreach (var rbody in rigidbodies)
      {
        pieceMaterials_[rbody.gameObject] = rbody.gameObject.GetComponent<MeshRenderer>().material;
      }
    }

    public void ExplodeAt(Vector3 inImpactPosition, Vector3 inImpactVelocity)
    {
      foreach (var rbody in rigidbodies)
      {
        Vector3 force = velocityForce * inImpactVelocity * Time.deltaTime;
        rbody.AddForceAtPosition(force, inImpactPosition);
        rbody.AddExplosionForce(impactForce, inImpactPosition, 0.5f, upwardModifier, ForceMode.Impulse);
        var material = pieceMaterials_[rbody.gameObject];
      }

      if (audioSource != null)
      {
        var audioClip = audioClips[Random.Range(0, audioClips.Count)];
        audioSource.PlayOneShot(audioClip);
      }
    }

    protected void Update()
    {
      foreach (KeyValuePair<GameObject, Material> item in pieceMaterials_)
      {
        if (pulseColorsPerMaterial_.ContainsKey(item.Value))
        {
          float pulseAmount = pulseColorsPerMaterial_[item.Value].grayscale;
          if (pulseAmount > 0.0f)
          {
            item.Value.SetColor("_EmissionColor", 0.025f * baseColor_ + pulseColorsPerMaterial_[item.Value]);
            pulseColorsPerMaterial_[item.Value] = Color.Lerp(pulseColorsPerMaterial_[item.Value], Color.black, pulseCooldown);
          }
        }
      }
      base.Update();
    }

    public void FixedUpdate()
    {
      
      foreach (var rbody in rigidbodies)
      {
        rbody.velocity = new Vector3(rbody.velocity.x, rbody.velocity.y, 0.0f);
      }
    }

    public void UpdateColor(Color inColor)
    {
      baseColor_ = inColor;
      foreach(KeyValuePair<GameObject, Material> item in pieceMaterials_)
      {
        pulseColorsPerMaterial_[item.Value] = 10.0f * Color.Lerp(baseColor_, Color.white, 0.5f);
        item.Value.SetColor("_EmissionColor", .025f * baseColor_ + pulseColorsPerMaterial_[item.Value]);
        item.Value.SetColor("_Color",         0.5f  * baseColor_);
      }
    }
    
    public void UpdateRigidBodyToMatch(Rigidbody inRigidBody)
    {
      foreach (var rbody in rigidbodies)
      {
        rbody.velocity = inRigidBody.velocity;
      }
    }
  }
}