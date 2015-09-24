using UnityEngine;
using System.Collections;

public class GlLineRenderer
{
    
    private Material material;

    public GlLineRenderer(Material material)
    {
        this.material = material;
    }
    
    public void renderLines(Vector3[] verts, int[] indexes, float[] distances)
    {
        material.SetPass(0);
        GL.PushMatrix();
        //GL.MultMatrix(controlMeshGameObject.transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        for (int i = 0; i < indexes.Length; i++)
        {
            float d = distances[indexes[i]];
            // yellow fading to purple GL.Color(new Color(1f-(d/2f), 1f-d, d, 0.5f));
            //Yellow fading to darkish red
            GL.Color(new Color(1f-(d/2f), 1f-d, 0.2f, 0.5f));
            GL.Vertex(verts[indexes[i]]);
        }
        GL.End();
        GL.PopMatrix();
    }
}
