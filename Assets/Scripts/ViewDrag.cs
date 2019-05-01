using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewDrag : MonoBehaviour
{
    public float turnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
    public float panSpeed = 4.0f;       // Speed of the camera when being panned
    public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth

    private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
    private bool isPanning;     // Is the camera being panned?
    private bool isRotating;    // Is the camera being rotated?
    private bool isZooming;		// Is the camera zooming?

    Vector3 hit_position = Vector3.zero;
    Vector3 current_position = Vector3.zero;
    Vector3 camera_position = Vector3.zero;

    //float ROTSpeed = 20;

    float GetWheelSpeed()
    {
        return System.Math.Max(QuickParser.sceneBound.size.z, QuickParser.sceneBound.size.x) / 20;
    }

    void Update()
    {
        var view = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        var isOutside = view.x < 0 || view.x > 1 || view.y < 0 || view.y > 1;

        if (isOutside)
        {
            return;
        }

        if (Input.GetMouseButtonDown(2))
        {
            mouseOrigin = Input.mousePosition;
            isRotating = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        if (Input.GetMouseButtonDown(1))
        {
            mouseOrigin = Input.mousePosition;
            isZooming = true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f) // forward
        {
            if (Camera.main.orthographic == true)
            {
                Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * GetWheelSpeed() * 3;
                if (Camera.main.orthographicSize < 1)
                {
                    Camera.main.orthographicSize = 1;
                }
            }
            else
            {
                Vector3 pos = Camera.main.ScreenPointToRay(Input.mousePosition).direction;
                Vector3 move = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * pos * GetWheelSpeed() * 3;
                transform.Translate(move, Space.World);
            }
        }

        // Disable movements on button release
        if (!Input.GetMouseButton(0)) isPanning = false;
        if (!Input.GetMouseButton(1)) isZooming = false;
        if (!Input.GetMouseButton(2)) isRotating = false;

        // Rotate camera along X and Y axis
        if (isRotating)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin).normalized * 0.1f;

            transform.RotateAround(transform.position, -transform.right, -pos.y * turnSpeed);
            transform.RotateAround(transform.position, -Vector3.up, pos.x * turnSpeed);
        }

        // Move the camera on it's XY plane
        if (isPanning)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin).normalized * 0.005f * GetWheelSpeed();

            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            transform.Translate(-move, Space.Self);
        }

        // Move the camera linearly along Z axis
        if (isZooming)
        {
            if (Camera.main.orthographic == true)
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin).normalized * 0.005f * GetWheelSpeed();
                Camera.main.orthographicSize -= pos.y * GetWheelSpeed();
                if (Camera.main.orthographicSize < 1)
                {
                    Camera.main.orthographicSize = 1;
                }
            }
            else
            {
                Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin).normalized * 0.001f * GetWheelSpeed();
                Vector3 move = pos.y * zoomSpeed * transform.forward;
                transform.Translate(move, Space.World);
            }
        }
    }
}