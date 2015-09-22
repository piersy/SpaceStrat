using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour
{
    //The list that holds all the lines
    List<int> linesList;
    //Cube that sits in front of the ship and can be dragged by the player in order to control the ship
    private GameObject controlCube;
    //Game object that is used to help construct the control mesh
    private GameObject controlMeshGameObject;
    Vector3[] newVertices;
    GlLineRenderer lineRenderer;
    void Start()
    {
        Vector3 front = frontOfShip();
        controlCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //ControlCube has its postion set to 10 units in front of the ship
        controlCube.transform.position = front + (transform.up * 10);
        controlCube.GetComponent<MeshRenderer>().material.color = Color.red;
        controlCube.AddComponent<Drag>();

        controlMeshGameObject = new GameObject();
        lineRenderer  = new GlLineRenderer(newMaterial());
    }

    private Material newMaterial()
    {
          Material  lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                "SubShader { Pass { " +
                "    Blend SrcAlpha OneMinusSrcAlpha " +
                "    ZWrite Off Cull Off Fog { Mode Off } " +
                "    BindChannels {" +
                "      Bind \"vertex\", vertex Bind \"color\", color }" +
                "} } }");
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
            return lineMaterial;
    }
    //Front of ship, the epoint from which the controll mesh starts
    Vector3 frontOfShip()
    {
        return new Vector3(transform.position.x, transform.position.y + GetComponent<Renderer>().bounds.extents.y, transform.position.z);
    }

    void LateUpdate()
    {
        Vector3 front = frontOfShip();
        //We draw our control mesh segments with a distance of 1 between them
        //So we truncate the distance to be an integer value to simplify segment placement
        int intDistance = (int)Vector3.Distance(controlCube.transform.position, front);
        //the direction from the ship to the control cube
        Vector3 direction = (controlCube.transform.position - front).normalized;
        //set the control cube to follow this direction
        controlCube.transform.rotation = Quaternion.LookRotation(direction);
        //control mesh verticies one segment per  unit distance
        newVertices = new Vector3[4 * intDistance];
        Vector2[] newUV;
        int vOffset = 0;
        int prevOffset;
        linesList = new List<int>();
        Vector3 segmentPosition = front;
        //Attempting to create a mesh object, to replace the existing object indicator
        for (int i = 0; i < intDistance; i++)
        {
            prevOffset = vOffset;
            vOffset = i * 4;
            
            segmentPosition = front + (i * direction);
            newVertices[vOffset + 0] = segmentPosition + controlCube.transform.up + controlCube.transform.right;
            newVertices[vOffset + 1] = segmentPosition - controlCube.transform.up + controlCube.transform.right;
            newVertices[vOffset + 2] = segmentPosition - controlCube.transform.up - controlCube.transform.right;
            newVertices[vOffset + 3] = segmentPosition + controlCube.transform.up - controlCube.transform.right;
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
		
    }

    void  OnRenderObject()
    {    
        lineRenderer.renderLines(newVertices, linesList.ToArray());
    }


}
