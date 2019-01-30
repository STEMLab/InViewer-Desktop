using DG.Tweening;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Ignition : MonoBehaviour
{
    const string ROOT = "Root";
    const string ROOT_CELLSPACE = "CellSpace";
    const string ROOT_GENERALSPACE = "GeneralSpace";
    const string ROOT_TRANSITIONSPACE = "TransitionSpace";
    const string ROOT_CELLSPACEBOUNDARY = "CellSpaceBoundary";
    const string ROOT_STATE = "State";
    const string ROOT_TRANSITION = "Transition";

    private GameObject gmlRoot;
    private GameObject gmlRootCellSpace;
    private GameObject gmlRootGeneralSpace;
    private GameObject gmlRootTransitionSpace;
    private GameObject gmlRootCellSpaceBoundary;
    private GameObject gmlRootState;
    private GameObject gmlRootTransition;

    private static Bounds totalBounds;

    private void Awake()
    {
        totalBounds = new Bounds();
    }

    void Start()
    {
        //ShortLoad(@"D:\201.gml");
    }

    public void OpenFileDialog()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "gml", false);
        if (paths.Length > 0)
        {
            //Clean Objects
            GameObject[] IndoorGMLs = new GameObject[6];

            IndoorGMLs[0] = GameObject.Find(ROOT_CELLSPACE);
            IndoorGMLs[1] = GameObject.Find(ROOT_GENERALSPACE);
            IndoorGMLs[2] = GameObject.Find(ROOT_TRANSITIONSPACE);

            IndoorGMLs[3] = GameObject.Find(ROOT_CELLSPACEBOUNDARY);
            IndoorGMLs[4] = GameObject.Find(ROOT_STATE);
            IndoorGMLs[5] = GameObject.Find(ROOT_TRANSITION);

            foreach (var root in IndoorGMLs)
            {
                GameObject.Destroy(root);
            }

            totalBounds = new Bounds();
            
            GameObject.Destroy(GameObject.Find(ROOT));

            ShortLoad(new System.Uri(paths[0]).AbsoluteUri);

        }
    }

    public void ToggleCellSpace(bool show)
    {
        //GameObject.Find(ROOT_CELLSPACE).transform.localScale = show ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        gmlRootCellSpace.SetActive(show);
    }

    public void ToggleGeneralSpace(bool show)
    {
        //GameObject.Find(ROOT_GENERALSPACE).transform.localScale = show ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        gmlRootGeneralSpace.SetActive(show);
    }

    public void ToggleTransitionSpace(bool show)
    {
        //GameObject.Find(ROOT_TRANSITIONSPACE).transform.localScale = show ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        gmlRootTransitionSpace.SetActive(show);
    }

    public void ToggleCellSpaceBoundary(bool show)
    {
        //GameObject.Find(ROOT_CELLSPACEBOUNDARY).transform.localScale = show ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        gmlRootCellSpaceBoundary.SetActive(show);
    }

    public void ToggleState(bool show)
    {
        //GameObject.Find(ROOT_STATE).transform.localScale = show ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        gmlRootState.SetActive(show);
    }

    public void ToggleTransition(bool show)
    {
        //GameObject.Find(ROOT_TRANSITION).transform.localScale = show ? new Vector3(1, 1, 1) : new Vector3(0, 0, 0);
        gmlRootTransition.SetActive(show);
    }

    private void ShortLoad(string fileURL)
    {
        SimpleParserIndoorGML parser = new SimpleParserIndoorGML(fileURL);

        parser.Load();

        var geometry3D = parser.PosBasedEntities.ToArray();

        int i = 1;

        var materialCellSpace = Resources.Load("Materials/CellSpace", typeof(Material)) as Material;
        var materialGeneralSpace = Resources.Load("Materials/GeneralSpace", typeof(Material)) as Material;
        var materialTransitionSpace = Resources.Load("Materials/TransitionSpace", typeof(Material)) as Material;
        var materialCellSpaceBoundary = Resources.Load("Materials/CellSpaceBoundary", typeof(Material)) as Material;
        var materialState = Resources.Load("Materials/State", typeof(Material)) as Material;

        string lastID = "";
        gmlRoot = new GameObject(ROOT);
        gmlRootCellSpace = new GameObject(ROOT_CELLSPACE);
        gmlRootGeneralSpace = new GameObject(ROOT_GENERALSPACE);
        gmlRootTransitionSpace = new GameObject(ROOT_TRANSITIONSPACE);
        gmlRootCellSpaceBoundary = new GameObject(ROOT_CELLSPACEBOUNDARY);
        gmlRootState = new GameObject(ROOT_STATE);
        gmlRootTransition = new GameObject(ROOT_TRANSITION);

        

        gmlRootCellSpace.transform.parent = gmlRoot.transform;
        gmlRootGeneralSpace.transform.parent = gmlRoot.transform;
        gmlRootTransitionSpace.transform.parent = gmlRoot.transform;
        gmlRootCellSpaceBoundary.transform.parent = gmlRoot.transform;
        gmlRootState.transform.parent = gmlRoot.transform;
        gmlRootTransition.transform.parent = gmlRoot.transform;

        GameObject lastSolid = null;

        foreach (var geom in geometry3D)
        {

            if (lastID.Equals(geom.id) == false && geom.localName.Equals("Point") == false)
            {
                lastSolid = new GameObject(geom.id);
                lastID = geom.id;
                i = 1;
            }

            // Point기반 State
            if (geom.localName.Equals("Point"))
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = geom.id;
                sphere.tag = "TAG_STATE";
                sphere.transform.localScale = new Vector3(5, 5, 5);
                sphere.transform.position = geom.vertices[0];
                sphere.transform.parent = gmlRootState.transform;
                sphere.GetComponent<Renderer>().material = materialState;
                sphere.AddComponent<MeshCollider>();
                continue;
            }

            // LineString 기반....
            if (geom.localName.Equals("LineString"))
            {
                var tmpTransition = lastSolid.AddComponent<LineRenderer>();
                tmpTransition.tag = "TAG_TRANSITION";
                tmpTransition.material = new Material(Shader.Find("Sprites/Default"));
                tmpTransition.startWidth = 1f;
                tmpTransition.endWidth = 0.1f;
                tmpTransition.startColor = Color.red;
                tmpTransition.endColor = Color.green;

                //Debug.Log("OutLine Length: " + geom.vertices.Count());
                tmpTransition.positionCount = geom.vertices.Count();
                tmpTransition.SetPositions(geom.vertices.ToArray());
                tmpTransition.transform.parent = gmlRootTransition.transform;
                //tmpTransition.AddComponent<MeshCollider>();
                continue;
            }


            // 여기 이하는 폴리곤 기반. CellSpace, CellSpaceBoundary
            if (geom.localName.Equals("LinearRing") == false)
            {
                continue;
            }

            Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();

            poly.outside = geom.vertices;
            // 유니티는 시계방향이 앞면, IndoorGML은 반시계 방향이 앞면.
            poly.outside.Reverse();

            var tmpObj = Poly2Mesh.CreateGameObject(poly, string.Format("Face:{0}", i++));

            tmpObj.AddComponent<MeshCollider>();

            tmpObj.transform.parent = lastSolid.transform;

            if (geom.spaceType == DATA_TYPE.CELLSPACE)
            {
                tmpObj.GetComponent<Renderer>().material = materialCellSpace;
                tmpObj.tag = "TAG_CELLSPACE";
                lastSolid.transform.parent = gmlRootCellSpace.transform;
            }
            else if (geom.spaceType == DATA_TYPE.GENERALSPACE)
            {
                tmpObj.GetComponent<Renderer>().material = materialGeneralSpace;
                tmpObj.tag = "TAG_GENERALSPACE";
                lastSolid.transform.parent = gmlRootGeneralSpace.transform;

            }
            else if (geom.spaceType == DATA_TYPE.TRANSITIONSPACE)
            {
                tmpObj.GetComponent<Renderer>().material = materialTransitionSpace;
                tmpObj.tag = "TAG_TRANSITIONSPACE";
                lastSolid.transform.parent = gmlRootTransitionSpace.transform;
            }
            else if (geom.spaceType == DATA_TYPE.CELLSPACEBOUNDARY)
            {
                tmpObj.GetComponent<Renderer>().material = materialCellSpaceBoundary;
                tmpObj.tag = "TAG_CELLSPACEBOUNDARY";
                lastSolid.transform.parent = gmlRootCellSpaceBoundary.transform;
            }
            else if (geom.spaceType == DATA_TYPE.STATE)
            {
                // Does reach here???
                tmpObj.GetComponent<Renderer>().material = materialCellSpaceBoundary;
                tmpObj.tag = "TAG_STATE";
                lastSolid.transform.parent = gmlRootState.transform;
            }

            // 외곽선
            var tmpOutLine = tmpObj.AddComponent<LineRenderer>();
            tmpOutLine.material = new Material(Shader.Find("Sprites/Default"));
            tmpOutLine.startWidth = 0.1f;
            tmpOutLine.endWidth = 0.1f;
            tmpOutLine.startColor = Color.black;
            tmpOutLine.endColor = Color.black;

            //Debug.Log("OutLine Length: " + geom.vertices.Count());
            tmpOutLine.positionCount = geom.vertices.Count();
            tmpOutLine.SetPositions(geom.vertices.ToArray());

            if (totalBounds == null || totalBounds.min.Equals(new Vector3(0, 0)))
            {
                Debug.Log("First Time");
                totalBounds = new Bounds();
                totalBounds = tmpObj.GetComponent<MeshFilter>().mesh.bounds;
            }
            else
            {
                totalBounds.Encapsulate(tmpObj.GetComponent<MeshFilter>().mesh.bounds);
            }
        }

        Debug.Log(totalBounds.ToString());
        //QuarterView();
        //Camera.main.transform.position = GetViewPoint(2);
        //DoMoveViewPoint(9);
        //GameObject.Find(ROOT)
    }

    public void DoMoveViewPoint(int direction)
    {
        float frustrumLength = 0.0f;
        float distance = 0.0f;
        Vector3 resultPos = new Vector3();
        Vector3 resultRot = new Vector3();


        switch (direction)
        {
            case 1:
                resultRot = new Vector3(45, 45, 0);
                frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
                break;
            case 2:
                resultRot = new Vector3(0, 0, 0);
                frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
                break;
            case 3:
                resultRot = new Vector3(45, 315, 0);
                frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
                break;
            case 4:
                resultRot = new Vector3(0, 90, 0);
                frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
                break;
            case 5:
                resultRot = new Vector3(90, 0, 0);
                frustrumLength = Math.Max(totalBounds.size.y, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(90, 0, 0) * Vector3.forward);
                break;
            case 6:
                resultRot = new Vector3(0, 270, 0);
                frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
                break;
            case 7:
                resultRot = new Vector3(45, 135, 0);
                frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
                break;
            case 8:
                resultRot = new Vector3(0, 180, 0);
                frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
                break;
            case 9:
                resultRot = new Vector3(45, 225, 0);
                frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
                distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
                resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
                break;


            default:
                break;

        }

        Camera.main.transform.DOMove(resultPos, 1);
        Camera.main.transform.DORotate(resultRot, 1);


        //return resultPos;
        //Camera.main.transform.localRotation = Quaternion.Euler(90, 0, 0);

        //Camera.main.transform.position = totalBounds.center - distance * Camera.main.transform.forward;
    }

    private static void SideView()
    {
        var frustrumLength = Math.Max(totalBounds.size.y, totalBounds.size.x);
        frustrumLength = Math.Max(frustrumLength, totalBounds.size.z);

        var distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

        Camera.main.transform.localRotation = Quaternion.Euler(0, 90, 0);
        Camera.main.transform.position = totalBounds.center - distance * Camera.main.transform.forward;
    }

    private static void QuarterView()
    {
        var frustrumLength = Math.Max(totalBounds.size.y, totalBounds.size.x);
        frustrumLength = Math.Max(frustrumLength, totalBounds.size.z);

        var distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

        Camera.main.transform.localRotation = Quaternion.Euler(45, 45, 0);
        Camera.main.transform.position = totalBounds.center - distance * Camera.main.transform.forward;
    }

}
