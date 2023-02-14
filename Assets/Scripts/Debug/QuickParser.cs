using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class StoreyAndOutLine
{
    private string _storey;
    public List<Vector3> innerLines;

    public StoreyAndOutLine(string storey)
    {
        _storey = storey;
    }

    public string GetStorey()
    {
        return _storey;
    }
}

public class QuickParser : MonoBehaviour
{
    bool is2D = false;

    //Vector3 firstPos;
    //double firstPosX;
    //double firstPosY;
    //double firstPosZ;
    private double coordMinX = float.MaxValue;
    private double coordMinZ = float.MaxValue;
    private double coordMaxX = float.MinValue;
    private double coordMaxZ = float.MinValue;
    private double coordCenterX = 0;
    private double coordCenterZ = 0;
    private double longerAxisLength = 0;

    bool isPassPhase1 = false;

    public static Bounds sceneBound;

    //static private List<List<Vector3>> outLines;
    static private List<Vector3> solidCenters;
    static private List<string> solidIDs;
    static private List<Vector3> nodeCenters;
    static private List<string> nodeIDs;

    // 최단거리를 표시하기위한 FromTo 정보가 담긴 Transition
    // (15 -> 301) key:"15-301" => value:"edge-1"
    static public Dictionary<string, string> dictEdgeToTransitionNormal;

    // 현재 Node에 포함된 type값이 부정확하다.
    // CellSpace와 Node 서로 다르게 type값이 있을수 있으므로 CellSpace의 type값을 우선시 한다.
    // Transition에서 가르키는 Node가 실질적으로 어떤 space type을 알기 위해서는 node의 duality값을 저장해둬야 한다.
    static public Dictionary<string, string> dictNodeToCellID;
    // 엘리베이터는 별도로 관리한다.
    // Room=1, Corridor=2, Stair=3, Elevator=4, Door=5, Gate=6, Window=7
    // <gml:description>storey="2":type="4"</gml:description>
    // <gml:name>M_cityhall-tica_F2F2_MV_487</gml:name>

    // 엘리베이터 타입의 cellID를 저장. 결국 dictNodeToCellID를 통해 cellID가 이 배열에 있으면 엘리베이터 node라는 의미.
    static public List<string> listElvCell;


    // 빠른길로 강조하기위한 Transition 리스트. 아닌부분은 가시성을 떨어트릴 목적.
    static public List<string> fastestEdgeList;


    // 층 선택을 위한 참조표
    // -1: 모든 층
    // 0~n: 특정 층
    static public string selectedStorey = "";
    static public List<string> allStoreys;
    static public Dictionary<string, string> dictCellToStorey;
    static public Dictionary<string, string> dictIdToStorey;
    static public Dictionary<string, string> dictStateToStorey;
    static public Dictionary<string, string> dictTransitionToStorey;

    static private List<StoreyAndOutLine> outLines;

    // 요구사항 5
    static private Dictionary<string, string> dicName;
    static private Dictionary<string, string> dicStorey;
    static private Dictionary<string, string> dicType;
    static private Dictionary<string, string> dicDesc;

    // 요구사항 3
    static private GameObject selectedCell;

    GameObject[] allSolidIDObjs;
    GameObject[] allNodeIDObjs;

    private XmlReader reader;

    private string _fileUrl;

    private Material lineMaterial;

    public string GetDirectoryName()
    {
        return Path.GetDirectoryName(_fileUrl);
    }

    private void Start()
    {
        if (Application.isEditor == false)
        {
            GameObject.Find("Debug_Canvas").transform.localScale = Vector3.zero;
        }

        //CommonObjs.Init();

        CommonObjs.Init();

        string targetStartFile = @"D:\TestData\Sample.gml";

        // 개발자의 빠른 작업을 위한 자동 로드.
        if (File.Exists(targetStartFile))
        {
            //Load(targetStartFile);
        }
    }

    static List<Vector3> lastFacePos;
    private ProBuilderMesh OnPolygon_PB(XmlReader reader, string storey)
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

        StoreyAndOutLine tmpLines = new StoreyAndOutLine(storey);
        tmpLines.innerLines = localOutlines;
        outLines.Add(tmpLines);

        for (int i = 0; i < localHoles.Count(); i++)
        {
            StoreyAndOutLine tmpLineHoles = new StoreyAndOutLine(storey);
            tmpLineHoles.innerLines = localHoles[i];
            outLines.Add(tmpLineHoles);
        }
        //outLines.Add(localOutlines);
        //for (int i = 0; i < localHoles.Count(); i++)
        //{
        //    outLines.Add(localHoles[i]);
        //}

        var polygon_pb = ProBuilderMesh.Create();


        IList<IList<Vector3>> localHoles_pb = new List<IList<Vector3>>(localHoles);


        var newLocalOutlines = new List<Vector3>(localOutlines);
        // ProBuilderMesh 구조체에서 중복된 점이 있으면 안된다.
        if (newLocalOutlines.First() == newLocalOutlines.Last())
        {
            newLocalOutlines.Remove(newLocalOutlines.Last());
        }

        // 아래가 원본
        //polygon_pb.CreateShapeFromPolygon(localOutlines, 0, false, localHoles_pb);
        polygon_pb.CreateShapeFromPolygon(newLocalOutlines, 0, false);
        //polygon_pb.CreatePolygon(localOutlines, true);

        // 확정된 mesh에서 좌표가 잘 얻어지지 않는다..
        lastFacePos = localOutlines;
        return polygon_pb;

        //Poly2Mesh.Polygon polygon = new Poly2Mesh.Polygon();
        //polygon.outside = localOutlines;
        //polygon.holes = localHoles;

        //return polygon;
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
        //double.TryParse(gmlVector3d[0], out unityVectorX);
        //double.TryParse(gmlVector3d[1], out unityVectorZ);
        //double.TryParse(gmlVector3d[2], out unityVectorY);
        //double aa = Convert.ToDouble("14150098.000000");

        unityVectorX = Convert.ToDouble(gmlVector3d[0]);
        unityVectorZ = Convert.ToDouble(gmlVector3d[1]);
        unityVectorY = Convert.ToDouble(gmlVector3d[2]);

        if (isPassPhase1 == false)
        {
            coordMinX = unityVectorX < coordMinX ? unityVectorX : coordMinX;
            coordMinZ = unityVectorZ < coordMinZ ? unityVectorZ : coordMinZ;

            coordMaxX = unityVectorX > coordMaxX ? unityVectorX : coordMaxX;
            coordMaxZ = unityVectorZ > coordMaxZ ? unityVectorZ : coordMaxZ;

            coordCenterX = (coordMaxX - coordMinX) / 2.0f;
            coordCenterZ = (coordMaxZ - coordMinZ) / 2.0f;

            double lengthX = coordMaxX - coordMinX;
            double lengthZ = coordMaxZ - coordMinZ;

            longerAxisLength = lengthX > lengthZ ? System.Math.Abs(lengthX) : System.Math.Abs(lengthZ);

            // Nothing to do more.
            return Vector3.zero;
        }

        // Pass Phase2
        double scaledX = (unityVectorX - coordMinX) / (longerAxisLength / 1000f);
        double scaledY = unityVectorY / (longerAxisLength / 1000f);
        double scaledZ = (unityVectorZ - coordMinZ) / (longerAxisLength / 1000f);

        Vector3 scaledVector = new Vector3(Convert.ToSingle(scaledX),
            Convert.ToSingle(scaledY),
            Convert.ToSingle(scaledZ));
        
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

    public void Load(string fileUrl)
    {
        //GameObject.Find("Toggle_ID").GetComponent<Toggle>().isOn = true;

        //firstPos = new Vector3();
        //firstPosX = 0;
        //firstPosY = 0;
        //firstPosZ = 0;

        sceneBound = new Bounds();

        //outLines = new List<List<Vector3>>();
        outLines = new List<StoreyAndOutLine>();

        solidCenters = new List<Vector3>();
        solidIDs = new List<string>();
        nodeCenters = new List<Vector3>();
        nodeIDs = new List<string>();

        dicName = new Dictionary<string, string>();
        dicStorey = new Dictionary<string, string>();
        dicType = new Dictionary<string, string>();
        dicDesc = new Dictionary<string, string>();


        // 층선택용 데이터 ㅇㅇ
        dictCellToStorey = new Dictionary<string, string>();
        dictIdToStorey = new Dictionary<string, string>();
        dictStateToStorey = new Dictionary<string, string>();
        dictTransitionToStorey = new Dictionary<string, string>();

        // 길찾기용 데이터 ㅇㅇ
        dictEdgeToTransitionNormal = new Dictionary<string, string>();
        listElvCell = new List<string>();
        dictNodeToCellID = new Dictionary<string, string>();
        fastestEdgeList = new List<string>();

        selectedStorey = "";
        allStoreys = new List<string>();
        //outLines = new List< .... 외곽선 타입은 가장 마지막에 작업>


        if (allSolidIDObjs != null)
        {
            foreach (var oneSolidID in allSolidIDObjs)
            {
                GameObject.Destroy(oneSolidID);
            }

            //foreach (var oneNodeID in allNodeIDObjs)
            //{
            //    GameObject.Destroy(oneNodeID);
            //}
        }

        _fileUrl = fileUrl;

        PosBasedEntity Face = new PosBasedEntity("", "", DATA_TYPE.Undefined);
        
        Stack<string> tagStack = new Stack<string>();

        // Phases 1 - Get Min and Max coordinates of plane world
        isPassPhase1 = false;
        coordMinX = float.MaxValue;
        coordMinZ = float.MaxValue;
        coordMaxX = float.MinValue;
        coordMaxZ = float.MinValue;

        using (XmlReader reader = XmlReader.Create(_fileUrl))
        {
            while (reader.Read())
            {
                if(isStartElement(reader, "pos"))
                {
                    reader.Read();
                    string[] gmlVector3d = reader.Value.Trim().Split(' ');

                    GetPos3D(reader);
                }
                else if (isStartElement(reader, "posList"))
                {
                    reader.Read();
                    GetPosList3D(reader);
                }
            }
        }

        // Phases 1.5 - Storey 정보, CellSpace <-> NodeID 정보, 엘리베이터(type=4) 리스트 업데이트
        // 층 정보가 없는 경우의 수는 매우 다양하다.
        // (에러의 경우)현재까지 규정된 형태의 데이터가 없다는 뜻은 층 정보처리가 필요 없다고 가정함.
        //try
        {
            using (XmlReader reader = XmlReader.Create(_fileUrl))
            {
                while (reader.Read())
                {
                    if (isStartElement(reader, "cellSpaceMember"))
                    {
                        OnCellSpace(reader, true);
                    }

                    if (isStartElement(reader, "stateMember"))
                    {                        
                        OnState(reader, true);
                    }
                }
            }
        }
        //catch
        //{
        //    Debug.Log("Skip Phases 1.5");
        //}

        isPassPhase1 = true;
        // Phases 2 - Normalization Position All Geometries In (0, ?, 0) - (1000, ?, 1000)
        if(coordMinX == coordMaxX)
        {
            Debug.Log("Assert) Cannot recognize coordinates");
            return;
        }

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
                    OnCellSpace(reader, false);
                }

                if(isStartElement(reader, "stateMember"))
                {
                    OnState(reader, false);
                }

                if (isStartElement(reader, "transitionMember"))
                {
                    OnTransition(reader);
                }
            }
        }


        // 잦은 Find를 방지하고자 메모리에 등록
        // 3개의 목록이 한쌍이다. ID, Vector3, GameObject
        GameObject root_id = GameObject.Find("Root_ID");
        GameObject text_id = GameObject.Find("Text_ID");

        allSolidIDObjs = new GameObject[solidCenters.Count];
        for (int i = 0; i < solidCenters.Count; i++)
        {
            allSolidIDObjs[i] = GameObject.Instantiate(text_id);
            allSolidIDObjs[i].name = "s_" + solidIDs[i];
            allSolidIDObjs[i].transform.parent = root_id.transform;

            allSolidIDObjs[i].GetComponent<Text>().text = solidIDs[i];

            string tmpStorey;
            dictCellToStorey.TryGetValue(solidIDs[i], out tmpStorey);
            dictIdToStorey.Add(allSolidIDObjs[i].name, tmpStorey);
        }

        // 층 선택관련 작업
        GameObject.Find("Dropdown_storey").GetComponent<Dropdown>().options.Clear();

        GameObject.Find("Dropdown_storey").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("- All"));
        allStoreys.Sort();
        foreach(var oneStorey in allStoreys)
        {
            GameObject.Find("Dropdown_storey").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(oneStorey.ToString()));
        }

        //GetFloors();
    }

    public void GetFloors()
    {
        var rootFloor = CommonObjs.gmlRootFloor.transform;
        Dictionary<string, int> floorDict = new Dictionary<string, int>();

        // floor 루트 객체에서 모든 객체들에 대한 층높이 통계를 구한다.
        for (int i=0; i< rootFloor.childCount; i++)
        {
            string thisHeight = rootFloor.GetChild(i).name.Split('_')[0];
            if (floorDict.ContainsKey(thisHeight))
            {
                floorDict[thisHeight] += 1;
            } else {
                floorDict.Add(thisHeight, 1);
                
            }
        }

        // 통계 결과에 따라 층별 자식노드를 만들고 재분류를 실시
        //var floorNames = Enumerable.Range(1, floorDict.Count).Select(x => x + "F");
        
        //foreach (string floorName in floorNames)
        //{
        //    GameObject thisFloor = new GameObject(floorName);
        //}

        Debug.Log(floorDict);
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
        float localMax = System.Math.Max(sceneBound.size.z, sceneBound.size.x);
        localMax = System.Math.Max(localMax, sceneBound.size.y);

        return localMax / 100f;
    }

    private void OnTransition(XmlReader reader)
    {
        string from = string.Empty;
        string to = string.Empty;

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

            if (isStartElement(reader, "connects"))
            {
                string dest = reader.GetAttribute("xlink:href").Replace("#", "");
                if (string.IsNullOrEmpty(from))
                {
                    from = dest;
                }
                else
                {
                    to = dest;
                    try
                    {
                        // 양방향 이동 가능 가정
                        dictEdgeToTransitionNormal.Add(string.Format("{0}~{1}", from, to), localName);
                        dictEdgeToTransitionNormal.Add(string.Format("{0}~{1}", to, from), localName);
                    }
                    catch(Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                }
            }

            if (isStartElement(reader, "description"))
            {
                reader.Read();
                string trimedValue = reader.Value.Trim();

                string[] partOfDesc = trimedValue.Split(':');
                foreach (string oneDesc in partOfDesc)
                {
                    int startStr = oneDesc.IndexOf("storey");
                    if (startStr >= 0)
                    {
                        string storey = oneDesc.Replace("storey=", "");
                        storey = storey.Replace("\"", "");

                        dictTransitionToStorey.Add(localName, storey);

                        if (allStoreys.Contains(storey) == false)
                        {
                            allStoreys.Add(storey);
                        }
                    }
                }
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

        transition.name = localName;

        //var lineRenderer = transition.AddComponent<LineRenderer>();
        //lineRenderer.positionCount = localLineString.Count();
        //lineRenderer.SetPositions(localLineString.ToArray());
        //lineRenderer.useWorldSpace = false;
        //lineRenderer.tag = CommonObjs.TAG_TRANSITION;
        //lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        //lineRenderer.startWidth = GetUnitSize() * 0.2f;
        //lineRenderer.endWidth = GetUnitSize() * 0.1f;
        //lineRenderer.startColor = Color.red;
        //lineRenderer.endColor = Color.green;


        // 요청사항 7. Transition 단색처리
        var lineRenderer = transition.AddComponent<LineRenderer>();
        lineRenderer.positionCount = localLineString.Count();
        lineRenderer.SetPositions(localLineString.ToArray());
        lineRenderer.useWorldSpace = false;
        lineRenderer.tag = CommonObjs.TAG_TRANSITION;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = GetUnitSize() * 0.15f;
        lineRenderer.endWidth = GetUnitSize() * 0.15f;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;




        transition.name = localName;
        transition.transform.parent = CommonObjs.gmlRootTransition.transform;
    }

    private void OnState(XmlReader reader, bool isStoreyUpdateOnly)
    {
        if (isStoreyUpdateOnly)
        {
            string localName = string.Empty;
            string localStorey = string.Empty;

            while (isEndElement(reader, "stateMember") == false)
            {
                reader.Read();
                if (string.IsNullOrWhiteSpace(localName))
                {
                    reader.Read();
                    localName = reader.GetAttribute("gml:id");
                }

                // 보통 state는 cellspace 정보가 다 끝난 후에 나온다.
                if (isStartElement(reader, "duality"))
                {
                    string duality = reader.GetAttribute("xlink:href").Replace("#", "");
                    string tmpStorey;
                    dictCellToStorey.TryGetValue(duality, out tmpStorey);
                    dictStateToStorey.Add(localName, tmpStorey);
                }
            }
        }
        else
        {
            string localName = string.Empty;
            string localStorey = string.Empty;

            while (isEndElement(reader, "stateMember") == false)
            {
                reader.Read();
                if (string.IsNullOrWhiteSpace(localName))
                {
                    reader.Read();
                    localName = reader.GetAttribute("gml:id");
                }

                // 보통 state는 cellspace 정보가 다 끝난 후에 나온다.
                if (isStartElement(reader, "duality"))
                {
                    string duality = reader.GetAttribute("xlink:href").Replace("#", "");
                    int tmpStorey;
                    //dictCellToStorey.TryGetValue(duality, out tmpStorey);
                    //dictStateToStorey.Add(localName, tmpStorey);
                }


                if (isStartElement(reader, "pos"))
                {
                    reader.Read();

                    Vector3 unityVector3d = GetPos3D(reader);

                    GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    sphere.transform.position = unityVector3d;
                    sphere.name = localName;

                    sphere.tag = CommonObjs.TAG_STATE;
                    sphere.transform.parent = CommonObjs.gmlRootState.transform;
                    sphere.transform.localScale = new Vector3(10, 10, 10);
                }
            }
        }
    }

    private void OnCellSpace(XmlReader reader, bool isStoreyUpdateOnly)
    {
        if (isStoreyUpdateOnly)
        {
            string duality = string.Empty;
            string localName = string.Empty;
            string localType = string.Empty;
            string localStorey = string.Empty;

            while (isEndElement(reader, "cellSpaceMember") == false)
            {
                reader.Read();

                if (string.IsNullOrWhiteSpace(localName))
                {
                    reader.Read();
                    localName = reader.GetAttribute("gml:id");
                    localType = reader.LocalName;
                }

                if (isStartElement(reader, "duality"))
                {
                    // xlink대신 nilReason="unknown" 으로 대치되는 경우가 있다.
                    try
                    {
                        duality = reader.GetAttribute("xlink:href").Replace("#", "");
                        dictNodeToCellID.Add(duality, localName);
                    }
                    catch
                    {
                        IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;
                        Debug.Log("Detected unknown duality: " + xmlInfo.LineNumber);
                    }
                    //dictCellToStorey.TryGetValue(duality, out tmpStorey);
                    //dictStateToStorey.Add(localName, tmpStorey);
                }


                if (isStartElement(reader, "description"))
                {
                    reader.Read();
                    string trimedValue = reader.Value.Trim();
                    //dicDesc.Add(localName, trimedValue);


                    string[] partOfDesc = trimedValue.Split(':');
                    foreach (string oneDesc in partOfDesc)
                    {
                        int startStr = oneDesc.IndexOf("storey");
                        if (startStr >= 0)
                        {
                            string storey = oneDesc.Replace("storey=", "");
                            storey = storey.Replace("\"", "");

                            dictCellToStorey.Add(localName, storey);

                            if (allStoreys.Contains(storey) == false)
                            {
                                allStoreys.Add(storey);
                            }
                        }

                        startStr = oneDesc.IndexOf("type");
                        if (startStr >= 0)
                        {
                            string cellType = oneDesc.Replace("type=", "");
                            cellType = cellType.Replace("\"", "");
                            int nCellType = Convert.ToInt32(cellType);

                            if(nCellType == 4)
                            {
                                listElvCell.Add(localName);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            GameObject solid = new GameObject();
            string duality = string.Empty;
            string localName = string.Empty;
            string localType = string.Empty;
            string localStorey = string.Empty;

            while (isEndElement(reader, "cellSpaceMember") == false)
            {
                reader.Read();

                if (string.IsNullOrWhiteSpace(localName))
                {
                    reader.Read();
                    localName = reader.GetAttribute("gml:id");
                    localType = reader.LocalName;

                    if (localType.ToLower().Equals("transitionspace"))
                    {
                        solid.tag = CommonObjs.TAG_TRANSITIONSPACE;
                        solid.transform.parent = CommonObjs.gmlRootTransitionSpace.transform;
                    }
                    else if (localType.ToLower().Equals("generalspace"))
                    {
                        solid.tag = CommonObjs.TAG_GENERALSPACE;
                        solid.transform.parent = CommonObjs.gmlRootGeneralSpace.transform;
                    }
                    else if (localType.ToLower().Equals("connectionspace"))
                    {
                        solid.tag = CommonObjs.TAG_CONNECTIONSPACE;
                        solid.transform.parent = CommonObjs.gmlRootConnectionSpace.transform;
                    }
                    else if (localType.ToLower().Equals("anchorspace"))
                    {
                        solid.tag = CommonObjs.TAG_ANCHORSPACE;
                        solid.transform.parent = CommonObjs.gmlRootAnchorSpace.transform;
                    }
                    else
                    {
                        solid.tag = CommonObjs.TAG_CELLSPACE;
                        solid.transform.parent = CommonObjs.gmlRootCellSpace.transform;
                    }
                }

                if (isStartElement(reader, "name"))
                {
                    reader.Read();
                    string trimedValue = reader.Value.Trim();

                    try
                    {
                        dicName.Add(localName, trimedValue);
                    }
                    catch
                    {
                        Debug.Log("Detected unsupport tag. (ex. <externalObject>)");
                    }
                }

                if (isStartElement(reader, "description"))
                {
                    reader.Read();
                    string trimedValue = reader.Value.Trim();
                    dicDesc.Add(localName, trimedValue);


                    string[] partOfDesc = trimedValue.Split(':');
                    foreach (string oneDesc in partOfDesc)
                    {
                        int startStr = oneDesc.IndexOf("storey");
                        if (startStr >= 0)
                        {
                            string storey = oneDesc.Replace("storey=", "");
                            storey = storey.Replace("\"", "");

                            //dictCellToStorey.Add(localName, nStorey);

                            if (allStoreys.Contains(storey) == false)
                            {
                                allStoreys.Add(storey);
                            }
                        }
                    }
                }


                // !!
                //if (listElvCell.Contains(localName) == false)
                //{
                //    continue;
                //}

                if (isStartElement(reader, "Geometry2D"))
                {
                    On2DCell(reader, solid, localName, localType);
                }

                if (isStartElement(reader, "Solid"))
                {
                    OnSolid(reader, solid, localName, localType);
                }
            }
        }
    }

    private void On2DCell(XmlReader reader, GameObject solid, string localName, string localType)
    {
        int faceCnt = 1;
        solid.name = localName;
        reader.Read();

        float lowestHeight = float.MaxValue;
        string lowestFaceName = "";
        Poly2Mesh.Polygon floorPolygon = new Poly2Mesh.Polygon();

        List<Vector3> allPoints = new List<Vector3>();

            reader.Read();
            if (isStartElement(reader, "Polygon") || isStartElement(reader, "PolygonPatch"))
            {
                //Poly2Mesh.Polygon polygon = OnPolygon(reader);
                string tmpStorey;
                dictCellToStorey.TryGetValue(localName, out tmpStorey);

                var polygon = OnPolygon_PB(reader, tmpStorey);

                //GameObject genPolygon = Poly2Mesh.CreateGameObject(polygon);
                GameObject genPolygon = polygon.gameObject;
                genPolygon.AddComponent<MeshCollider>();

                genPolygon.name = string.Format("{0}_Face:{1}", localName, faceCnt++);
                genPolygon.transform.parent = solid.transform;

                // Cell의 ID를 가시화 하기 위해서 각 Face의 중앙값을 모아서 Solid의 중앙값을 구한다.
                //polygon.vertexCount .ToMesh();
                //var posArray = polygon.GetVertices().ToList();

                // Debug.Log($"{genPolygon.name} : {polygon.positions.Count}");
                var faceAverage = new Vector3(
                lastFacePos.Average(x => x.x),
                lastFacePos.Average(x => x.y),
                lastFacePos.Average(x => x.z));
                allPoints.Add(faceAverage);
                //Debug.Log($"{genPolygon.name} : {polygon.vertexCount}");

                ApplyCellSpaceMaterial(localType, genPolygon);

                //!! 디버그!!!!!
                //break;
        }

        var solidAverage = new Vector3(
                allPoints.Average(x => x.x),
                allPoints.Average(x => x.y),
                allPoints.Average(x => x.z));

        solidCenters.Add(solidAverage);
        solidIDs.Add(localName);

        //RegisterFloor(localType, lowestHeight, lowestFaceName, floorPolygon);
    }



    private void OnSolid(XmlReader reader, GameObject solid, string localName, string localType)
    {
        int faceCnt = 1;
        solid.name = localName;
        reader.Read();

        float lowestHeight = float.MaxValue;
        string lowestFaceName = "";
        Poly2Mesh.Polygon floorPolygon = new Poly2Mesh.Polygon();

        List<Vector3> allPoints = new List<Vector3>();

        while (isEndElement(reader, "Solid") == false)
        {
            reader.Read();
            if (isStartElement(reader, "Polygon") || isStartElement(reader, "PolygonPatch"))
            {
                //Poly2Mesh.Polygon polygon = OnPolygon(reader);
                string tmpStorey;
                dictCellToStorey.TryGetValue(localName, out tmpStorey);

                var polygon = OnPolygon_PB(reader, tmpStorey);

                //GameObject genPolygon = Poly2Mesh.CreateGameObject(polygon);
                GameObject genPolygon = polygon.gameObject;
                genPolygon.AddComponent<MeshCollider>();

                genPolygon.name = string.Format("{0}_Face:{1}", localName, faceCnt++);
                genPolygon.transform.parent = solid.transform;

                // Cell의 ID를 가시화 하기 위해서 각 Face의 중앙값을 모아서 Solid의 중앙값을 구한다.
                //polygon.vertexCount .ToMesh();
                //var posArray = polygon.GetVertices().ToList();

                // Debug.Log($"{genPolygon.name} : {polygon.positions.Count}");
                var faceAverage = new Vector3(
                lastFacePos.Average(x => x.x),
                lastFacePos.Average(x => x.y),
                lastFacePos.Average(x => x.z));
                allPoints.Add(faceAverage);
                //Debug.Log($"{genPolygon.name} : {polygon.vertexCount}");


                // 각 솔리드에서 가장 낮은 면을 바닥으로 잡음.
                // 조금은 기울어진 바닥이라 할지라도 잡아낼 수 있게끔한게 의도.
                // 현재까지의 모든 데이터는 바닥이 모두 평면으로 되어 있음.
                //float thisMinY = polygon.outside.Average(v => v.y);
                //if (thisMinY < lowestHeight)
                //{
                //    lowestHeight = thisMinY;
                //    floorPolygon = polygon;
                //    lowestFaceName = localName;
                //}
                ApplyCellSpaceMaterial(localType, genPolygon);

                //!! 디버그!!!!!
                //break;
            }
        }

        var solidAverage = new Vector3(
                allPoints.Average(x => x.x),
                allPoints.Average(x => x.y),
                allPoints.Average(x => x.z));

        solidCenters.Add(solidAverage);
        solidIDs.Add(localName);

        //RegisterFloor(localType, lowestHeight, lowestFaceName, floorPolygon);
    }

    private static void RegisterFloor(string localType, float lowestHeight, string lowestFaceName, Poly2Mesh.Polygon floorPolygon)
    {
        GameObject floorObj = Poly2Mesh.CreateGameObject(floorPolygon);
        floorObj.name = lowestHeight + "_" + lowestFaceName;

        // 하나의 솔리드에서 공간타입이 2개 이상 존재할 수는 없다
        ApplyCellSpaceMaterial(localType, floorObj);

        floorObj.transform.parent = CommonObjs.gmlRootFloor.transform;
    }

    private static void ApplyCellSpaceMaterial(string localType, GameObject genPolygon)
    {
        if (localType.ToLower().Equals("transitionspace"))
        {
            genPolygon.tag = "TAG_TRANSITIONSPACE";
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialTransitionSpace;
        }
        else if (localType.ToLower().Equals("generalspace"))
        {
            genPolygon.tag = "TAG_GENERALSPACE";
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialGeneralSpace;
        }
        else if (localType.ToLower().Equals("connectionspace"))
        {
            genPolygon.tag = "TAG_CONNECTIONSPACE";
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialConnectionSpace;
        }
        else if (localType.ToLower().Equals("anchorspace"))
        {
            genPolygon.tag = "TAG_ANCHORSPACE";
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialAnchorSpace;
        }
        else
        {
            // CellSpace (Default)
            genPolygon.tag = "TAG_CELLSPACE";
            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialCellSpace;
        }
    }

    private void OnCellSpaceBoundaryMember(XmlReader reader)
    {
        List<Vector2> localUVs = new List<Vector2>();
        //Poly2Mesh.Polygon localPolygon = new Poly2Mesh.Polygon();
        ProBuilderMesh localPolygon = new ProBuilderMesh();
        string localFileName = string.Empty;
        string localName = string.Empty;
        while (isEndElement(reader, "cellSpaceBoundaryMember") == false)
        {
            reader.Read();

            if (string.IsNullOrWhiteSpace(localName))
            {
                reader.Read();
                localName = reader.GetAttribute("gml:id");
            }

            if (isStartElement(reader, "Polygon") || isStartElement(reader, "PolygonPatch"))
            {
                //localPolygon = OnPolygon_PB(reader);

                // 정보가 없다.. 일단 0층으로 묶는다.
                localPolygon = OnPolygon_PB(reader, "0");
                //localPolygon_PB.uv
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

            if (isStartElement(reader, "OrientableSurface"))
            {
                // Now, Viewer shows always both of sides.
                // So nothing to do for viewing for other side. 
                return;
            }
        }

        //tagStack.Pop();

        if (localUVs.Count() > 2)
        {
            //localPolygon.SetUVs(0, new List<Vector4>(localUVs));


            //localPolygon.outsideUVs = localUVs;
            //localPolygon.holesUVs = new List<List<Vector2>>();
            //localPolygon.outsideUVs.Reverse();
        }

        //localPolygon.outside.Reverse();

        //for (int i = 0; i < localPolygon.holes.Count(); i++)
        //{
        //    localPolygon.holes[i].Reverse();
        //    localPolygon.holesUVs.Add(new List<Vector2>());
        //}

        //// Texture 구멍 무시.
        //if (string.IsNullOrWhiteSpace(localFileName) == false)
        //{
        //    localPolygon.holes = new List<List<Vector3>>();
        //    localPolygon.holesUVs = new List<List<Vector2>>();
        //}

        GameObject cellSpaceBoundary = localPolygon.gameObject;
        cellSpaceBoundary.name = localName;

        if (string.IsNullOrWhiteSpace(localFileName))
        {
            // 일반 벽과 지오메트리 정보가 겹칠경우를 대비하여 앞쪽으로 조금 이동
            //cellSpaceBoundary.transform.Translate(localPolygon.planeNormal * 0.01f);
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

    public void SelectOneStorey(int idx)
    {
        if (idx > 0)
        {
            var tmpDropDown = GameObject.Find("Dropdown_storey").GetComponent<Dropdown>();

            string selectedItem = tmpDropDown.options[tmpDropDown.value].text;
            selectedStorey = selectedItem;

            // 선택한 층의 Cell만 출력한다.
            foreach (string oneKey in dictCellToStorey.Keys)
            {
                string cellStorey;
                dictCellToStorey.TryGetValue(oneKey, out cellStorey);
                if (cellStorey != selectedStorey)
                {
                    GameObject.Find(oneKey).transform.localScale = Vector3.zero;
                }
                else
                {
                    GameObject.Find(oneKey).transform.localScale = Vector3.one;
                }
            }

            // 선택한 층의 ID만 출력한다.
            foreach (string oneKey in dictIdToStorey.Keys)
            {
                string idStorey;
                dictIdToStorey.TryGetValue(oneKey, out idStorey);
                if (idStorey != selectedStorey)
                {
                    GameObject.Find(oneKey).transform.localScale = Vector3.zero;
                }
                else
                {
                    GameObject.Find(oneKey).transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                }
            }

            // 선택한 층의 state만 출력한다..
            int state_size = Convert.ToInt32(GetUnitSize() * Ignition.lastStateSize);
            foreach (string oneKey in dictStateToStorey.Keys)
            {
                string idStorey;
                dictStateToStorey.TryGetValue(oneKey, out idStorey);
                if (idStorey != selectedStorey)
                {
                    GameObject.Find(oneKey).transform.localScale = Vector3.zero;
                }
                else
                {
                    GameObject.Find(oneKey).transform.localScale = new Vector3(state_size, state_size, state_size);
                }
            }

            // 선택한 층의 transition만 출력한다.
            foreach (string oneKey in dictTransitionToStorey.Keys)
            {
                string idStorey;
                dictTransitionToStorey.TryGetValue(oneKey, out idStorey);
                if (idStorey != selectedStorey)
                {
                    GameObject.Find(oneKey).transform.localScale = Vector3.zero;
                }
                else
                {
                    //GameObject.Find(oneKey).transform.localScale = Vector3.one;
                    if (fastestEdgeList.Count() > 0)
                    {
                        if (fastestEdgeList.Contains(oneKey))
                        {
                            GameObject.Find(oneKey).transform.localScale = Vector3.one;
                            // 최단거리 색상은 붉은색.
                            var lineRenderer = GameObject.Find(oneKey).GetComponent<LineRenderer>();
                            lineRenderer.startColor = Color.red;
                            lineRenderer.endColor = Color.red;
                        }
                        else
                        {
                            GameObject.Find(oneKey).transform.localScale = Vector3.zero;
                        }
                    }
                    else
                    {
                        GameObject.Find(oneKey).transform.localScale = Vector3.one;
                        // 일반은 녹색.
                        var lineRenderer = GameObject.Find(oneKey).GetComponent<LineRenderer>();
                        lineRenderer.startColor = Color.green;
                        lineRenderer.endColor = Color.green;
                    }

                }
            }

        }
        else
        {
            selectedStorey = "";
            
            int state_size = Convert.ToInt32(GetUnitSize() * Ignition.lastStateSize);

            // 모든층을 표현함.
            foreach (string oneKey in dictCellToStorey.Keys)
            {
                GameObject.Find(oneKey).transform.localScale = Vector3.one;
            }

            // ID 출력한다.
            foreach (string oneKey in dictIdToStorey.Keys)
            {
                GameObject.Find(oneKey).transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
            }

            // State 출력.
            foreach (string oneKey in dictStateToStorey.Keys)
            {
                GameObject.Find(oneKey).transform.localScale = new Vector3(state_size, state_size, state_size);
            }

            // Transition 출력.
            foreach (string oneKey in dictTransitionToStorey.Keys)
            {
                if (fastestEdgeList.Count() > 0)
                {
                    if (fastestEdgeList.Contains(oneKey))
                    {
                        GameObject.Find(oneKey).transform.localScale = Vector3.one;
                        // 최단거리 색상은 붉은색.
                        var lineRenderer = GameObject.Find(oneKey).GetComponent<LineRenderer>();
                        lineRenderer.startColor = Color.red;
                        lineRenderer.endColor = Color.red;
                    }
                    else
                    {
                        GameObject.Find(oneKey).transform.localScale = Vector3.zero;
                    }
                }
                else
                {
                    GameObject.Find(oneKey).transform.localScale = Vector3.one;
                    // 최단거리 색상은 녹색.
                    var lineRenderer = GameObject.Find(oneKey).GetComponent<LineRenderer>();
                    lineRenderer.startColor = Color.green;
                    lineRenderer.endColor = Color.green;
                }
            }
        }

        GameObject.Find("_Main_").GetComponent<Ignition>().UpdateStatesSize();
    }

    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        if (outLines == null || outLines.Count() == 0)
            return;

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        //for (int i = 0; i < outLines.Count(); ++i)
        //{
        //    GL.Begin(GL.LINES);
        //    for (int j = 0; j < outLines[i].Count() - 1; j++)
        //    {
        //        GL.Vertex3(outLines[i][j].x, outLines[i][j].y, outLines[i][j].z);
        //        GL.Vertex3(outLines[i][j + 1].x, outLines[i][j + 1].y, outLines[i][j + 1].z);
        //    }
        //    GL.End();
        //}
        //GL.PopMatrix();

        //selectedStorey

        for (int i = 0; i < outLines.Count(); ++i)
        {
            if (outLines[i].GetStorey() == selectedStorey || selectedStorey == "")
            {
                GL.Begin(GL.LINES);
                for (int j = 0; j < outLines[i].innerLines.Count() - 1; j++)
                {
                    GL.Vertex3(outLines[i].innerLines[j].x, outLines[i].innerLines[j].y, outLines[i].innerLines[j].z);
                    GL.Vertex3(outLines[i].innerLines[j + 1].x, outLines[i].innerLines[j + 1].y, outLines[i].innerLines[j + 1].z);
                }
                GL.End();
            }
        }
        GL.PopMatrix();






        // ID 출력
        var CanvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>();

        if (GameObject.Find("Toggle_ID").GetComponent<Toggle>().isOn)
        {
            for (int i = 0; i < allSolidIDObjs.Length; i++)
            {
                Vector3 ViewportPosition = Camera.main.WorldToViewportPoint(solidCenters[i]);
                Vector3 WorldObject_ScreenPosition = new Vector2(
                ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

                if (ViewportPosition.x > 0 && ViewportPosition.y > 0 && ViewportPosition.z > 0)
                {
                    allSolidIDObjs[i].GetComponent<RectTransform>().anchoredPosition = WorldObject_ScreenPosition;
                }
                else
                {
                    allSolidIDObjs[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-10000, -10000);
                }

            }
        }
        else
        {
            try
            {
                for (int i = 0; i < allSolidIDObjs.Length; i++)
                {
                    allSolidIDObjs[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-10000, -10000);
                }
            }
            catch
            {
                // ID 형식이 맞지 않은경우..
            }

        }
    }

    static public string GetCellInfo(string cellID)
    {
        string valName = string.Empty;
        string valStorey = string.Empty;
        string valType = string.Empty;
        string valDesc = string.Empty;

        if (dicName.TryGetValue(cellID, out valName) == false)
        {
            valName = "(Empty)";
        }

        if (dicStorey.TryGetValue(cellID, out valStorey) == false)
        {
            valStorey = "(Empty)";
        }

        if (dicType.TryGetValue(cellID, out valType) == false)
        {
            valType = "(Empty)";
        }

        if (dicDesc.TryGetValue(cellID, out valDesc) == false)
        {
            valDesc = "(Empty)";
        }

        string sepID = "[#entry#]";
        string sepValues = "[#value#]";

        return $"{cellID}{sepID}{valName}{sepValues}{valStorey}{sepValues}{valType}{sepValues}{valDesc}";
    }

    public void OnPostRender()
    {
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

    public static int cell_From;
    public static int cell_To;

    public void GoNormal()
    {
        var from = Convert.ToInt32(GameObject.Find("InputField_From").GetComponent<InputField>().text.Trim());
        var to = Convert.ToInt32(GameObject.Find("InputField_To").GetComponent<InputField>().text.Trim());

        GoNormal(false, from, to);
    }

    public void GoFire()
    {
        var from = Convert.ToInt32(GameObject.Find("InputField_From").GetComponent<InputField>().text.Trim());
        var to = Convert.ToInt32(GameObject.Find("InputField_To").GetComponent<InputField>().text.Trim());

        GoNormal(true, from, to);
    }

    public void GoClear()
    {
        fastestEdgeList.Clear();
        SelectOneStorey(GameObject.Find("Dropdown_storey").GetComponent<Dropdown>().value);
    }


    public void GoNormal(bool isFire, int from = -1, int to = -1)
    {
        // PathFinder와 QUickParser는 같은 Main객체에 있어야 한다.
        //var myFinder = GetComponent<PathFinder>();

        //GraphArray.Clear();
        //string myFrom = "-1";
        //string myTo;

        if (from == -1 || to == -1)
        {
            from = Convert.ToInt32(GameObject.Find("InputField_From").GetComponent<InputField>().text.Trim());
            to = Convert.ToInt32(GameObject.Find("InputField_To").GetComponent<InputField>().text.Trim());
        }

        List<int> listFrom = new List<int>();
        List<int> listTo = new List<int>();
        List<float> listDistance = new List<float>();

        var allKeys = dictEdgeToTransitionNormal.Keys.ToArray();
        for (int i = 0; i < allKeys.Count(); i++)
        {
            string oneFrom = (allKeys[i].Split('~')[0]);
            string oneTo = (allKeys[i].Split('~')[1]);

            float oneDist = Vector3.Distance(GameObject.Find(oneFrom).transform.position, GameObject.Find(oneTo).transform.position);

            string fromCellID;
            string toCellID;

            dictNodeToCellID.TryGetValue(oneFrom, out fromCellID);
            dictNodeToCellID.TryGetValue(oneTo, out toCellID);

            if (isFire && (listElvCell.Contains(fromCellID) || listElvCell.Contains(toCellID)))
            {
                //oneDist = 10;
                continue;
            }

            int nFrom = Convert.ToInt32(oneFrom.Split('-')[1]);
            int nTo = Convert.ToInt32(oneTo.Split('-')[1]);

            listFrom.Add(nFrom);
            listTo.Add(nTo);
            listDistance.Add(oneDist);
        }

        int maxi = listFrom.Max() > listTo.Max() ? listFrom.Max() : listTo.Max();
        
        
        OLD_Path(from, to, listFrom, listTo, listDistance, maxi);
    }

    private void OLD_Path(int myFrom, int myTo, List<int> listFrom, List<int> listTo, List<float> listDistance, int maxi)
    {
        GraphArray g = new GraphArray(maxi + 1);
        for (int i = 0; i < listDistance.Count(); i++)
        {
            g.AddEdge(listFrom[i], listTo[i], Convert.ToInt32(listDistance[i]));
        }

        //List<int> transList = g.Dijkstra(196, 15);
        List<int> transList = g.Dijkstra(myFrom, myTo);

        fastestEdgeList = new List<string>();
        for (int i = 1; i < transList.Count; i++)
        {
            string nameTrans = string.Empty;
            bool isFound = dictEdgeToTransitionNormal.TryGetValue($"node-{transList[i - 1]}~node-{transList[i]}", out nameTrans);
            if (isFound == false)
            {
                isFound = dictEdgeToTransitionNormal.TryGetValue($"node-{transList[i]}~node-{transList[i - 1]}", out nameTrans);
            }
            // 무조건 있어야 한다.
            if (isFound)
            {
                fastestEdgeList.Add(nameTrans);
            }
            else
            {
                Debug.LogError("Fatal ERROR - Cannot find transition) " + $"node-{transList[i - 1]} ~ node-{transList[i]}");
                string allRoute = "GO";
                foreach (int n in transList)
                {
                    allRoute += "-> " + n;
                }
                Debug.Log(allRoute);
            }
        }


        SelectOneStorey(GameObject.Find("Dropdown_storey").GetComponent<Dropdown>().value);

        Debug.Log("* Complete");
    }
}
