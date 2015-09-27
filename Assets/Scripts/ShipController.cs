using UnityEngine;

//using System.Collections;
using System.Collections.Generic;

public class ShipController : MonoBehaviour
{
    Rigidbody rb;
    bool paused=true;
    //Does the hard work of displaying the bezier wireframe controll
    BezierControlMesh bcm;

    void Start()
    {
        bcm = new BezierControlMesh(transform, frontOfShip());
        rb = GetComponent<Rigidbody>();
    }


    //Front of ship in local space
    Vector3 frontOfShip()
    {
        Bounds bounds = GetComponent<Renderer>().bounds;
        return transform.forward * bounds.extents.y;
    }

    void FixedUpdate()
    {
        if(paused) return;
        Vector3 heading = bcm.GetHeading();
        rb.AddForce(transform.forward*heading.y , ForceMode.Force);
       //rb.AddForceAtPosition(transform.right * heading.x/50f, frontOfShip(),ForceMode.Force);
       //rb.AddForceAtPosition(transform.right *heading.z, frontOfShip(),ForceMode.Force);
       rb.AddForceAtPosition(transform.right *heading.x/20, frontOfShip(),ForceMode.Force);
    }


    void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Space)) paused = !paused;
        if(!paused) {
            Time.timeScale = 1f; 
            return; 
        }else{
            Time.timeScale = 0f; 
        }
        bcm.BuildControlMesh();
    }

    void  OnRenderObject()
    {    
        if (!paused) return;
        bcm.Render();
    }


}
