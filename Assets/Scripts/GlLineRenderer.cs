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
            GL.Color(new Color(1, 1, 1, 1f-(distances[indexes[i]])));
            GL.Vertex(verts[indexes[i]]);
        }
        GL.End();
        GL.PopMatrix();
    }
}
