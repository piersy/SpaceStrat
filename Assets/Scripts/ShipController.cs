using UnityEngine;

//using System.Collections;
using System.Collections.Generic;

public class ShipController : PausableComponent
{
    Rigidbody rb;
    //Does the hard work of displaying the bezier wireframe controll
    BezierControlMesh bcm;
    Vector3 frontOfShip;
    Vector3 velocity;
    Vector3 angularVelocity;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        frontOfShip = GetComponent<Renderer>().bounds.extents.y * Vector3.up;
        bcm = gameObject.AddComponent<BezierControlMesh>();
        bcm.startPointLocal = frontOfShip;
    }

    void FixedUpdate()
    {
        if (!Game.paused)
        {
            Vector3 heading = bcm.GetLocalHeading();
            //rb.AddForce(transform.up * heading.y, ForceMode.Force);
            //This force here is not relative so we need to convert it to world space
            Vector3 frontOfShipWorld = transform.TransformPoint(frontOfShip); 
            rb.AddForceAtPosition(transform.right * heading.x, frontOfShipWorld, ForceMode.Force);
            rb.AddForceAtPosition(transform.forward * heading.z, frontOfShipWorld, ForceMode.Force);
            rb.AddForce(transform.up * heading.y, ForceMode.Force);

            //rb.AddForceAtPosition(transform.forward * heading.z, frontOfShipWorld, ForceMode.Force);
            //rb.AddForceAtPosition(transform.TransformPoint(transform.right) * heading.x, frontOfShip(),ForceMode.Force);
            GameObject endCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            endCube.GetComponent<MeshRenderer>().material.color = Color.black;
            endCube.transform.position = transform.TransformPoint(heading);
            endCube.gameObject.GetComponent<Collider>().enabled = false;
            //rb.AddForceAtPosition(transform.forward *heading.z, frontOfShip(),ForceMode.Force);
            // rb.AddForceAtPosition(transform.right * heading.x / 20, frontOfShip(), ForceMode.Force);
        }
    }

    protected override void OnPaused(bool paused)
    {
        if (paused)
        {
            velocity = rb.velocity;
            angularVelocity = rb.angularVelocity;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }

}
