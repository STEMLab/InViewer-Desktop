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
    private GameObject gmlRoot;
    private GameObject gmlRootCellSpace;
    private GameObject gmlRootGeneralSpace;
    private GameObject gmlRootTransitionSpace;
    private GameObject gmlRootCellSpaceBoundary;
    private GameObject gmlRootState;
    private GameObject gmlRootTransition;

    public static Bounds totalBounds;

    private Material materialCellSpace;
    private Material materialGeneralSpace;
    private Material materialTransitionSpace;
    private Material materialCellSpaceBoundary;
    private Material materialState;
    private Shader shaderCullOFF;
    private Shader shaderCullON;

    private void Awake()
    {
        totalBounds = new Bounds();
    }

    void Start()
    {
    }

    public void OpenFileDialog()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "gml", false);
        if (paths.Length > 0)
        {
            ShortLoad(new System.Uri(paths[0]).AbsoluteUri);
        }
    }

    public void ToggleCellSpace(bool show)
    {
        gmlRootCellSpace.SetActive(show);
    }

    public void ToggleGeneralSpace(bool show)
    {
        gmlRootGeneralSpace.SetActive(show);
    }

    public void ToggleTransitionSpace(bool show)
    {
        gmlRootTransitionSpace.SetActive(show);
    }

    public void ToggleCellSpaceBoundary(bool show)
    {
        gmlRootCellSpaceBoundary.SetActive(show);
    }

    public void ToggleState(bool show)
    {
        gmlRootState.SetActive(show);
    }

    public void ToggleTransition(bool show)
    {
        gmlRootTransition.SetActive(show);
    }

    public void ShortLoad(string fileURL)
    {
        totalBounds = new Bounds();
        GameObject.Destroy(GameObject.Find(CommonNames.ROOT));

        shaderCullON = Shader.Find("Standard");
        shaderCullOFF = Resources.Load("Materials/STEM_CullOFF") as Shader;

        materialCellSpace = Resources.Load("Materials/CellSpace", typeof(Material)) as Material;
        materialGeneralSpace = Resources.Load("Materials/GeneralSpace", typeof(Material)) as Material;
        materialTransitionSpace = Resources.Load("Materials/TransitionSpace", typeof(Material)) as Material;
        materialCellSpaceBoundary = Resources.Load("Materials/CellSpaceBoundary", typeof(Material)) as Material;
        materialState = Resources.Load("Materials/State", typeof(Material)) as Material;

        materialCellSpace.shader = shaderCullOFF;
        materialGeneralSpace.shader = shaderCullOFF;
        materialTransitionSpace.shader = shaderCullOFF;
        materialCellSpaceBoundary.shader = shaderCullOFF;

        gmlRoot = new GameObject(CommonNames.ROOT);
        gmlRootCellSpace = new GameObject(CommonNames.ROOT_CELLSPACE);
        gmlRootGeneralSpace = new GameObject(CommonNames.ROOT_GENERALSPACE);
        gmlRootTransitionSpace = new GameObject(CommonNames.ROOT_TRANSITIONSPACE);
        gmlRootCellSpaceBoundary = new GameObject(CommonNames.ROOT_CELLSPACEBOUNDARY);
        gmlRootState = new GameObject(CommonNames.ROOT_STATE);
        gmlRootTransition = new GameObject(CommonNames.ROOT_TRANSITION);

        gmlRootCellSpace.transform.parent = gmlRoot.transform;
        gmlRootGeneralSpace.transform.parent = gmlRoot.transform;
        gmlRootTransitionSpace.transform.parent = gmlRoot.transform;
        gmlRootCellSpaceBoundary.transform.parent = gmlRoot.transform;
        gmlRootState.transform.parent = gmlRoot.transform;
        gmlRootTransition.transform.parent = gmlRoot.transform;

        SimpleParserIndoorGML parser = new SimpleParserIndoorGML(fileURL);

        parser.Load();

        var geometry3D = parser.PosBasedEntities.ToArray();

        int i = 1;

        string lastID = "";
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
                tmpTransition.useWorldSpace = false;
                tmpTransition.tag = "TAG_TRANSITION";
                tmpTransition.material = new Material(Shader.Find("Sprites/Default"));
                tmpTransition.startWidth = 0.1f;
                tmpTransition.endWidth = 0.01f;
                tmpTransition.startColor = Color.red;
                tmpTransition.endColor = Color.green;

                tmpTransition.positionCount = geom.vertices.Count();
                tmpTransition.SetPositions(geom.vertices.ToArray());
                tmpTransition.transform.parent = gmlRootTransition.transform;
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

            GameObject tmpObj;

            // CellSpaceBoundary의 경우에는 Face가 1개밖에 없으므로 트리구조를 단순화 함
            if (geom.spaceType == DATA_TYPE.CELLSPACEBOUNDARY)
            {
                tmpObj = Poly2Mesh.CreateGameObject(poly, geom.id);
            } else
            {
                tmpObj = Poly2Mesh.CreateGameObject(poly, string.Format("{0}_Face:{1}",lastID, i++));
            }

            tmpObj.AddComponent<MeshCollider>();
            tmpObj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            tmpObj.GetComponent<MeshRenderer>().receiveShadows = false;

            tmpObj.transform.parent = lastSolid.transform;

            if (geom.spaceType == DATA_TYPE.CELLSPACE)
            {
                tmpObj.GetComponent<Renderer>().material = materialCellSpace;
                tmpObj.tag = "TAG_CELLSPACE";
                lastSolid.tag = "TAG_CELLSPACE";
                lastSolid.transform.parent = gmlRootCellSpace.transform;
            }
            else if (geom.spaceType == DATA_TYPE.GENERALSPACE)
            {
                tmpObj.GetComponent<Renderer>().material = materialGeneralSpace;
                tmpObj.tag = "TAG_GENERALSPACE";
                lastSolid.tag = "TAG_GENERALSPACE";
                lastSolid.transform.parent = gmlRootGeneralSpace.transform;
            }
            else if (geom.spaceType == DATA_TYPE.TRANSITIONSPACE)
            {
                tmpObj.GetComponent<Renderer>().material = materialTransitionSpace;
                tmpObj.tag = "TAG_TRANSITIONSPACE";
                lastSolid.tag = "TAG_TRANSITIONSPACE";
                lastSolid.transform.parent = gmlRootTransitionSpace.transform;
            }
            else if (geom.spaceType == DATA_TYPE.CELLSPACEBOUNDARY)
            {
                tmpObj.GetComponent<Renderer>().material = materialCellSpaceBoundary;
                tmpObj.tag = "TAG_CELLSPACEBOUNDARY";
                lastSolid.tag = "TAG_CELLSPACEBOUNDARY";
                lastSolid.transform.parent = gmlRootCellSpaceBoundary.transform;
            }

            // 외곽선
            var tmpOutLine = tmpObj.AddComponent<LineRenderer>();
            tmpOutLine.useWorldSpace = false;
            tmpOutLine.material = new Material(Shader.Find("Sprites/Default"));
            tmpOutLine.startWidth = 0.1f;
            tmpOutLine.endWidth = 0.1f;
            tmpOutLine.startColor = Color.black;
            tmpOutLine.endColor = Color.black;

            tmpOutLine.positionCount = geom.vertices.Count();
            tmpOutLine.SetPositions(geom.vertices.ToArray());

            if (totalBounds == null || totalBounds.min.Equals(new Vector3(0, 0)))
            {
                totalBounds = new Bounds();
                totalBounds = tmpObj.GetComponent<MeshFilter>().mesh.bounds;
            }
            else
            {
                totalBounds.Encapsulate(tmpObj.GetComponent<MeshFilter>().mesh.bounds);
            }

            // CellSpaceBoundary 실제 객체 노드레벨을 하나 올리고 중복되는 빈 객체 삭제
            if (geom.spaceType == DATA_TYPE.CELLSPACEBOUNDARY)
            {
                Destroy(lastSolid);
                tmpObj.transform.parent = gmlRootCellSpaceBoundary.transform;
            }
        }

        UpdateStatesSize(1);

        Debug.Log(totalBounds.ToString());        

        SendAndReceive.TreeToJSON();
    }

    public void UpdateStatesSize(float multiple)
    {
        int state_size = Convert.ToInt32(GetUnitSize() * multiple);
        var states = GameObject.FindGameObjectsWithTag("TAG_STATE");
        foreach (var state in states)
        {
            state.transform.localScale = new Vector3(state_size, state_size, state_size);
        }
    }

    public static void DoMoveViewPoint(int direction)
    {
        float frustrumLength = 0.0f;
        float distance = 0.0f;
        Vector3 resultPos = new Vector3();
        Vector3 resultRot = new Vector3();

        switch (direction)
        {
            case 1:
                resultRot = new Vector3(45, 45, 0);
                break;
            case 2:
                resultRot = new Vector3(0, 0, 0);
                break;
            case 3:
                resultRot = new Vector3(45, 315, 0);
                break;
            case 4:
                resultRot = new Vector3(0, 90, 0);
                break;
            case 5:
                resultRot = new Vector3(90, 0, 0);
                break;
            case 6:
                resultRot = new Vector3(0, 270, 0);
                break;
            case 7:
                resultRot = new Vector3(45, 135, 0);
                break;
            case 8:
                resultRot = new Vector3(0, 180, 0);
                break;
            case 9:
                resultRot = new Vector3(45, 225, 0);
                break;
            case 51:
                resultRot = new Vector3(90, 90, 0);
                break;
            case 52:
                resultRot = new Vector3(90, 180, 0);
                break;
            case 53:
                resultRot = new Vector3(90, 270, 0);
                break;
            default:
                break;

        }

        frustrumLength = Math.Max(totalBounds.size.z, totalBounds.size.x);
        frustrumLength = Math.Max(frustrumLength, totalBounds.size.y);

        distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        resultPos = totalBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);

        Camera.main.transform.DOMove(resultPos, 1);
        Camera.main.transform.DORotate(resultRot, 1);
    }

    public float GetUnitSize()
    {
        float localMax = Math.Max(totalBounds.size.z, totalBounds.size.x);
        localMax = Math.Max(localMax, totalBounds.size.y);

        return localMax / 50f;
    }

    public void GeneralSpaceBackFaceCulling(bool isOn)
    {
        materialGeneralSpace.shader = isOn ? shaderCullON : shaderCullOFF;
    }

    public void CellSpaceBackFaceCulling(bool isOn)
    {
        materialCellSpace.shader = isOn ? shaderCullON : shaderCullOFF;
    }

    public void TransitionBackFaceCulling(bool isOn)
    {
        materialTransitionSpace.shader = isOn ? shaderCullON : shaderCullOFF;
    }

    public void CellSpaceBoundaryBackFaceCulling(bool isOn)
    {
        materialCellSpaceBoundary.shader = isOn ? shaderCullON : shaderCullOFF;
    }

    public Material GetMaterialByTag(string tag)
    {
        Material resultMat;

        if(tag.Equals("TAG_CELLSPACE"))
        {
            resultMat = materialCellSpace;
        }
        else if (tag.Equals("TAG_GENERALSPACE"))
        {
            resultMat = materialGeneralSpace;
        }
        else if (tag.Equals("TAG_TRANSITIONSPACE"))
        {
            resultMat = materialTransitionSpace;
        }
        else
        {
            resultMat = materialCellSpaceBoundary;
        }

        return resultMat;
    }
}
