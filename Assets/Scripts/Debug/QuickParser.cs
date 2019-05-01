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
    Vector3 firstPos;
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

    private Poly2Mesh.Polygon OnPolygon(XmlReader reader)
    {
        List<Vector3> localOutlines = new List<Vector3>();
        List<List<Vector3>> localHoles = new List<List<Vector3>>();

        while (isEndElement(reader, "Polygon") == false)
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

    private Vector3 GetPos3D(XmlReader reader)
    {
        string[] gmlVector3d = reader.Value.Trim().Split(' ');
        Vector3 unityVector3d = new Vector3();

        // Unity3D Vector Style.
        float.TryParse(gmlVector3d[0], out unityVector3d.x);
        float.TryParse(gmlVector3d[1], out unityVector3d.z);
        float.TryParse(gmlVector3d[2], out unityVector3d.y);

        if (firstPos.Equals(Vector3.zero))
        {
            firstPos = unityVector3d;
        }
        Vector3 relativeUnityVector3d = unityVector3d - firstPos;

        sceneBound.Encapsulate(relativeUnityVector3d);

        return relativeUnityVector3d;
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
        firstPos = new Vector3();
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
                while (isEndElement(reader, "Solid") == false)
                {
                    reader.Read();
                    if (isStartElement(reader, "Polygon"))
                    {
                        var polygon = OnPolygon(reader);
                        GameObject genPolygon = Poly2Mesh.CreateGameObject(polygon);
                        genPolygon.name = string.Format("{0}_Face:{1}", localName, faceCnt++);

                        genPolygon.transform.parent = solid.transform;

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
                            // CellSpace
                            genPolygon.GetComponent<Renderer>().material = CommonObjs.materialCellSpace;
                        }
                    }
                }
            }
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
            }

            if (isStartElement(reader, "Polygon"))
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
            Debug.Log(GetDirectoryName() + "\\" + localFileName);
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
