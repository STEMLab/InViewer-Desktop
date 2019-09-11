using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

public class QuickParser : MonoBehaviour
{
    // In-Working..
    // 
    public bool ActivateFloorPlanTestFunc = false;
    private int totalFloors = 3;
    private Dictionary<int, int> floorMap;
    private List<List<Vector3>>[] floorOutLines;
    public int wannaPickHeight = -20;
    private int outLineFloor = -1;
    private GameObject[] floorRootObjs;
    //
    // In-Working..

    //Vector3 firstPos;
    double firstPosX;
    double firstPosY;
    double firstPosZ;

    public static Bounds sceneBound;

    static private List<List<Vector3>> outLines;

    private List<List<string>> LevelsFloorIDs;
    private string currentFileUrl;

    private XmlReader reader;

    private string _fileUrl;

    private Material lineMaterial;

    public string GetDirectoryName()
    {
        return Path.GetDirectoryName(_fileUrl);
    }

    private void Awake()
    {
        /////////////////////////
        // In-Working.
        // Test flight.
        //wannaPickHeight = -20;

        int[] heights = new int[]
        {
            -20,
            0,
            20,
        };

        int[] levels = new int[]
        {
            0,
            1,
            2
        };

        floorMap = new Dictionary<int, int>();

        totalFloors = heights.Count();
        floorRootObjs = new GameObject[totalFloors];

        for (int i = 0; i < totalFloors; i++)
        {
            floorMap.Add(heights[i], levels[i]);

            // 한개의 층에 2개 이상의 높이가 존재할 수도 있다고 가정
            GameObject preCreatedLevel = GameObject.Find((levels[i] + 1) + "F");
            if (preCreatedLevel == null)
            {
                preCreatedLevel = new GameObject((levels[i] + 1) + "F");
            }
            floorRootObjs[i] = preCreatedLevel;
        }

        floorOutLines = new List<List<Vector3>>[totalFloors];
        for (int i = 0; i < totalFloors; i++)
        {
            floorOutLines[i] = new List<List<Vector3>>();
        }

        floorMap.TryGetValue(wannaPickHeight, out outLineFloor);

        //if(floorMap.TryGetValue(wannaPickHeight, out outLineFloor) == false)
        //{
        //    outLineFloor = -1;
        //}

        // In-Working..
        /////////////////////////        
    }

    private void Start()
    {
        //if (Application.isEditor == false)
        //{
        //    GameObject.Find("Canvas").SetActive(false);
        //}

        //CommonObjs.Init();

        //materialTextureSurface = new Material(Shader.Find("Unlit/Texture"));

        //Load(@"E:\Data\Centralplaza_IndoorGML\CentralPlaza-texture.gml");
        //Load(@"E:\Data\Bldg313-F234-Naming\PNU_313_PSextension.gml");
        //CommonObjs.Init();
        //Load(@"D:\GitHub\Working\201.gml");
        //Load(@"D:\GitHub\Working\Centralplaza_IndoorGML\CentralPlaza-texture.gml");
        //Load(@"D:\GitHub\Working\Official\FJK-Haus_IndoorGML_withEXR-corrected_1_0_3.gml");
    }

    private Poly2Mesh.Polygon OnPolygon(XmlReader reader)
    {
        List<Vector3> localOutlines = new List<Vector3>();
        List<List<Vector3>> localHoles = new List<List<Vector3>>();

        while (isEndElement(reader, "Polygon") == false && isEndElement(reader, "PolygonPatch") == false)
        {
            reader.Read();

            if (isStartElement(reader, "exterior"))
            {
                // pos 목록을 localExterior에 모두 등록. exterior는 1개를 초과하지 않음.
                localOutlines = new List<Vector3>();
                while (isEndElement(reader, "exterior") == false)
                {
                    reader.Read();

                    if (isStartElement(reader, "pos"))
                    {
                        reader.Read();

                        Vector3 unityVector3d = GetPos3D(reader);

                        localOutlines.Add(unityVector3d);
                    }
                    else if(isStartElement(reader, "posList"))
                    {
                        reader.Read();
                        localOutlines = GetPosList3D(reader);
                    }
                }
            }

            if (isStartElement(reader, "interior"))
            {
                localHoles.Add(new List<Vector3>());
                while (isEndElement(reader, "interior") == false)
                {
                    reader.Read();
                    if (isStartElement(reader, "pos"))
                    {
                        reader.Read();

                        Vector3 unityVector3d = GetPos3D(reader);

                        localHoles.Last().Add(unityVector3d);
                    }
                    else if (isStartElement(reader, "posList"))
                    {
                        reader.Read();
                        //var lastHole = localHoles.Last();
                        //lastHole = GetPosList3D(reader);
                        int lastHoleIdx = localHoles.Count;
                        localHoles[lastHoleIdx-1] = GetPosList3D(reader);
                    }
                }
            }
        }

        Poly2Mesh.Polygon polygon = new Poly2Mesh.Polygon();
        polygon.outside = localOutlines;
        polygon.holes = localHoles;

        outLines.Add(localOutlines);

        for (int i = 0; i < localHoles.Count(); i++)
        {
            outLines.Add(localHoles[i]);
        }

        return polygon;
    }

    private static Vector3 GetPos2D(XmlReader reader)
    {
        string[] gmlVector3d = reader.Value.Trim().Split(' ');
        Vector2 unityVector2d = new Vector2();

        // Unity3D Vector Style.
        float.TryParse(gmlVector3d[0], out unityVector2d.x);
        float.TryParse(gmlVector3d[1], out unityVector2d.y);
        return unityVector2d;
    }

    private List<Vector3> GetPosList3D(XmlReader reader)
    {
        string[] gmlVector3d = reader.Value.Trim().Split(' ');
        List<Vector3> vector3s = new List<Vector3>();
        for (int i = 0; i < gmlVector3d.Length; i += 3)
        {
            string[] vs = new string[3];
            vs[0] = gmlVector3d[i];
            vs[1] = gmlVector3d[i + 1];
            vs[2] = gmlVector3d[i + 2];

            vector3s.Add(GetPos3DCore(vs));
        }

        return vector3s;
    }

    private Vector3 GetPos3D(XmlReader reader)
    {
        string[] gmlVector3d = reader.Value.Trim().Split(' ');
        Vector3 relativeUnityVector3d = GetPos3DCore(gmlVector3d);

        return relativeUnityVector3d;
    }

    private Vector3 GetPos3DCore(string[] gmlVector3d)
    {
        //Vector3 unityVector3d = new Vector3();
        double unityVectorX;
        double unityVectorY;
        double unityVectorZ;

        // Unity3D Vector Style.
        double.TryParse(gmlVector3d[0], out unityVectorX);
        double.TryParse(gmlVector3d[1], out unityVectorZ);
        double.TryParse(gmlVector3d[2], out unityVectorY);

        if (firstPosX == 0 && firstPosY == 0 && firstPosZ == 0)
        {
            firstPosX = unityVectorX;
            firstPosY = unityVectorY;
            firstPosZ = unityVectorZ;
        }

        //epsg3857 의 경우는 추후 다시 검토할 것. 좌표계 원점을 다르게 적용해야 할수도 있다.

        //Vector3 relativeUnityVector3d = unityVector3d - firstPos;
        double deltaX = unityVectorX - firstPosX;
        double deltaY = unityVectorY - firstPosY;
        double deltaZ = unityVectorZ - firstPosZ;

        Vector3 scaledVector = new Vector3(Convert.ToSingle(deltaX),
            Convert.ToSingle(deltaY),
            Convert.ToSingle(deltaZ));
        
        sceneBound.Encapsulate(scaledVector);
        return scaledVector;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(sceneBound.center, sceneBound.size);
    }

    public bool isStartElement(XmlReader reader, string tag)
    {
        return reader.IsStartElement() && reader.LocalName.Equals(tag);
    }

    public bool isEndElement(XmlReader reader, string tag)
    {
        return reader.NodeType == XmlNodeType.EndElement && reader.LocalName.Equals(tag);
    }

    public void LoadOnlyFloor(int level = 0)
    {

    }

    public void Load(string fileUrl)
    {
        currentFileUrl = fileUrl;

        //firstPos = new Vector3();
        firstPosX = 0;
        firstPosY = 0;
        firstPosZ = 0;

        sceneBound = new Bounds();

        outLines = new List<List<Vector3>>();

        _fileUrl = fileUrl;

        PosBasedEntity Face = new PosBasedEntity("", "", DATA_TYPE.Undefined);
        
        Stack<string> tagStack = new Stack<string>();
        
        using (XmlReader reader = XmlReader.Create(_fileUrl))
        {
            while (reader.Read())
            {
                if(isStartElement(reader, "cellSpaceBoundaryMember"))
                {
                    OnCellSpaceBoundaryMember(reader);
                }

                if (isStartElement(reader, "cellSpaceMember"))
                {
                    OnCellSpace(reader);
                }

                if(isStartElement(reader, "stateMember"))
                {
                    OnState(reader);
                }

                if (isStartElement(reader, "transitionMember"))
                {
                    OnTransition(reader);
                }
            }
        }

        GetFloors();
    }

    public void GetFloors()
    {
        var rootFloor = CommonObjs.gmlRootFloor.transform;
        Dictionary<float, int> floorDict = new Dictionary<float, int>();

        // floor 루트 객체에서 모든 객체들에 대한 층높이 통계를 구한다.
        // 출현하는 층높이의 횟수를 저장하는 이유는 추후 데이터 분석을 위함. 놓치는 부분이 있다던지..
        for (int i=0; i< rootFloor.childCount; i++)
        {
            float thisHeight = Convert.ToSingle(rootFloor.GetChild(i).name.Split('_')[0]);
            if (floorDict.ContainsKey(thisHeight))
            {
                floorDict[thisHeight] += 1;
            } else {
                floorDict.Add(thisHeight, 1);
                
            }
        }

        // Stage-1. 통계 결과에 따라 층별 노드를 생성
        var floorNames = Enumerable.Range(1, floorDict.Count).Select(x => x + "F");
        var keys = floorDict.Keys.ToList();
        keys.Sort();

        // Floor 높이를 모두 받아서 정렬 후 Key Value 순으로 저장.
        // -20 1F
        // 0 2F
        // 20 3F

        //foreach (string floorName in floorNames)
        //{
        //    Debug.Log(floorName);
        //    GameObject thisFloor = new GameObject(floorName);           
        //}

        // Stage-2. 각 바닥 객체들의 부모위치를 변경한다
        for (int i = 0; i < rootFloor.childCount; i++)
        {
            var oneFloor = GameObject.Instantiate(rootFloor.GetChild(i));
            float thisHeight = Convert.ToSingle(oneFloor.name.Split('_')[0]);
            int realFloor = keys.FindIndex(x => Math.Abs(x - thisHeight) < 0.001f) + 1;

            oneFloor.transform.parent = GameObject.Find(realFloor + "F").transform;
            //Instanitate로 생성된 클론딱지를 제거함
            oneFloor.name = oneFloor.name.Split('_')[1].Replace("(Clone)", "");
        }

        Destroy(rootFloor.gameObject);

        // 여기까지 왔으면 각 층별로 1F, 2F, 3F 순서로 기존 Root 의 ID가 저장 완료.
        // 이 리스트를 가지고 외곽선을 포함한 정제된 순수 geometry를 걸러낸다.

        // Stage-3. 리스트를 확보하고 객체를 제거한다..
        LevelsFloorIDs = new List<List<string>>();

        for (int i = 1; i <= floorNames.Count(); i++)
        {
            List<string> localFloor = new List<string>();
            
            foreach(Transform floorName in GameObject.Find(i.ToString() + "F").transform)
            {
                localFloor.Add(floorName.name);
            }

            LevelsFloorIDs.Add(localFloor);

            // 향후 퍼포먼스 향상을 위해 앞서 객체의 복사도 생략할 수 있도록 함을 고려
            // 현재는 층별 선별이 올바른지를 눈으로 확인할 수 있도록 코드를 남겨둠
            // GameObject.Destroy(GameObject.Find(i.ToString() + "F"));
        }

        Debug.Log(LevelsFloorIDs.Count());
    }

    public void RegisterFloorName(string floorName)
    {
        GameObject thisFloor = new GameObject(floorName);
    }

    private void LoggerInt(int num)
    {
        Debug.Log("Debug: " + num);
    }
    
    public float GetUnitSize()
    {
        float localMax = Math.Max(sceneBound.size.z, sceneBound.size.x);
        localMax = Math.Max(localMax, sceneBound.size.y);

        return localMax / 100f;
    }

    private void OnTransition(XmlReader reader)
    {
        string localName = string.Empty;
        List<Vector3> localLineString = new List<Vector3>();
        while (isEndElement(reader, "transitionMember") == false)
        {
            reader.Read();
            if (string.IsNullOrWhiteSpace(localName))
            {
                reader.Read();
                localName = reader.GetAttribute("gml:id");
            }

            if (isStartElement(reader, "pos"))
            {
                reader.Read();

                Vector3 unityVector3d = GetPos3D(reader);
                localLineString.Add(unityVector3d);
            }
            else if (isStartElement(reader, "posList"))
            {
                reader.Read();
                localLineString = GetPosList3D(reader);
            }
        }

        GameObject transition = new GameObject();

        var lineRenderer = transition.AddComponent<LineRenderer>();
        lineRenderer.positionCount = localLineString.Count();
        lineRenderer.SetPositions(localLineString.ToArray());
        lineRenderer.useWorldSpace = false;
        lineRenderer.tag = CommonObjs.TAG_TRANSITION;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = GetUnitSize() * 0.2f;
        lineRenderer.endWidth = GetUnitSize() * 0.1f;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.green;

        transition.name = localName;
        transition.transform.parent = CommonObjs.gmlRootTransition.transform;
    }

    public void TakeFloorPlan()
    {
        StartCoroutine(ScreenShotWithoutUI());
    }

    IEnumerator ScreenShotWithoutUI()
    {
        yield return null;
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;

        yield return new WaitForEndOfFrame();
        UnityEngine.ScreenCapture.CaptureScreenshot(string.Format(@"D:\FloorPlan_{0}_{1}.jpg", Path.GetFileName(currentFileUrl), wannaPickHeight));

        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
    }

    private void OnState(XmlReader reader)
    {
        string localName = string.Empty;

        while (isEndElement(reader, "stateMember") == false)
        {
            reader.Read();
            if (string.IsNullOrWhiteSpace(localName))
            {
                reader.Read();
                localName = reader.GetAttribute("gml:id");
            }

            if (isStartElement(reader, "pos"))
            {
                reader.Read();

                Vector3 unityVector3d = GetPos3D(reader);

                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);

                //
                // FloorPlanWork
                var theText = new GameObject();
                var textMesh = theText.AddComponent<TextMesh>();
                var meshRenderer = theText.AddComponent<MeshRenderer>();
                theText.name = "id-" + localName;
                textMesh.text = localName;
                textMesh.fontSize = 50;
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.alignment = TextAlignment.Center;
                theText.transform.position = unityVector3d;
                theText.transform.Rotate(90, 0, 0);
                int parentIndex = 0;
                if (floorMap.TryGetValue(Convert.ToInt32(unityVector3d.y), out parentIndex) == true)
                {
                    theText.transform.parent = GameObject.Find(string.Format("{0}F", (parentIndex + 1))).transform;
                }
                // FloorPlanWork
                //

                sphere.transform.position = unityVector3d;
                sphere.name = localName;

                sphere.tag = CommonObjs.TAG_STATE;
                sphere.transform.parent = CommonObjs.gmlRootState.transform;
            }
        }
    }

    private void OnCellSpace(XmlReader reader)
    {
        GameObject solid = new GameObject();

        string localName = string.Empty;
        string localType = string.Empty;
        while (isEndElement(reader, "cellSpaceMember") == false)
        {
            reader.Read();

            if (string.IsNullOrWhiteSpace(localName))
            {
                reader.Read();
                localName = reader.GetAttribute("gml:id");
                localType = reader.LocalName;

                if(localType.Equals("TransitionSpace"))
                {
                    solid.tag = CommonObjs.TAG_TRANSITIONSPACE;
                    solid.transform.parent = CommonObjs.gmlRootTransitionSpace.transform;
                }
                else if (localType.Equals("GeneralSpace"))
                {
                    solid.tag = CommonObjs.TAG_GENERALSPACE;
                    solid.transform.parent = CommonObjs.gmlRootGeneralSpace.transform;

                }
                else
                {
                    solid.tag = CommonObjs.TAG_CELLSPACE;
                    solid.transform.parent = CommonObjs.gmlRootCellSpace.transform;
                }
            }

            if (isStartElement(reader, "Solid"))
            {
                int faceCnt = 1;
                solid.name = localName;
                reader.Read();

                float lowestHeight = float.MaxValue;
                string lowestFaceName = "";
                Poly2Mesh.Polygon floorPolygon = new Poly2Mesh.Polygon();

                while (isEndElement(reader, "Solid") == false)
                {
                    reader.Read();
                    if (isStartElement(reader, "Polygon") || isStartElement(reader, "PolygonPatch"))
                    {
                        Poly2Mesh.Polygon polygon = OnPolygon(reader);
                        GameObject genPolygon = Poly2Mesh.CreateGameObject(polygon);

                        genPolygon.name = string.Format("{0}_Face:{1}", localName, faceCnt++);
                        genPolygon.transform.parent = solid.transform;

                        // 각 솔리드에서 가장 낮은 면을 바닥으로 잡음.
                        // 조금은 기울어진 바닥이라 할지라도 잡아낼 수 있게끔한게 의도.
                        // 현재까지의 모든 데이터는 바닥이 모두 평면으로 되어 있음.
                        float thisMinY = polygon.outside.Average(v => v.y);
                        if (thisMinY < lowestHeight)
                        {
                            lowestHeight = thisMinY;
                            floorPolygon = polygon;
                            lowestFaceName = localName;

                            //if(floorIndex == -99)
                            //{
                            //    Debug.Log("Cannot filter something..");
                            //}
                        }
                        ApplyCellSpaceMaterial(localType, genPolygon);
                    }
                }
                int floorIndex = -99;
                int convertedHeight = Convert.ToInt32(lowestHeight);

                floorMap.TryGetValue(convertedHeight, out floorIndex);

                floorOutLines[floorIndex].Add(floorPolygon.outside);
                RegisterFloor(localType, lowestHeight, lowestFaceName, floorPolygon);
            }
        }
    }

    private static void RegisterFloor(string localType, float lowestHeight, string lowestFaceName, Poly2Mesh.Polygon floorPolygon)
    {
        GameObject floorObj = Poly2Mesh.CreateGameObject(floorPolygon);        
        floorObj.name = lowestHeight + "_" + lowestFaceName;

        // 하나의 솔리드에서 공간타입이 2개 이상 존재할 수는 없다
        ApplyFloorMaterial(localType, floorObj);
        //ApplyCellSpaceMaterial(localType, floorObj);

        floorObj.transform.parent = CommonObjs.gmlRootFloor.transform;
    }

    private static void ApplyFloorMaterial(string localType, GameObject genPolygon)
    {
        if (localType.Equals("TransitionSpace"))
        {
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialFloorTransitionSpace;
        }
        else if (localType.Equals("GeneralSpace"))
        {
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialFloorGeneralSpace;
        }
        else
        {
            // CellSpace (Default)
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialFloorCellSpace;
        }
    }

    private static void ApplyCellSpaceMaterial(string localType, GameObject genPolygon)
    {
        if (localType.Equals("TransitionSpace"))
        {
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialTransitionSpace;
        }
        else if (localType.Equals("GeneralSpace"))
        {
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialGeneralSpace;
        }
        else
        {
            // CellSpace (Default)
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialCellSpace;
        }
    }

    private void OnCellSpaceBoundaryMember(XmlReader reader)
    {
        List<Vector2> localUVs = new List<Vector2>();
        Poly2Mesh.Polygon localPolygon = new Poly2Mesh.Polygon();
        string localFileName = string.Empty;
        string localName = string.Empty;
        while (isEndElement(reader, "cellSpaceBoundaryMember") == false)
        {
            reader.Read();

            if (string.IsNullOrWhiteSpace(localName))
            {
                reader.Read();
                localName = reader.GetAttribute("gml:id");
                if(localName == "ROOM80-TEXTURE-FLOOR")
                {
                    Debug.Log("Hit");
                }
            }

            if (isStartElement(reader, "Polygon") || isStartElement(reader, "PolygonPatch"))
            {
                localPolygon = OnPolygon(reader);
            }

            if (isStartElement(reader, "TextureImage"))
            {
                reader.Read();
                localFileName = reader.Value;
            }

            if (isStartElement(reader, "TextureCoordinate"))
            {
                localUVs = new List<Vector2>();
                while (isEndElement(reader, "TextureCoordinate") == false)
                {
                    reader.Read();

                    if (isStartElement(reader, "pos"))
                    {
                        reader.Read();

                        localUVs.Add(GetPos2D(reader));
                    }
                }
            }
        }

        //tagStack.Pop();

        if (localUVs.Count() > 2)
        {
            localPolygon.outsideUVs = localUVs;
            localPolygon.holesUVs = new List<List<Vector2>>();
            localPolygon.outsideUVs.Reverse();
        }

        localPolygon.outside.Reverse();

        for (int i = 0; i < localPolygon.holes.Count(); i++)
        {
            localPolygon.holes[i].Reverse();
            localPolygon.holesUVs.Add(new List<Vector2>());
        }

        // Texture 구멍 무시.
        if (string.IsNullOrWhiteSpace(localFileName) == false)
        {
            localPolygon.holes = new List<List<Vector3>>();
            localPolygon.holesUVs = new List<List<Vector2>>();
        }

        GameObject cellSpaceBoundary = Poly2Mesh.CreateGameObject(localPolygon);
        cellSpaceBoundary.name = localName;

        if (string.IsNullOrWhiteSpace(localFileName))
        {
            // 일반 벽과 지오메트리 정보가 겹칠경우를 대비하여 앞쪽으로 조금 이동
            cellSpaceBoundary.transform.Translate(localPolygon.planeNormal * 0.01f);
        } else
        {
            //덱스쳐가 면별로 긴밀하게 붙어있는 경우 기둥사이가 보이는 현상이 발생하므로 적절히 수치를 조절하여 사용.
            //cellSpaceBoundary.transform.Translate(localPolygon.planeNormal * 0.005f);
        }

        if (string.IsNullOrWhiteSpace(localFileName) == false)
        {
            //Debug.Log(GetDirectoryName() + "\\" + localFileName);
            IEnumerator tmpRunner = ApplyTexture(cellSpaceBoundary, GetDirectoryName() + "\\" + localFileName);
            StartCoroutine(tmpRunner);
        } else
        {
            cellSpaceBoundary.GetComponent<Renderer>().material = CommonObjs.materialCellSpaceBoundary;
        }

        cellSpaceBoundary.tag = CommonObjs.TAG_CELLSPACEBOUNDARY;
        cellSpaceBoundary.transform.parent = CommonObjs.gmlRootCellSpaceBoundary.transform;
    }

    IEnumerator ApplyTexture(GameObject target, string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        try
        {
            Texture myTexture = DownloadHandlerTexture.GetContent(www);
            target.GetComponent<Renderer>().material = CommonObjs.materialTextureSurface;
            target.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", myTexture);
        }
        catch
        {
            Debug.Log("ERROR File: " + url);
        }
    }

    //public void OnRenderObject()
    //{
    //    CreateLineMaterial();

    //    // Apply the line material
    //    lineMaterial.SetPass(0);

    //    if (outLines == null || outLines.Count() == 0)
    //        return;

    //    GL.PushMatrix();
    //    // Set transformation matrix for drawing to
    //    // match our transform
    //    GL.MultMatrix(transform.localToWorldMatrix);

    //    // Draw lines
    //    for (int i = 0; i < outLines.Count(); ++i)
    //    {
    //        GL.Begin(GL.LINES);
    //        for (int j = 0; j < outLines[i].Count() - 1; j++)
    //        {
    //            GL.Vertex3(outLines[i][j].x, outLines[i][j].y, outLines[i][j].z);
    //            GL.Vertex3(outLines[i][j + 1].x, outLines[i][j + 1].y, outLines[i][j + 1].z);
    //        }
    //        GL.End();
    //    }
    //    GL.PopMatrix();
    //}

    public void OnRenderObject()
    {
        if (outLines == null)
        {
            return;
        }

        CreateLineMaterial();

        List<List<Vector3>> outLineArray;

        if (ActivateFloorPlanTestFunc == true)
        {
            floorMap.TryGetValue(wannaPickHeight, out outLineFloor);

            outLineArray = floorOutLines[outLineFloor];
            if (CommonObjs.gmlRoot != null)
            {
                CommonObjs.gmlRoot.SetActive(false);
                for(int i=0;i <floorRootObjs.Count(); i++)
                {
                    if(outLineFloor != i)
                    {
                        floorRootObjs[i].SetActive(false);
                    } else
                    {
                        floorRootObjs[i].SetActive(true);
                    }
                }
            }
        }
        else
        {
            outLineArray = outLines;

            CommonObjs.gmlRoot.SetActive(true);
            for (int i = 0; i < floorRootObjs.Count(); i++)
            {
                floorRootObjs[i].SetActive(false);
            }
        }

        // Apply the line material
        lineMaterial.SetPass(0);

        if (floorOutLines == null || outLineArray.Count() == 0)
            return;

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        for (int i = 0; i < outLineArray.Count(); ++i)
        {
            GL.Begin(GL.LINES);
            for (int j = 0; j < outLineArray[i].Count() - 1; j++)
            {
                GL.Vertex3(outLineArray[i][j].x, outLineArray[i][j].y, outLineArray[i][j].z);
                GL.Vertex3(outLineArray[i][j + 1].x, outLineArray[i][j + 1].y, outLineArray[i][j + 1].z);
            }
            GL.End();
        }
        GL.PopMatrix();
    }

    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
            lineMaterial.color = new Color(0, 0, 0);
        }
    }
}
