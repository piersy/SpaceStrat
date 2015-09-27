using UnityEngine;

//using System.Collections;
using System.Collections.Generic;

public class BezierControlMesh
{
    //The list that holds all the lines
    private List<int> linesList;
    //Cube that sits in front of the ship and can be dragged by the player in order to control the ship
    private GameObject controlCube;
    //Game object that is used to help construct the control mesh
    private GameObject controlMeshGameObject;
    //The point form which the mesh starts, this shoudl be in local space of the transform
    private Vector3 startPoint;
    private Vector3[] newVertices;
    private float[] distances;
    private GlLineRenderer lineRenderer;
    private GameObject curveControl;
    private GameObject endCube;
    private Bounds bounds;
    private Rigidbody rb;
    private Vector3 heading;
    //The transform of the gameobject that this control mesh is assigned to
    private Transform transform;

    public BezierControlMesh(Transform transform, Vector3 startPoint)
    {
        this.transform = transform;
        this.startPoint = startPoint;

        heading = new Vector3(0f, 0f, 0f);
        curveControl = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curveControl.GetComponent<MeshRenderer>().material.color = Color.white;

        endCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        endCube.GetComponent<MeshRenderer>().material.color = Color.black;

        controlCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //ControlCube has its postion set to 10 units in front of the ship
        controlCube.transform.position = startPoint + (transform.forward * 10);
        controlCube.GetComponent<MeshRenderer>().material.color = Color.red;
        controlCube.AddComponent<Drag>();

        controlMeshGameObject = new GameObject();
        lineRenderer = new GlLineRenderer(newMaterial());
    }

    private Material newMaterial()
    {
        Material lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
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

    public Vector3 GetHeading()
    {
        return controlCube.transform.position - startPoint;
    }

    public void BuildControlMesh()
    {
        Vector3 front = transform.position + startPoint;

        //We draw our control mesh segments with a distance of 1 between them
        //So we truncate the distance to be an integer value to simplify segment placement
        int intDistance = (int)Vector3.Distance(controlCube.transform.position, front);

        //the direction from the ship to the control cube
        Vector3 direction = (controlCube.transform.position - front).normalized;
        Vector3 endPoint = front + ((intDistance - 1) * direction);
        /*
         The bezier controll points straight out in front of the ship half the distance in that
         direction to the end point + half the lateral distance from the forward line of the ship to the end point
         this ensures that when the controll cube is at 45 degree angle to the front direction of the
         ship the control point is positioned such that it has an angle of 90 degrees bewtween start and end points
         but when the control cube is directly in front of the ship the controll point is halfway between
         it and the ship, this ensures a smooth curve without kinks
         */
        Vector3 curveControllPoint = new Vector3(front.x, (endPoint.y / 2) + Mathf.Sqrt(Mathf.Pow(endPoint.x - front.x, 2) + Mathf.Pow(endPoint.z - front.z, 2)) / 2, front.z);

        curveControl.transform.position = curveControllPoint;
        endCube.transform.position = endPoint;
        //set the control cube to follow this direction
        controlCube.transform.rotation = Quaternion.LookRotation(direction);
        //control mesh verticies one segment per  unit distance
        newVertices = new Vector3[4 * intDistance];
        int vOffset = 0;
        int prevOffset;
        int linesSize = 4 * intDistance * 2 + 4 * (intDistance - 1) * 2;
        linesList = new List<int>(linesSize);
        Vector3 segmentPosition = front;
        distances = new float[4 * intDistance];
        float maxDistance = 0;
        float minDistance = Mathf.Infinity;
        //Attempting to create a mesh object, to replace the existing object indicator
        for (int i = 0; i < intDistance; i++)
        {
            prevOffset = vOffset;
            vOffset = i * 4;
         
            float t = (float)i / (float)intDistance;
            if (t == 0)
            {
                t += 0.01f;
            }
            Vector3 bezierPoint = (Mathf.Pow(1f - t, 2) * front) + (2 * (1f - t) * t * curveControllPoint) + (Mathf.Pow(t, 2) * endPoint); 
            Vector3 bezierSlope = 2 * (1f - t) * (curveControllPoint - front) + 2 * t * (endPoint - curveControllPoint);
            controlMeshGameObject.transform.rotation = Quaternion.LookRotation(bezierSlope);

            newVertices[vOffset + 0] = bezierPoint + controlMeshGameObject.transform.up + controlMeshGameObject.transform.right;
            newVertices[vOffset + 1] = bezierPoint - controlMeshGameObject.transform.up + controlMeshGameObject.transform.right;
            newVertices[vOffset + 2] = bezierPoint - controlMeshGameObject.transform.up - controlMeshGameObject.transform.right;
            newVertices[vOffset + 3] = bezierPoint + controlMeshGameObject.transform.up - controlMeshGameObject.transform.right;

            for (int j = 0; j < 4; j++)
            {
                Vector3 currHeading = newVertices[vOffset + j] - Camera.main.gameObject.transform.position;
                float distance = Vector3.Dot(currHeading, Camera.main.gameObject.transform.forward);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
                else if (distance > maxDistance)
                {
                    maxDistance = distance;
                }
                distances[vOffset + j] = distance;
            }
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
        float delta = maxDistance - minDistance;
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = (distances[i] - minDistance) / delta;
        }
    }

    public void Render()
    {
        lineRenderer.renderLines(newVertices, linesList.ToArray(), distances);
    }
}
