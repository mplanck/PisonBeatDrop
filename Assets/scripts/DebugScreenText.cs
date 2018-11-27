using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Pison
{
  public class DebugScreenText : MonoBehaviour
  {
    public class DebugText
    {
      public Vector3 position;
      public string  text;
      public Color?  color;
      public float   timeOfDeath;
    }

    public List<DebugText> textList = new List<DebugText>();

    public void OnDrawGizmos()
    {
#if UNITY_EDITOR
      UnityEditor.Handles.BeginGUI();
      foreach (var textItem in textList)
      {
        GUIStyle style = new GUIStyle();
        Color    color = textItem.color ?? Color.green;
        style.normal.textColor = color;

        UnityEditor.Handles.color = color;
        UnityEditor.Handles.Label(textItem.position, textItem.text, style);
      }      
      UnityEditor.Handles.EndGUI();
#endif
    }

    private static DebugScreenText m_instance;

    public static DebugScreenText instance
    {
      get
      {
        if (m_instance == null)
        {
          m_instance = GameObject.FindObjectOfType<DebugScreenText>();
          if (m_instance == null)
          {
            var go = new GameObject("[singleton] DebugScreenText");
            m_instance = go.AddComponent<DebugScreenText>();
          }
        }

        return m_instance;
      }
    }

    public static void DrawText(Vector3 pos, string text, Color? color = null, float duration = 1.0f)
    {
      instance.textList.Add(new DebugScreenText.DebugText()
                            {
                              position    = pos,
                              text        = text,
                              color       = color,
                              timeOfDeath = Time.time + duration
                            });

      for (var i = instance.textList.Count - 1; i >= 0; i--)
      {
        var textItem = instance.textList[i];
        if (textItem.timeOfDeath <= Time.time)
        {
          instance.textList.RemoveAt(i);
        }
      }
    }
  }
}