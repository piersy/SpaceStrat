using UnityEngine;

[RequireComponent(typeof(MeshCollider))]

public class CenterCamera : MonoBehaviour
{
    
    void OnMouseDown()
    {
        Camera.main.transform.LookAt(transform.position, Camera.main.transform.up);
    }



    void OnMouseDrag()
    {
        float x = Input.GetAxis("Mouse X"); 
        float y = Input.GetAxis("Mouse Y");
        Camera.main.transform.RotateAround(transform.position, Camera.main.transform.up, x*5);
        Camera.main.transform.RotateAround(transform.position, Camera.main.transform.right, y*5);
        //Camera.main.transform.RotateAround(transform.position, transform.up, x);
    }


}
