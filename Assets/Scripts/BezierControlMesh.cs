using UnityEngine;

//using System.Collections;
using System.Collections.Generic;

public class BezierControlMesh : PausableComponent
{
    //The list that holds all the lines
    private List<int> linesList;
    //Cube that sits in front of the ship and can be dragged by the player in order to control the ship
    private GameObject controlCube;
    //Game object that is used to help construct the control mesh
    private GameObject controlMeshGameObject;
    //The point form which the mesh starts, this shoudl be in local space of the transform
    public Vector3 startPointLocal;
    Vector3[] newVertices;
    float[] distances;
    GlLineRenderer lineRenderer;
    GameObject curveControl;
    GameObject endCube;
    Bounds bounds;
    Rigidbody rb;
    //Required to pass back to the ship controller so that it can be manipulated according to the transform
    private Vector3 localHeading;
    delegate void PerformCalculation(int x, int y);
    //The transform of the gameobject that this control mesh is assigned to
    void Start()
    {
        //Set the initial heading it should be out in front of the object
        localHeading = Vector3.up * 10;
        curveControl = GameObject.CreatePrimitive(PrimitiveType.Cube);
        curveControl.GetComponent<MeshRenderer>().material.color = Color.white;

        endCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        endCube.GetComponent<MeshRenderer>().material.color = Color.black;

        controlCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        controlCube.GetComponent<MeshRenderer>().material.color = Color.red;
        controlCube.AddComponent<Drag>();
        controlCube.transform.position = transform.TransformPoint(startPointLocal) + transform.TransformDirection(localHeading);

        controlMeshGameObject = new GameObject();
        lineRenderer = new GlLineRenderer(newMaterial());
    }

    protected override void OnPaused(bool paused)
    {

        if (paused)
        {
            controlCube.transform.position = transform.TransformPoint(startPointLocal) + transform.TransformDirection(localHeading);
        }
        else
        {
            linesList = new List<int>();
        }
        this.enabled = paused;
        controlCube.SetActive(paused);
        curveControl.SetActive(paused);
        endCube.SetActive(paused);
        controlMeshGameObject.SetActive(paused);
    }
    
    Material newMaterial()
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

    public Vector3 GetLocalHeading()
    {
        return localHeading;
    }

    void LateUpdate()
    {
        //Find the start point of the controller in world space, get the diff between that and
        //ControlCube location and then convert tht to a local direction
        Vector3 startPoint = transform.TransformPoint(startPointLocal);
        localHeading = transform.InverseTransformDirection(controlCube.transform.position - startPoint);
        //So we truncate the distance to be an integer value to simplify segment placement
        int intDistance = (int)localHeading.magnitude;

        //the direction from the ship to the control cube
        Vector3 endPointLocal = startPointLocal + ((intDistance - 1) * localHeading.normalized);
        //the below is all wrong and only works if we are pointing up
        /*
         The bezier controll points straight out in front of the ship half the distance in that
         direction to the end point + half the lateral distance from the forward line of the ship to the end point
         this ensures that when the controll cube is at 45 degree angle to the front direction of the
         ship the control point is positioned such that it has an angle of 90 degrees bewtween start and end points
         but when the control cube is directly in front of the ship the controll point is halfway between
         it and the ship, this ensures a smooth curve without kinks
         */
        //Vector3 curveControllPoint = new Vector3(startPointLocal.x, (endPoint.y / 2) + Mathf.Sqrt(Mathf.Pow(endPoint.x - startPoint.x, 2) + Mathf.Pow(endPoint.z - startPoint.z, 2)) / 2, startPoint.z);
        Vector3 curveControllPointLocal = new Vector3(startPointLocal.x, (endPointLocal.y / 2) + Mathf.Sqrt(Mathf.Pow(endPointLocal.x - startPointLocal.x, 2) + Mathf.Pow(endPointLocal.z - startPointLocal.z, 2)) / 2, startPointLocal.z);
        //Vector3 curveControllPointLocal = startPointLocal + (endPointLocal - startPointLocal) / 2f;

        Vector3 endPoint = transform.TransformPoint(endPointLocal);
        Vector3 curveControllPoint = transform.TransformPoint(curveControllPointLocal);


        curveControl.transform.position = curveControllPoint;
        endCube.transform.position = transform.TransformPoint(endPointLocal);
        //set the control cube to follow this direction
        controlCube.transform.rotation = Quaternion.LookRotation(transform.TransformPoint(localHeading));
        //control mesh verticies one segment per  unit distance
        newVertices = new Vector3[4 * intDistance];
        int vOffset = 0;
        int prevOffset;
        int linesSize = 4 * intDistance * 2 + 4 * (intDistance - 1) * 2;
        linesList = new List<int>(linesSize);
        Vector3 segmentPosition = startPoint;
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
            Vector3 bezierPoint = (Mathf.Pow(1f - t, 2) * startPoint) + (2 * (1f - t) * t * curveControllPoint) + (Mathf.Pow(t, 2) * endPoint); 
            Vector3 bezierSlope = 2 * (1f - t) * (curveControllPoint - startPoint) + 2 * t * (endPoint - curveControllPoint);
            controlMeshGameObject.transform.rotation = Quaternion.LookRotation(bezierSlope, transform.up);

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

    public void OnRenderObject()
    {
        lineRenderer.renderLines(newVertices, linesList.ToArray(), distances);
    }
}
