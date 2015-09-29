using UnityEngine;

//using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour
{
    Rigidbody rb;
    bool paused = true;
    //Does the hard work of displaying the bezier wireframe controll
    BezierControlMesh bcm;
    Vector3 frontOfShip;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        frontOfShip = GetComponent<Renderer>().bounds.extents.y * transform.up;
        bcm = new BezierControlMesh(transform, frontOfShip);
    }

    void FixedUpdate()
    {
        if (paused)
            return;
        Vector3 heading = bcm.GetHeading();
        Debug.Log(heading);
        //rb.AddForce(transform.up * heading.y, ForceMode.Force);
        //This force here is not relative so we need to convert it to world space
        rb.AddForceAtPosition(transform.right * heading.x, transform.TransformPoint(frontOfShip), ForceMode.Force);
        //rb.AddForceAtPosition(transform.TransformPoint(transform.right) * heading.x, frontOfShip(),ForceMode.Force);
        GameObject endCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        endCube.GetComponent<MeshRenderer>().material.color = Color.black;
        endCube.transform.position = frontOfShip;
        //rb.AddForceAtPosition(transform.forward *heading.z, frontOfShip(),ForceMode.Force);
        // rb.AddForceAtPosition(transform.right * heading.x / 20, frontOfShip(), ForceMode.Force);
    }


    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            paused = !paused;
            if (paused)
                bcm.Activate();
            if (!paused)
                bcm.Deactivate();
        }
        if (!paused)
        {
            Time.timeScale = 1f; 
            return; 
        }
        Time.timeScale = 0f; 
        bcm.BuildControlMesh();
    }

    void  OnRenderObject()
    {    
        if (!paused)
            return;
        bcm.Render();
    }


}
