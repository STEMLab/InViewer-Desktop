using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCenter : MonoBehaviour
{
    public GameObject CenterObject;


    void Update()
    {
        // Orbit center make
        if (Input.GetMouseButtonDown(1))
        {

            //RaycastHit hit = new RaycastHit();


            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if (Physics.Raycast(ray.origin, ray.direction, out hit))
            //{
            //    CenterObject.transform.position = hit.point;
            //}
        }
    }
}