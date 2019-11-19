using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO.Pipes;
using System.Security.Principal;
using System.ComponentModel;
using TMPro;
using System.Threading;
using System.Text;
using Newtonsoft.Json;
using DG.Tweening;
using System.IO;
using System.IO.Compression;

public class SendAndReceive : MonoBehaviour
{
    //public GameObject textObject;
    private Ignition mainFuncs;

    private BackgroundWorker backgroundWorkerReader;
    private BackgroundWorker backgroundWorkerWriter;

    private StreamString clientStream;
    private NamedPipeClientStream NPtoGUI;
    private NamedPipeClientStream NPtoUnity;

    private GameObject HighlightedObject = null;
    private Material materialSelected;

    private string textBuffer = "";

    private string dataFromGUI;

    void Start()
    {
        this.backgroundWorkerReader = new BackgroundWorker();
        this.backgroundWorkerWriter = new BackgroundWorker();

        this.backgroundWorkerReader.DoWork += new DoWorkEventHandler(backgroundWorker_Unity_Reader);
        this.backgroundWorkerReader.WorkerReportsProgress = true;
        this.backgroundWorkerReader.RunWorkerAsync();

        this.backgroundWorkerWriter.DoWork += new DoWorkEventHandler(backgroundWorker_Unity_Writer);
        this.backgroundWorkerWriter.WorkerReportsProgress = true;
        this.backgroundWorkerWriter.RunWorkerAsync();

        materialSelected = Resources.Load("Materials/SelectedObject", typeof(Material)) as Material;
        mainFuncs = GetComponent<Ignition>();
    }

    public void TestFuncB()
    {
        dataFromGUI = "HIDE|State";
    }

    void Update()
    {
        if (dataFromGUI != null)
        {
            string guiCmd = dataFromGUI.Split('|')[0].ToUpper().Trim();
            string guiParam = dataFromGUI.Split('|')[1].Trim();
            dataFromGUI = null;

            // Unity3D -> GUI 명령어 전송
            if(guiCmd.Equals("OPEN"))
            {
                mainFuncs.ShortLoad(guiParam);

                // GUI -> Unity3D 로 데이터 전송
                // 1. IndoorGML 객체 리스트를 확보
                // 2. 내부규칙에 따른 문자열 형태로 dataToGUI 생성
                // Root
                // ㄴCellSpace
                // ㄴGeneralSpace
                // ㄴCellSpaceBoundary
                // ㄴState
                // ㄴTransition
                //StringBuilder sb = new StringBuilder();
                mainFuncs.DoMoveViewPoint(1);
                // Unity3D에서 Tree갱신을 기다려 준다.
                StartCoroutine(DelayAndUpdateTree());
            }
            else if (guiCmd.Equals("SHOW"))
            {
                string[] showObjects = guiParam.Split(' ');

                if (showObjects.Length == 1 && (
                    showObjects[0].Equals(CommonNames.ROOT_CELLSPACE)
                    || showObjects[0].Equals(CommonNames.ROOT_CELLSPACEBOUNDARY)
                    || showObjects[0].Equals(CommonNames.ROOT_GENERALSPACE)
                    || showObjects[0].Equals(CommonNames.ROOT_STATE)
                    || showObjects[0].Equals(CommonNames.ROOT_TRANSITION)
                    || showObjects[0].Equals(CommonNames.ROOT_TRANSITIONSPACE)))
                {
                    var root = GameObject.Find(showObjects[0].TrimEnd()).transform;
                    for (int i = 0; i < root.childCount; i++)
                    {
                        ShowObject(root.GetChild(i).gameObject);
                    }
                }
                else
                {
                    foreach (string id in showObjects)
                    {
                        ShowObject(id);
                    }
                }
            }
            else if (guiCmd.Equals("HIDE"))
            {
                string[] hideObjects = guiParam.Split(' ');

                if (hideObjects.Length == 1 && (
                    hideObjects[0].Equals(CommonNames.ROOT_CELLSPACE)
                    || hideObjects[0].Equals(CommonNames.ROOT_CELLSPACEBOUNDARY)
                    || hideObjects[0].Equals(CommonNames.ROOT_GENERALSPACE)
                    || hideObjects[0].Equals(CommonNames.ROOT_STATE)
                    || hideObjects[0].Equals(CommonNames.ROOT_TRANSITION)
                    || hideObjects[0].Equals(CommonNames.ROOT_TRANSITIONSPACE)))
                {
                    var root = GameObject.Find(hideObjects[0].TrimEnd()).transform;
                    for (int i = 0; i < root.childCount; i++)
                    {
                        HideObject(root.GetChild(i).gameObject);
                    }
                }
                else
                {
                    foreach (string id in hideObjects)
                    {
                        HideObject(id);
                    }
                }
            }
            else if (guiCmd.Equals("GOTO"))
            {
                SelectObject(guiParam, true);
            }
            else if (guiCmd.Equals("SELECT"))
            {
                SelectObject(guiParam, false);
            }
            else if (guiCmd.Equals("VIEW"))
            {
                if (guiParam.Equals("ORTHOGONAL"))
                {
                    Camera.main.orthographic = true;
                    Camera.main.farClipPlane = 10000;
                    Camera.main.nearClipPlane = -10000;
                }
                else if(guiParam.Equals("PERSPECTIVE"))
                {
                    Camera.main.orthographic = false;
                    Camera.main.nearClipPlane = 0.3f;
                }
                else
                { 
                    mainFuncs.DoMoveViewPoint(Convert.ToInt32(guiParam));
                }
            }
            else if (guiCmd.Equals("STATE"))
            {
                mainFuncs.UpdateStatesSize(Convert.ToSingle(guiParam));
            }
            else if (guiCmd.Equals("CULLOFF"))
            {
                if(guiParam.Equals(CommonNames.ROOT_CELLSPACE))
                {
                    mainFuncs.CellSpaceBackFaceCulling(false);
                }
                else if (guiParam.Equals(CommonNames.ROOT_GENERALSPACE))
                {
                    mainFuncs.GeneralSpaceBackFaceCulling(false);
                }
                else if (guiParam.Equals(CommonNames.ROOT_TRANSITIONSPACE))
                {
                    mainFuncs.TransitionBackFaceCulling(false);
                }
                else if (guiParam.Equals(CommonNames.ROOT_CELLSPACEBOUNDARY))
                {
                    mainFuncs.CellSpaceBoundaryBackFaceCulling(false);
                }
            }
            else if (guiCmd.Equals("CULLON"))
            {
                if (guiParam.Equals(CommonNames.ROOT_CELLSPACE))
                {
                    mainFuncs.CellSpaceBackFaceCulling(true);
                }
                else if (guiParam.Equals(CommonNames.ROOT_GENERALSPACE))
                {
                    mainFuncs.GeneralSpaceBackFaceCulling(true);
                }
                else if (guiParam.Equals(CommonNames.ROOT_TRANSITIONSPACE))
                {
                    mainFuncs.TransitionBackFaceCulling(true);
                }
                else if (guiParam.Equals(CommonNames.ROOT_CELLSPACEBOUNDARY))
                {
                    mainFuncs.CellSpaceBoundaryBackFaceCulling(true);
                }
            }
        }
    }

    private IEnumerator DelayAndUpdateTree()
    {
        yield return new WaitForSecondsRealtime(1);

        SendToGUI(TreeToJSON());
    }

    public void HideObject(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0, 0, 0);
    }

    public void HideObject(string id)
    {
        var obj = GameObject.Find(id);
        HideObject(obj);
    }

    public void ShowObject(string id)
    {
        var obj = GameObject.Find(id);

        ShowObject(obj);
    }

    public void ShowObject(GameObject obj)
    {
        if (obj.transform.parent.name.Equals(CommonNames.ROOT_STATE))
        {
            float state_size = mainFuncs.quickParser.GetUnitSize();
            obj.transform.localScale = new Vector3(state_size, state_size, state_size);
        }
        else
        {
            obj.transform.localScale = new Vector3(1, 1, 1);
        }

    }

    public void SelectObject(string targetObjName, bool zoomAction)
    {
        // 최 상위 객체는 선택할 수 없음.. 클릭시 큰 의미 없으며 큰 데이터일경우 매우 느릴 수 있음.
        if (GameObject.Find(targetObjName).transform.parent.name.Equals(CommonNames.ROOT))
        {
            return;
        }

        // 선택된 객체의 영역을 구한다.
        Bounds myBounds = new Bounds();
        GameObject targetObj = GameObject.Find(targetObjName);

        // 자식이 있다면 직계손들의 범위만 인정.
        if (targetObj.transform.childCount > 0)
        {
            myBounds = targetObj.transform.GetChild(0).GetComponent<MeshFilter>().mesh.bounds;

            for (int i = 1; i < targetObj.transform.childCount; i++)
            {
                myBounds.Encapsulate(targetObj.transform.GetChild(i).GetComponent<MeshFilter>().mesh.bounds);
            }
        }

        if (targetObj.transform.childCount == 0)
        {
            myBounds = targetObj.transform.GetComponent<MeshFilter>().mesh.bounds;
        }

        var frustrumLength = Math.Max(myBounds.size.y, myBounds.size.x);
        frustrumLength = Math.Max(frustrumLength, myBounds.size.z);
        var distance = frustrumLength / Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);

        var resultRot = new Vector3(45, 225, 0);
        var resultPos = myBounds.center - distance * (Quaternion.Euler(resultRot) * Vector3.forward);
        
        if (targetObj.tag.Equals("TAG_STATE"))
        {
            resultPos = targetObj.transform.position - mainFuncs.quickParser.GetUnitSize() * 5 * (Quaternion.Euler(resultRot) * Vector3.forward);
        }

        // 선택시 하이라이트.
        // 1. 기존 하이라이트 객체를 원래대로 돌린다.
        if (HighlightedObject != null)
        {
            Material materialOriginal = mainFuncs.GetMaterialByTag(HighlightedObject.tag);

            if (HighlightedObject.transform.childCount > 0)
            {
                foreach (Transform face in HighlightedObject.transform)
                {
                    face.GetComponent<Renderer>().material = materialOriginal;
                }
            }
            else
            {
                HighlightedObject.GetComponent<Renderer>().material = materialOriginal;
            }
        }

        HighlightedObject = targetObj;

        // 2. 새로운 객체를 하이라이트화 시킨다.
        if (targetObj.transform.childCount > 0)
        {
            foreach (Transform face in targetObj.transform)
            {
                face.GetComponent<Renderer>().material = materialSelected;
            }
        }
        else
        {
            targetObj.GetComponent<Renderer>().material = materialSelected;
        }


        if (zoomAction)
        {
            //Camera.main.transform.DOMove(resultPos, 1);
            //Camera.main.transform.DORotate(resultRot, 1);

            GameObject.Find("Camera Orbit").transform.position = resultPos;
            GameObject.Find("Camera Orbit").transform.localScale = new Vector3(50, 50, 50);

        }
    }

    public static string TreeToJSON()
    {
        CommonTree currentTree = new CommonTree();

        GameObject tmpObject = GameObject.Find(CommonNames.ROOT_CELLSPACE);
        int cntObject = tmpObject.transform.childCount;
        currentTree.ROOT_CELLSPACE = new string[cntObject];
        currentTree.ROOT_CELLSPACE_FACES_CNT = new int[cntObject];
        for (int i = 0; i < cntObject; i++)
        {
            currentTree.ROOT_CELLSPACE[i] = tmpObject.transform.GetChild(i).name;
            currentTree.ROOT_CELLSPACE_FACES_CNT[i] = tmpObject.transform.GetChild(i).childCount;
        }

        tmpObject = GameObject.Find(CommonNames.ROOT_GENERALSPACE);
        cntObject = tmpObject.transform.childCount;
        currentTree.ROOT_GENERALSPACE = new string[cntObject];
        currentTree.ROOT_GENERALSPACE_FACES_CNT = new int[cntObject];
        for (int i = 0; i < cntObject; i++)
        {
            currentTree.ROOT_GENERALSPACE[i] = tmpObject.transform.GetChild(i).name;
            currentTree.ROOT_GENERALSPACE_FACES_CNT[i] = tmpObject.transform.GetChild(i).childCount;

        }

        tmpObject = GameObject.Find(CommonNames.ROOT_TRANSITIONSPACE);
        cntObject = tmpObject.transform.childCount;
        currentTree.ROOT_TRANSITIONSPACE = new string[cntObject];
        currentTree.ROOT_TRANSITIONSPACEFACES_CNT = new int[cntObject];
        for (int i = 0; i < cntObject; i++)
        {
            currentTree.ROOT_TRANSITIONSPACE[i] = tmpObject.transform.GetChild(i).name;
            currentTree.ROOT_TRANSITIONSPACEFACES_CNT[i] = tmpObject.transform.GetChild(i).childCount;
        }

        tmpObject = GameObject.Find(CommonNames.ROOT_CELLSPACEBOUNDARY);
        cntObject = tmpObject.transform.childCount;
        currentTree.ROOT_CELLSPACEBOUNDARY = new string[cntObject];
        for (int i = 0; i < cntObject; i++)
        {
            currentTree.ROOT_CELLSPACEBOUNDARY[i] = tmpObject.transform.GetChild(i).name;
        }

        tmpObject = GameObject.Find(CommonNames.ROOT_STATE);
        cntObject = tmpObject.transform.childCount;
        currentTree.ROOT_STATE = new string[cntObject];
        for (int i = 0; i < cntObject; i++)
        {
            currentTree.ROOT_STATE[i] = tmpObject.transform.GetChild(i).name;
        }

        tmpObject = GameObject.Find(CommonNames.ROOT_TRANSITION);
        cntObject = tmpObject.transform.childCount;
        currentTree.ROOT_TRANSITION = new string[cntObject];
        for (int i = 0; i < cntObject; i++)
        {
            currentTree.ROOT_TRANSITION[i] = tmpObject.transform.GetChild(i).name;
        }

        string result = JsonConvert.SerializeObject(currentTree);

        return result;
    }

    private void backgroundWorker_Unity_Writer(object sender, DoWorkEventArgs e)
    {
        try
        {
            NPtoGUI = new NamedPipeClientStream(".", "NPtoGUI", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
            NPtoGUI.Connect();

            clientStream = new StreamString(NPtoGUI);
        }
        catch (Exception ex)
        {
            //updateTextBuffer(ex.Message + Environment.NewLine + ex.StackTrace.ToString() + Environment.NewLine + "Last Message was: " + textBuffer);
        }
    }

    private void SendToGUI(string msg)
    {
        byte[] sendBytes = Encoding.ASCII.GetBytes(msg);

        int msgLength = sendBytes.Length;

        clientStream.WriteString(msgLength.ToString());

        NPtoGUI.Write(sendBytes, 0, msgLength);
        NPtoGUI.Flush();
    }

    private void backgroundWorker_Unity_Reader(object sender, DoWorkEventArgs e)
    {
        try
        {
            NPtoUnity = new NamedPipeClientStream(".", "NPtoUnity", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.None);
            NPtoUnity.Connect();

            StreamString clientStream = new StreamString(NPtoUnity);

            while(true)
            {
                dataFromGUI = clientStream.ReadString();
            }
        }
        catch (Exception ex)
        {
            //updateTextBuffer(ex.Message + Environment.NewLine + ex.StackTrace.ToString() + Environment.NewLine + "Last Message was: " + textBuffer);
        }
    }

    //private void updateTextBuffer(string text)
    //{
    //    lock (textBuffer)
    //    {
    //        textBuffer = textBuffer + Environment.NewLine + text;
    //        textBuffer = text;
    //        textObject.GetComponent<TextMeshProUGUI>().text = textBuffer;
    //    }
    //}

    public void TestFunc(string param)
    {
        dataFromGUI = "GOTO|" + param;
    }
}