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

public class QuickParser : MonoBehaviour
{
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

    static private List<List<Vector3>> outLines;

    private XmlReader reader;

    private string _fileUrl;

    private Material lineMaterial;

    public string GetDirectoryName()
    {
        return Path.GetDirectoryName(_fileUrl);
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

    private ProBuilderMesh OnPolygon_PB(XmlReader reader)
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

        outLines.Add(localOutlines);

        for (int i = 0; i < localHoles.Count(); i++)
        {
            outLines.Add(localHoles[i]);
        }

        var polygon_pb = ProBuilderMesh.Create();


        IList<IList<Vector3>> localHoles_pb = new List<IList<Vector3>>(localHoles);

        polygon_pb.CreateShapeFromPolygon(localOutlines, 0, false, localHoles_pb);

        return polygon_pb;

        //Poly2Mesh.Polygon polygon = new Poly2Mesh.Polygon();
        //polygon.outside = localOutlines;
        //polygon.holes = localHoles;

        //return polygon;
    }

    private Poly2Mesh.Polygon OnPolygon_old(XmlReader reader)
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
        //firstPos = new Vector3();
        //firstPosX = 0;
        //firstPosY = 0;
        //firstPosZ = 0;

        sceneBound = new Bounds();

        outLines = new List<List<Vector3>>();

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
                        //Poly2Mesh.Polygon polygon = OnPolygon(reader);
                        var polygon = OnPolygon_PB(reader);

                        //GameObject genPolygon = Poly2Mesh.CreateGameObject(polygon);
                        GameObject genPolygon = polygon.gameObject;

                        genPolygon.name = string.Format("{0}_Face:{1}", localName, faceCnt++);
                        genPolygon.transform.parent = solid.transform;

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
                    }
                }

                //RegisterFloor(localType, lowestHeight, lowestFaceName, floorPolygon);
            }
        }
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
                localPolygon = OnPolygon_PB(reader);
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
        for (int i = 0; i < outLines.Count(); ++i)
        {
            GL.Begin(GL.LINES);
            for (int j = 0; j < outLines[i].Count() - 1; j++)
            {
                GL.Vertex3(outLines[i][j].x, outLines[i][j].y, outLines[i][j].z);
                GL.Vertex3(outLines[i][j + 1].x, outLines[i][j + 1].y, outLines[i][j + 1].z);
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
