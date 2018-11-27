using UnityEngine;

namespace Pison
{
  public class TerminalPawn : MonoBehaviour
  {
    public  float lifetime = 4.0f;
    private float life_    = 0.0f;

    protected void Update()
    {
      life_ += Time.deltaTime;
      if (life_ >= lifetime)
      {
        Destroy(this.gameObject);
      }
    }
  }
}