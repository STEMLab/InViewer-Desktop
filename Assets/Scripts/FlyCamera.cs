using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class FlyCamera : MonoBehaviour
{
    /*
    Writen by Windexglow 11-13-10.  Use it, edit it, steal it I don't care.  
    Converted to C# 27-02-13 - no credit wanted.
    Simple flycam I made, since I couldn't find any others made public.  
    Made simple to use (drag and drop, done) for regular keyboard layout  
    wasd : basic movement
    shift : Makes camera accelerate
    space : Moves camera on X and Z axis only.  So camera doesn't gain any height*/

    float mainSpeed = 100.0f; //regular speed
    float shiftAdd = 250.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 1000.0f; //Maximum speed when holdin gshift
    float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    private bool moveMode = false;

    public Dropdown raycastOption;
    public Dropdown raycastResult;

    private string CurrentRayCastType()
    {
        int idx = raycastOption.GetComponent<Dropdown>().value;
        string rawString = raycastOption.GetComponent<Dropdown>().options[idx].text;
        string onlyValue = rawString.Split('-')[1].Trim().ToUpper().Replace(" ", string.Empty);
        return "TAG_" + onlyValue;

    }

    void Update()
    {
        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            moveMode = !moveMode;
            Cursor.visible = !moveMode;
        }

        if (Input.GetMouseButtonDown(1) && moveMode == false)
        {
            // RayCast.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit[] hits;

            Physics.queriesHitBackfaces = true;

            hits = Physics.RaycastAll(ray, Mathf.Infinity);

            //if (Physics.Raycast(ray, out hitObj, Mathf.Infinity))
            List<Dropdown.OptionData> tmpOptions = new List<Dropdown.OptionData>();
            //tmpOptions.Add(new Dropdown.OptionData("-- Results --"));

            //if (hits.Length > 0)
            {
                var openedList = raycastResult.transform.Find("Dropdown List");
                var blocker = GameObject.Find("Blocker");

                if (openedList != null)
                {
                    //openedList.gameObject.SetActive(false);
                    GameObject.Destroy(openedList.gameObject);

                    GameObject.Destroy(blocker);
                }

                raycastResult.ClearOptions();

                //tmpOptions.Add()
                //raycastResult.AddOptions("-- Results --");

                foreach (var hitObj in hits)
                {
                    var hitObjTag = hitObj.transform.tag;
                    var selectedRayTag = CurrentRayCastType();

                    string appendTag = string.Format(" [Type: {0}] ", hitObjTag.Split('_')[1]);

                    if (hitObjTag.Equals(selectedRayTag) 
                        || selectedRayTag.Equals("TAG_ALL") 
                        || selectedRayTag.Equals("TAG_CELLSPACE") && (hitObjTag.Equals("TAG_GENERALSPACE") || hitObjTag.Equals("TAG_TRANSITIONSPACE")))
                    {
                        if (hitObjTag.Equals("TAG_STATE"))
                        {
                            tmpOptions.Add(new Dropdown.OptionData(hitObj.transform.name + appendTag));
                        }
                        else
                        {
                            tmpOptions.Add(new Dropdown.OptionData(hitObj.transform.parent.name + appendTag));
                        }
                    }
                }
                raycastResult.options = tmpOptions;
            }
        }

        if (moveMode == false)
        {
            lastMouse = Input.mousePosition;
            return;
        }

        transform.eulerAngles = lastMouse;
        lastMouse = Input.mousePosition;
        //Mouse  camera angle done.  

        //Keyboard commands

        Vector3 p = GetBaseInput() * 0.1f;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            p = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }

        p = p * Time.deltaTime;
        Vector3 newPosition = transform.position;
        if (Input.GetKey(KeyCode.Space))
        { //If player wants to move on X and Z axis only
            transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else
        {
            transform.Translate(p);
        }

    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.Q))
        {
            p_Velocity += new Vector3(0, -1, 0);
        }
        if (Input.GetKey(KeyCode.E))
        {
            p_Velocity += new Vector3(0, 1, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }
}