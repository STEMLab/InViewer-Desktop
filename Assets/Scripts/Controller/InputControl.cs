using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

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
        // return cameraOrbit.transform.localScale.x / originalOrbitScale;
        return 1.0f;
    }

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        {
            if (Screen.width < mousePos.x || Screen.height < mousePos.y || mousePos.y < 0 || mousePos.x < 0 || Application.isFocused == false)
            {
                return;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hitData = Physics.RaycastAll(ray);

            List<string> resultList = new List<string>();

            StringBuilder sb = new StringBuilder(); ;
            foreach (var oneHit in hitData)
            {
                if(oneHit.transform.name.IndexOf("_Face") != -1)
                {
                    string targetID = oneHit.transform.name.Split('_')[0];
                    
                    if (resultList.Contains(targetID) == false)
                    {
                        resultList.Add(targetID);
                    }
                }
            }


            string sepRecord = "[#record#]";
            foreach (string oneTarget in resultList)
            {
                sb.Append(QuickParser.GetCellInfo(oneTarget));
                //sb.Append(oneTarget);
                sb.Append(sepRecord);
            }

            var accessPoint = GameObject.Find("_Main_").GetComponent<SendAndReceive>();

            try
            {
                accessPoint.SendToGUI($"HIT|{sb.ToString()}");
                //Debug.Log("All Result: " + sb.ToString());
                GameObject.Find("Text_SendTest").GetComponent<Text>().text = sb.ToString();
            }
            catch(Exception e)
            {
                GameObject.Find("Text_SendTest").GetComponent<Text>().text = e.Message;
            }
            
        }

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
            //if(Math.Abs(scrollFactor) < 0.2f)
            //{
            //    if (scrollFactor < 0)
            //    {
            //        scrollFactor = -0.2f;
            //    }
            //    else
            //    {
            //        scrollFactor = 0.2f;
            //    }
            //}
            //else if (Math.Abs(scrollFactor) > 0.5f)
            //{
            //        scrollFactor = 0.05f;
            //}

            //if(cameraOrbit.transform.localScale < 0.5f)
            //{
            //    cameraOrbit.transform.localScale = 0.5f;
            //}

            if(cameraOrbit.transform.localScale.z < 1f)
            {
                cameraOrbit.transform.localScale = new Vector3(1, 1, 1);
            }

            cameraOrbit.transform.localScale *= (1f - scrollFactor);
           
        }
    }
}
