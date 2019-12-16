using DG.Tweening;
using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class Ignition : MonoBehaviour
{
    [HideInInspector]
    public QuickParser quickParser;

    void Start()
    {
        quickParser = GetComponent<QuickParser>();

        //CommonObjs.Init();

        if (Application.isEditor == false)
        {
            GameObject.Find("Canvas").SetActive(false);
        }
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
        CommonObjs.gmlRootCellSpace.SetActive(show);
    }

    public void ToggleGeneralSpace(bool show)
    {
        CommonObjs.gmlRootGeneralSpace.SetActive(show);
    }

    public void ToggleTransitionSpace(bool show)
    {
        CommonObjs.gmlRootTransitionSpace.SetActive(show);
    }

    public void ToggleCellSpaceBoundary(bool show)
    {
        CommonObjs.gmlRootCellSpaceBoundary.SetActive(show);
    }

    public void ToggleState(bool show)
    {
        CommonObjs.gmlRootState.SetActive(show);
    }

    public void ToggleTransition(bool show)
    {
        CommonObjs.gmlRootTransition.SetActive(show);
    }

    public void ShortLoad(string fileURL)
    {
        GameObject.Destroy(GameObject.Find(CommonNames.ROOT));
        CommonObjs.Init();

        quickParser.Load(fileURL);

        UpdateStatesSize(0.3f);

        //Debug.Log(totalBounds.ToString());

        SendAndReceive.TreeToJSON();
    }

    private Vector3[] RecenterOfVectors(List<Vector3> vectors, Vector3 center)
    {
        Vector3[] reCenter = new Vector3[vectors.Count()];
        for (int i = 0; i < vectors.Count(); i++)
        {
            reCenter[i] = vectors[i] - center;
        }

        return reCenter;
    }

    private Vector3 CenterOfVectors(List<Vector3> vectors)
    {
        Vector3 sum = Vector3.zero;
        if (vectors == null || vectors.Count() == 0)
        {
            return sum;
        }

        foreach (Vector3 vec in vectors)
        {
            sum += vec;
        }
        return sum / vectors.Count();
    }

    public void UpdateStatesSize(float multiple)
    {
        int state_size = Convert.ToInt32(quickParser.GetUnitSize() * multiple);
        var states = GameObject.FindGameObjectsWithTag("TAG_STATE");
        foreach (var state in states)
        {
            state.transform.localScale = new Vector3(state_size, state_size, state_size);
        }
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(totalBounds.center, totalBounds.size);
    //}

    public void DoMoveViewPoint(int direction)
    {
        float frustrumLength = 0.0f;
        float distance = 0.0f;
        Vector3 resultPos = new Vector3();
        Vector3 resultRot = new Vector3();

        //switch (direction)
        //{
        //    case 1:
        //        resultRot = new Vector3(45, 45, 0);
        //        break;
        //    case 2:
        //        resultRot = new Vector3(0, 0, 0);
        //        break;
        //    case 3:
        //        resultRot = new Vector3(45, 315, 0);
        //        break;
        //    case 4:
        //        resultRot = new Vector3(0, 90, 0);
        //        break;
        //    case 5:
        //        resultRot = new Vector3(90, 0, 0);
        //        break;
        //    case 6:
        //        resultRot = new Vector3(0, 270, 0);
        //        break;
        //    case 7:
        //        resultRot = new Vector3(45, 135, 0);
        //        break;
        //    case 8:
        //        resultRot = new Vector3(0, 180, 0);
        //        break;
        //    case 9:
        //        resultRot = new Vector3(45, 225, 0);
        //        break;
        //    case 51:
        //        resultRot = new Vector3(90, 90, 0);
        //        break;
        //    case 52:
        //        resultRot = new Vector3(90, 180, 0);
        //        break;
        //    case 53:
        //        resultRot = new Vector3(90, 270, 0);
        //        break;
        //    default:
        //        break;
        //}


        switch (direction)
        {
            case 1:
                resultRot = new Vector3(0, -45, 45);
                break;
            case 2:
                resultRot = new Vector3(0, -90, 90);
                break;
            case 3:
                resultRot = new Vector3(0, -135, 45);
                break;
            case 4:
                resultRot = new Vector3(0, 0, 90);
                break;
            case 5:
                resultRot = new Vector3(0, -90, 0);
                break;
            case 6:
                resultRot = new Vector3(0, -180, 90);
                break;
            case 7:
                resultRot = new Vector3(0, 45, 45);
                break;
            case 8:
                resultRot = new Vector3(0, 90, 90);
                break;
            case 9:
                resultRot = new Vector3(0, 135, 45);
                break;
            case 51:
                resultRot = new Vector3(0, 0, 1);
                break;
            case 52:
                resultRot = new Vector3(0, 90, 1);
                break;
            case 53:
                resultRot = new Vector3(0, 180, 1);
                break;
            default:
                break;
        }

        frustrumLength = Math.Max(QuickParser.sceneBound.size.z, QuickParser.sceneBound.size.x);
        frustrumLength = Math.Max(frustrumLength, QuickParser.sceneBound.size.y);

        distance = frustrumLength * 0.5f / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        resultPos = QuickParser.sceneBound.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);

        //Camera.main.transform.DOMove(resultPos, 1);
        //Camera.main.transform.DORotate(resultRot, 1);
        GameObject.Find("Camera Orbit").transform.DORotate(resultRot, 1);
    }
    
    public void GeneralSpaceBackFaceCulling(bool isOn)
    {
        CommonObjs.materialGeneralSpace.shader = isOn ? CommonObjs.shaderCullON : CommonObjs.shaderCullOFF;
    }

    public void CellSpaceBackFaceCulling(bool isOn)
    {
        CommonObjs.materialCellSpace.shader = isOn ? CommonObjs.shaderCullON : CommonObjs.shaderCullOFF;
    }

    public void TransitionBackFaceCulling(bool isOn)
    {
        CommonObjs.materialTransitionSpace.shader = isOn ? CommonObjs.shaderCullON : CommonObjs.shaderCullOFF;
    }

    public void CellSpaceBoundaryBackFaceCulling(bool isOn)
    {
        CommonObjs.materialCellSpaceBoundary.shader = isOn ? CommonObjs.shaderCullON : CommonObjs.shaderCullOFF;
    }

    public Material GetMaterialByTag(string tag)
    {
        Material resultMat;

        if (tag.Equals("TAG_CELLSPACE"))
        {
            resultMat = CommonObjs.materialCellSpace;
        }
        else if (tag.Equals("TAG_GENERALSPACE"))
        {
            resultMat = CommonObjs.materialGeneralSpace;
        }
        else if (tag.Equals("TAG_TRANSITIONSPACE"))
        {
            resultMat = CommonObjs.materialTransitionSpace;
        }
        else
        {
            resultMat = CommonObjs.materialCellSpaceBoundary;
        }

        return resultMat;
    }
}
