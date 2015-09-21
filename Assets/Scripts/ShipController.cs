using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour
{
    List<int> linesList;
    //Cube that sits in front of the ship and can be dragged by the player in order to control the ship
    private GameObject controlCube;
    //Wireframe mesh that helps indicate the control applied to the ship
    private GameObject controlMeshGameObject;
    private Mesh controlMesh;
    //The material used to render the wireframe of the controlmesh
    private Material lineMaterial;
    Color lineColor = Color.red;
    Color backgroundColor = Color.black;
    bool ZWrite = true;
    bool AWrite = true;
    bool blend = true;
    //The indexes of al the lines used to draw the control mesh
    void Start()
    {
        Debug.Log("Starring ship");

        Vector3 front = frontOfShip();
        controlCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //ControlCube has its postion set to 10 units in front of the ship
        controlCube.transform.position = front + (transform.up * 10);
        controlCube.GetComponent<MeshRenderer>().material.color = Color.red;
        controlCube.AddComponent<Drag>();

        Vector3[] verts = controlCube.GetComponent<MeshFilter>().mesh.vertices;
        controlMeshGameObject = new GameObject();
        MeshFilter meshFilter = controlMeshGameObject.AddComponent<MeshFilter>();
        controlMesh = controlMeshGameObject.GetComponent<MeshFilter>().mesh;
        controlMeshGameObject.AddComponent<MeshRenderer>(); 
    }

    //Front of ship, the epoint from which the controll mesh starts
    Vector3 frontOfShip()
    {
        return new Vector3(transform.position.x, transform.position.y + GetComponent<Renderer>().bounds.extents.y, transform.position.z);
    }

    private Vector3 screenPoint;
    private Vector3 offset;

    Vector3[] newVertices;
    void LateUpdate()
    {
        Vector3 front = frontOfShip();
        //We draw our control mesh segments with a distance of 1 between them
        //So we truncate the distance to be an inteer value to simplify segment placement
        int intDistance = (int)Vector3.Distance(controlCube.transform.position, front);
        //the direction from the ship to the control cube
        Vector3 direction = (controlCube.transform.position - front).normalized;
        //set the control cube to follow this direction
        controlCube.transform.rotation = Quaternion.LookRotation(direction);
        //control mesh verticies one segment per  unit distance
        newVertices = new Vector3[4 * intDistance];
        Vector2[] newUV;
        //control mesh triangles we will have 2 triangles per face and distance-1 * 4 face
        //a triangle is made up of 3 indexes 
        int[] newTriangles = new int[intDistance * 4 * 2 * 3];
        //we precalculate the first set of verts here so that we can allways set triangles in the loop
        Vector3 segPos = front;
        newVertices[0] = segPos + controlCube.transform.up + controlCube.transform.right;
        newVertices[1] = segPos - controlCube.transform.up + controlCube.transform.right;
        newVertices[2] = segPos - controlCube.transform.up - controlCube.transform.right;
        newVertices[3] = segPos + controlCube.transform.up - controlCube.transform.right;
        int vOffset = 0;
        int prevOffset;
        //we want four lines for each segment and then four lines to connect each segment to the next
        linesList = new List<int>();
        linesList.Add(0);
        linesList.Add(1);

        linesList.Add(1);
        linesList.Add(2);
            
        linesList.Add(2);
        linesList.Add(3);
            
        linesList.Add(3);
        linesList.Add(0);
        //Attempting to create a mesh object, to replace the existing object indicator
        for (int i = 1; i < intDistance; i++)
        {
            prevOffset = vOffset;
            vOffset = i * 4;
            
            segPos = front + (i * direction);
            newVertices[vOffset + 0] = segPos + controlCube.transform.up + controlCube.transform.right;
            newVertices[vOffset + 1] = segPos - controlCube.transform.up + controlCube.transform.right;
            newVertices[vOffset + 2] = segPos - controlCube.transform.up - controlCube.transform.right;
            newVertices[vOffset + 3] = segPos + controlCube.transform.up - controlCube.transform.right;
            //Add lines connecting each segment
            linesList.Add(vOffset + 0);
            linesList.Add(vOffset + 1);

            linesList.Add(vOffset + 1);
            linesList.Add(vOffset + 2);
            
            linesList.Add(vOffset + 2);
            linesList.Add(vOffset + 3);
            
            linesList.Add(vOffset + 3);
            linesList.Add(vOffset + 0);
            
            //segment connecting lines
            linesList.Add(prevOffset + 0);
            linesList.Add(vOffset + 0);

            linesList.Add(prevOffset + 1);
            linesList.Add(vOffset + 1);

            linesList.Add(prevOffset + 2);
            linesList.Add(vOffset + 2);

            linesList.Add(prevOffset + 3);
            linesList.Add(vOffset + 3);

        }
		
        drawThisThing();
    }

    void setQuadTris(int index, int[] tris, int topRight, int bottomRight, int topLeft, int bottomLeft)
    {
        setTri(index, tris, topRight, topLeft, bottomRight);
        setTri(index + 3, tris, bottomRight, bottomLeft, topLeft);
    }

    void setTri(int index, int[] tris, int corner1, int corner2, int corner3)
    {
        tris[index + 0] = corner1;
        tris[index + 1] = corner2;
        tris[index + 2] = corner3;
    }

    private void drawThisThing()
    {
    
        controlMeshGameObject.GetComponent<MeshRenderer>().material = new Material("Shader \"Lines/Background\" { Properties { _Color (\"Main Color\", Color) = (1,1,1,1) } SubShader { Pass {" + (ZWrite ? " ZWrite on " : " ZWrite off ") + (blend ? " Blend SrcAlpha OneMinusSrcAlpha" : " ") + (AWrite ? " Colormask RGBA " : " ") + "Lighting Off Offset 1, 1 Color[_Color] }}}");
  
        // Old Syntax without Bind :    
        //   lineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha ZWrite On Cull Front Fog { Mode Off } } } }"); 
  
        // New Syntax with Bind : 
        lineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass { Blend SrcAlpha OneMinusSrcAlpha BindChannels { Bind \"Color\",color } ZWrite On Cull Front Fog { Mode Off } } } }");    
  
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
  
        List<Vector3> linesArray = new List<Vector3>();
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter.sharedMesh;
    }
	

    void  OnRenderObject()
    {    
        MeshRenderer meshRenderer = controlMeshGameObject.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial.color = backgroundColor; 
        lineMaterial.SetPass(0); 
             
        GL.PushMatrix(); 
        GL.MultMatrix(controlMeshGameObject.transform.localToWorldMatrix); 
        GL.Begin(GL.LINES); 
        GL.Color(lineColor); 
             
        Debug.Log(linesList.Count);
        for (int i = 0; i < linesList.Count; i++)
        { 
            GL.Vertex(newVertices[linesList[i]]); 
        } 
             
        GL.End(); 
        GL.PopMatrix(); 
    }

}
