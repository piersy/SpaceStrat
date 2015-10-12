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

    void LateUpdate()
    {
        //GameObject predictionMeshControlObject = new GameObject();

        Vector3 heading = bcm.GetLocalHeading();
        GameObject predictionMeshControlObject = Instantiate(gameObject);
        Destroy(predictionMeshControlObject.GetComponent<BezierControlMesh>());
        Rigidbody prb = predictionMeshControlObject.GetComponent<Rigidbody>();
        //Rigidbody prb = new Rigidbody();
        //need to set predictionmensh control object to match the ship and then fast forward
        //prb.transform.position = rb.transform.position;
        //prb.angularDrag = rb.angularDrag;
        //prb.angularVelocity = rb.angularVelocity;
        //prb.velocity = rb.velocity;
        //prb.drag = rb.drag;
        //prb.rotation = rb.rotation;
        //prb.mass = rb.mass;
        //prb.isKinematic = rb.isKinematic;
        for (int i = 0; i < 1; i++)
        {
            Vector3 frontOfShipWorld = prb.transform.TransformPoint(frontOfShip); 
            rb.AddForceAtPosition(prb.transform.right * heading.x, frontOfShipWorld, ForceMode.Force);
            rb.AddForceAtPosition(prb.transform.forward * heading.z, frontOfShipWorld, ForceMode.Force);
            rb.AddForce(prb.transform.up * heading.y, ForceMode.Force);
            GameObject g =  GameObject.CreatePrimitive(PrimitiveType.Cube);
            g.transform.position = prb.transform.position;
            g.transform.rotation = prb.transform.rotation;
        }
        Destroy(predictionMeshControlObject);
         
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
