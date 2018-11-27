using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pison
{
    public class PisonDisplaySignalLine : MonoBehaviour
    {
        public PisonController controller;
        public Vector3 offset = Vector3.zero;
        public float yscale = 0.001f;
        public float width = 10.0f;
        public float threshold = 10000f;
        public float lineWidth = 1.0f;

        private void OnPostRender()
        {
            if (controller.liftBuffer.Size == 0)
            {
                return;
            }
            Matrix4x4 mat = new Matrix4x4();
            mat.SetTRS(this.transform.position + offset +new Vector3(0.0f, 0.0f, 2.0f),
                this.transform.rotation,
                Vector3.one);
            GL.PushMatrix();
            GL.MultMatrix(mat);
            GL.Begin(GL.LINES);
            GL.Color(new Color(1.0f, 0.75f, 0.1f));
            float x = 0.0f;
            float dx = width / controller.liftBuffer.Capacity;
            Vector3 currPoint = new Vector3(x, controller.liftBuffer[0] * yscale, 0.0f);
            Vector3 nextPoint = Vector3.zero;
            for (int i = 1; i < controller.liftBuffer.Size; i++)
            {
                nextPoint = currPoint;
                nextPoint.x += dx;
                nextPoint.y = yscale * controller.liftBuffer[i];
                GL.Vertex(currPoint);
                GL.Vertex(nextPoint);
                currPoint = nextPoint;
            }
            GL.End();
            
            GL.Begin(GL.QUADS);
            GL.Color(Color.red);
            GL.Vertex(new Vector3(0.0f, yscale * threshold, 0.0f));
            GL.Vertex(new Vector3(width, yscale * threshold, 0.0f));
            GL.Vertex(new Vector3(width, yscale * threshold + lineWidth, 0.0f));
            GL.Vertex(new Vector3(0.0f, yscale * threshold + lineWidth, 0.0f));
            GL.End();
            GL.PopMatrix();
        }
    }
}