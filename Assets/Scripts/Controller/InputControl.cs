using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControl : MonoBehaviour
{
    public GameObject cameraOrbit;

    public float rotateSpeed = 1.5f;
    public float panSpeed = 1.5f;

    private Vector3 lastPanningPositionDelta;

    private float originalOrbitScale;

    private void Awake()
    {
        originalOrbitScale = cameraOrbit.transform.localScale.x;
    }

    private float GetZoomRatio()
    {
        return cameraOrbit.transform.localScale.x / originalOrbitScale;
    }

    void Update()
    {
        // Panning camera action like what you think.
        if (Input.GetMouseButtonDown(1))
        {
            lastPanningPositionDelta = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 delta = Input.mousePosition - lastPanningPositionDelta;

            var y = Camera.main.transform.up * delta.y * panSpeed * GetZoomRatio();
            var x = Camera.main.transform.right * delta.x * panSpeed * GetZoomRatio();
            var tot = x + y;

            cameraOrbit.transform.transform.Translate(-tot, Space.World);

            lastPanningPositionDelta = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            float h = rotateSpeed * Input.GetAxis("Mouse X");
            float v = rotateSpeed * Input.GetAxis("Mouse Y");

            if (cameraOrbit.transform.eulerAngles.z + v <= 0.001f || cameraOrbit.transform.eulerAngles.z + v >= 179.999f)
                v = 0;

            cameraOrbit.transform.eulerAngles = new Vector3(cameraOrbit.transform.eulerAngles.x, cameraOrbit.transform.eulerAngles.y + h, cameraOrbit.transform.eulerAngles.z + v);
        }

        float scrollFactor = Input.GetAxis("Mouse ScrollWheel");

        if (Camera.main.orthographic == true)
        {
            Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * (1400 / 20);
            if (Camera.main.orthographicSize < 1)
            {
                Camera.main.orthographicSize = 1;
            }
        }
        else if (scrollFactor != 0)
        {
            cameraOrbit.transform.localScale = cameraOrbit.transform.localScale * (1f - scrollFactor);
        }
    }
}
