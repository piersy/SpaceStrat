using UnityEngine;

//using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour
{
    Rigidbody rb;
    bool paused = true;
    //Does the hard work of displaying the bezier wireframe controll
    BezierControlMesh bcm;

    void Start()
    {
        bcm = new BezierControlMesh(transform,frontOfShip());
        rb = GetComponent<Rigidbody>();
    }


    //Front of ship in local space
    Vector3 frontOfShip()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;
        return transform.up *  bounds.extents.y;
    }

    void FixedUpdate()
    {
        if (paused)
            return;
        Vector3 heading = bcm.GetHeading();
        Debug.Log( heading );
        //rb.AddForce(transform.up * heading.y, ForceMode.Force);
        rb.AddForceAtPosition(transform.right * heading.x, transform.TransformPoint(frontOfShip()),ForceMode.Force);
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
