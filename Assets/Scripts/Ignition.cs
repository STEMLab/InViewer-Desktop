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
    private GameObject gmlRoot;
    private GameObject gmlRootCellSpace;
    private GameObject gmlRootGeneralSpace;
    private GameObject gmlRootTransitionSpace;
    private GameObject gmlRootCellSpaceBoundary;
    private GameObject gmlRootState;
    private GameObject gmlRootTransition;

    public static Bounds totalBounds;

    public static Material materialCellSpace;
    public static Material materialGeneralSpace;
    public static Material materialTransitionSpace;
    public static Material materialCellSpaceBoundary;
    public static Material materialState;
    public static Material materialTextureSurface;

    private Material materialLine;

    private Shader shaderCullOFF;
    private Shader shaderCullON;
    private Shader shaderOutline;

    private List<List<Vector3>> outLines;
    //private List<Vector3> outLinesBegin;
    //private List<Vector3> outLinesEnd;

    private void Awake()
    {
        totalBounds = new Bounds();
    }

    void Start()
    {
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
        materialTextureSurface = new Material(Shader.Find("Unlit/Texture"));

        materialCellSpace.shader = shaderCullOFF;
        materialGeneralSpace.shader = shaderCullOFF;
        materialTransitionSpace.shader = shaderCullOFF;
        materialCellSpaceBoundary.shader = shaderCullOFF;

        outLines = new List<List<Vector3>>();

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

        int faceIdx = 1;

        string lastID = "";
        GameObject lastSolid = null;

        foreach (var geom in geometry3D)
        {

            if (lastID.Equals(geom.id) == false && geom.localName.Equals("Point") == false)
            {
                lastSolid = new GameObject(geom.id);
                lastID = geom.id;
                faceIdx = 1;
            }

            // Point기반 State
            if (geom.localName.Equals("Point"))
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sphere.name = geom.id;
                sphere.tag = "TAG_STATE";

                var myRenderer = sphere.GetComponent<Renderer>();

                sphere.transform.localScale = new Vector3(5, 5, 5);
                sphere.transform.position = geom.exterior[0];
                sphere.transform.parent = gmlRootState.transform;
                myRenderer.material = materialState;
                sphere.AddComponent<MeshCollider>();

                // State 선택가능
                sphere.AddComponent<IndoorGmlSelectable>();

                // 성능 문제
                myRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                myRenderer.receiveShadows = false;
                myRenderer.allowOcclusionWhenDynamic = false;
                myRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;


                continue;
            }

            // LineString 기반....
            if (geom.localName.Equals("LineString"))
            {
                var tmpTransition = lastSolid.AddComponent<LineRenderer>();
                tmpTransition.useWorldSpace = false;
                tmpTransition.tag = "TAG_TRANSITION";
                tmpTransition.material = new Material(Shader.Find("Sprites/Default"));
                tmpTransition.startWidth = 1f;
                tmpTransition.endWidth = 0.1f;
                tmpTransition.startColor = Color.red;
                tmpTransition.endColor = Color.green;

                tmpTransition.positionCount = geom.exterior.Count();

                var tmpCenter = CenterOfVectors(geom.exterior);
                lastSolid.transform.position = tmpCenter;

                var recenterVectors = RecenterOfVectors(geom.exterior, tmpCenter);
                tmpTransition.SetPositions(recenterVectors);
                //tmpTransition.SetPositions(geom.vertices.ToArray());

                tmpTransition.transform.parent = gmlRootTransition.transform;
                lastSolid.AddComponent<IndoorGmlSelectable>();
                continue;
            }

            // 여기 이하는 폴리곤 기반. CellSpace, CellSpaceBoundary
            if (geom.localName.Equals("LinearRing") == false)
            {
                continue;
            }


            Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();

            // Combine 작업을 안할경우 제대로 출력되는 조합
            //var faceCenter = CenterOfVectors(geom.vertices);
            //var faceAdjPos = new List<Vector3>(RecenterOfVectors(geom.vertices, faceCenter));
            //poly.outside = faceAdjPos;

            // Combine 할 경우 
            Vector3 myCenter;
            parser.mapCenter.TryGetValue(geom.id, out myCenter);
            var centerAdjOutsidePos = new List<Vector3>(RecenterOfVectors(geom.exterior, myCenter));


            poly.outside = centerAdjOutsidePos;

            if (geom.interiors.Count() > 0)
            {
                poly.holesUVs = new List<List<Vector2>>();
                for (int i = 0; i < geom.interiors.Count(); i++)
                {
                    // 읽는 시점에서 뒤집기를 고려해 보자.
                    //geom.interiors[i].Reverse();

                    var centerAdjHolePos = new List<Vector3>(RecenterOfVectors(geom.interiors[i], myCenter));
                    Debug.Log("Hole WTF: " + geom.id);
                    poly.holes.Add(centerAdjHolePos);
                    poly.holesUVs.Add(new List<Vector2>());
                }
            }

            //if (geom.vertices.Count() == 4)
            //{
            //    poly.outsideUVs = new List<Vector2>();
            //    poly.outsideUVs.Add(new Vector2(0, 1));
            //    poly.outsideUVs.Add(new Vector2(1, 1));
            //    poly.outsideUVs.Add(new Vector2(1, 0));
            //    poly.outsideUVs.Add(new Vector2(0, 0));
            //}

            if ((geom.exterior.Count() == geom.texture_coordinates.Count()))
            {
                //geom.texture_coordinates.RemoveAt(geom.texture_coordinates.Count - 1);
                poly.outsideUVs = geom.texture_coordinates;
            }
            else if (geom.texture_coordinates.Count() != 0 && string.IsNullOrEmpty(geom.texture) == false)
            {
                Debug.Log(string.Format("WTF: {0} (tex:{1} vs outline:{2})", geom.id, geom.texture_coordinates.Count(), geom.exterior.Count()));
            }


            //poly.outsideUVs = geom.texture_coordinates;

            lastSolid.transform.position = myCenter;


            // Do not use the line.
            //poly.outside = geom.vertices;



            // 유니티는 시계방향이 앞면, IndoorGML은 반시계 방향이 앞면.
            poly.outside.Reverse();

            // 데이터 생성 버그로 예외 경우 추가. 디버깅으로 뒤집기 임시 해제
            //if (geom.spaceType != DATA_TYPE.CELLSPACEBOUNDARY)
            //{
            //    if (poly.holes.Count() != 0)
            //    {
            //        poly.holes[0].Reverse();
            //    }
            //}

            GameObject partObj;

            if (geom.id.Equals("CBG-CORRIDOR1-TEXTURE-CEILING"))
            {
                Debug.Log("Hit");
            }

            // CellSpaceBoundary의 경우에는 Face가 1개밖에 없으므로 트리구조를 단순화 함
            if (geom.spaceType == DATA_TYPE.CELLSPACEBOUNDARY)
            {
                partObj = Poly2Mesh.CreateGameObject(poly, geom.id);
            }
            else
            {
                //Debug.Log("Pass: " + geom.id);
                partObj = Poly2Mesh.CreateGameObject(poly, string.Format("{0}_Face:{1}", lastID, faceIdx++));
            }

            // 모든 생성 객체는 선택가능하도록 함
            //tmpObj.AddComponent<IndoorGmlSelectable>();
            //tmpObj.transform.position = faceCenter;
            partObj.AddComponent<MeshCollider>();
            partObj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            partObj.GetComponent<MeshRenderer>().receiveShadows = false;

            partObj.transform.parent = lastSolid.transform;


            if (geom.spaceType == DATA_TYPE.CELLSPACE)
            {
                partObj.GetComponent<Renderer>().material = materialCellSpace;
                partObj.tag = "TAG_CELLSPACE";
                lastSolid.tag = "TAG_CELLSPACE";
                lastSolid.transform.parent = gmlRootCellSpace.transform;
            }
            else if (geom.spaceType == DATA_TYPE.GENERALSPACE)
            {
                partObj.GetComponent<Renderer>().material = materialGeneralSpace;
                partObj.tag = "TAG_GENERALSPACE";
                lastSolid.tag = "TAG_GENERALSPACE";
                lastSolid.transform.parent = gmlRootGeneralSpace.transform;
            }
            else if (geom.spaceType == DATA_TYPE.TRANSITIONSPACE)
            {
                partObj.GetComponent<Renderer>().material = materialTransitionSpace;
                partObj.tag = "TAG_TRANSITIONSPACE";
                lastSolid.tag = "TAG_TRANSITIONSPACE";
                lastSolid.transform.parent = gmlRootTransitionSpace.transform;
            }
            else if (geom.spaceType == DATA_TYPE.CELLSPACEBOUNDARY)
            {
                if (geom.texture.Equals(string.Empty) == false)
                {
                    partObj.GetComponent<Renderer>().material = materialTextureSurface;
                }
                else
                {
                    partObj.GetComponent<Renderer>().material = materialCellSpaceBoundary;
                }

                partObj.tag = "TAG_CELLSPACEBOUNDARY";

                var myMesh = partObj.GetComponent<MeshFilter>();

                Vector2[] uvs = new Vector2[4];
                for (int i = 0; i < 4; i++)
                {
                    uvs[i] = new Vector2(-99, -99);
                }

                Vector3 firstCorner = new Vector3();
                Vector3 secondCorner = new Vector3();

                // 사각형 형태의 mesh 에 대해서만 TextureSurface 허용
                if (myMesh.mesh.triangles.Length == 6)
                {
                    // 겹치는 vertices 2개 구함
                    int matchCnt = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        for (int j = 3; j < 6; j++)
                        {
                            if (myMesh.mesh.triangles[i] == myMesh.mesh.triangles[j])
                            {
                                if (matchCnt == 0)
                                {
                                    firstCorner = myMesh.mesh.vertices[i];
                                    uvs[myMesh.mesh.triangles[i]] = new Vector2(1, 1);
                                    matchCnt++;
                                }
                                else if (matchCnt == 1)
                                {
                                    secondCorner = myMesh.mesh.vertices[i];
                                    uvs[myMesh.mesh.triangles[i]] = new Vector2(0, 0);
                                    i = 3;
                                    break;
                                }
                            }
                        }
                    }

                    matchCnt = 0;

                    for (int i = 0; i < 5; i++)
                    {
                        if (uvs[i].x == -99)
                        {
                            if (matchCnt == 0)
                            {
                                uvs[i] = new Vector2(0, 1);
                                matchCnt++;
                            }
                            else
                            {
                                // 2개 비어 있고 끝
                                uvs[i] = new Vector2(1, 0);
                                break;
                            }
                        }
                    }

                    if (firstCorner.y < secondCorner.y)
                    {
                        // 뒤집힌 경우 uv를 뒤집는다.
                        for (int i = 0; i < 4; i++)
                        {
                            if (uvs[i].Equals(new Vector2(0, 0)))
                            {
                                uvs[i] = new Vector2(1, 1);
                            }
                            if (uvs[i].Equals(new Vector2(1, 1)))
                            {
                                uvs[i] = new Vector2(0, 0);
                            }
                            if (uvs[i].Equals(new Vector2(0, 1)))
                            {
                                uvs[i] = new Vector2(1, 0);
                            }
                            if (uvs[i].Equals(new Vector2(1, 0)))
                            {
                                uvs[i] = new Vector2(0, 1);
                            }
                        }
                    }
                }

                lastSolid.tag = "TAG_CELLSPACEBOUNDARY";
                lastSolid.transform.parent = gmlRootCellSpaceBoundary.transform;

                // TextureSurface 일 경우.
                if (geom.texture.Equals(string.Empty) == false)
                {
                    IEnumerator tmpRunner = ApplyTexture(lastSolid.name, parser.GetDirectoryName() + "\\" + geom.texture);
                    StartCoroutine(tmpRunner);
                }
            }

            // Combine 안할경우
            //tmpObj.transform.position = faceCenter;

            //Combine 할 경우
            //tmpObj.transform.position = myCenter;

            // 외곽선
            //var tmpOutLine = tmpObj.AddComponent<LineRenderer>();

            //// 성능 최적화 부분
            //tmpOutLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            //tmpOutLine.receiveShadows = false;
            //tmpOutLine.allowOcclusionWhenDynamic = false;
            //tmpOutLine.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;

            //tmpOutLine.useWorldSpace = true;
            //tmpOutLine.material = new Material(Shader.Find("Sprites/Default"));
            //tmpOutLine.startWidth = 0.1f;
            //tmpOutLine.endWidth = 0.1f;
            //tmpOutLine.startColor = Color.black;
            //tmpOutLine.endColor = Color.black;

            //tmpOutLine.positionCount = geom.vertices.Count();
            //tmpOutLine.SetPositions(geom.vertices.ToArray());

            // CellSpaceBoundary 실제 객체 노드레벨을 하나 올리고 중복되는 빈 객체 삭제
            if (geom.spaceType == DATA_TYPE.CELLSPACEBOUNDARY)
            {
                Destroy(lastSolid);
                partObj.transform.parent = gmlRootCellSpaceBoundary.transform;
                partObj.transform.position = myCenter;
            }
            
            outLines.Add(geom.exterior);
            
            //if (totalBounds == null || totalBounds.min.Equals(new Vector3(0, 0)))
            //{
            //    totalBounds = new Bounds();
            //    totalBounds = partObj.GetComponent<MeshFilter>().mesh.bounds;
            //}
            //else
            //{
            //    totalBounds.Encapsulate(partObj.GetComponent<MeshFilter>().mesh.bounds);
            //}
        }

        totalBounds = new Bounds();

        for (int i = 0; i < gmlRootCellSpace.transform.childCount; i++)
        {
            var cellSpace = gmlRootCellSpace.transform.GetChild(i);
            cellSpace.gameObject.AddComponent<Combining>();
            cellSpace.gameObject.AddComponent<IndoorGmlSelectable>();

            if (totalBounds == null || totalBounds.min.Equals(new Vector3(0, 0)))
            {
                totalBounds = new Bounds();
                totalBounds.center = cellSpace.gameObject.transform.position;
            }
            else
            {
                totalBounds.Encapsulate(cellSpace.gameObject.transform.position);
            }
        }

        for (int i = 0; i < gmlRootGeneralSpace.transform.childCount; i++)
        {
            var generalSpace = gmlRootGeneralSpace.transform.GetChild(i);
            generalSpace.gameObject.AddComponent<Combining>();
            generalSpace.gameObject.AddComponent<IndoorGmlSelectable>();

            if (totalBounds == null || totalBounds.min.Equals(new Vector3(0, 0)))
            {
                totalBounds = new Bounds();
                totalBounds.center = generalSpace.gameObject.transform.position;
            }
            else
            {
                totalBounds.Encapsulate(generalSpace.gameObject.transform.position);
            }
        }

        for (int i = 0; i < gmlRootTransitionSpace.transform.childCount; i++)
        {
            var transitionSpace = gmlRootTransitionSpace.transform.GetChild(i);
            transitionSpace.gameObject.AddComponent<Combining>();
            transitionSpace.gameObject.AddComponent<IndoorGmlSelectable>();

            if (totalBounds == null || totalBounds.min.Equals(new Vector3(0, 0)))
            {
                totalBounds = new Bounds();
                totalBounds.center = transitionSpace.gameObject.transform.position;
            }
            else
            {
                totalBounds.Encapsulate(transitionSpace.gameObject.transform.position);
            }
        }

        //카메라 앵글을 위한 Bounds 속성을 손보자.

        UpdateStatesSize(1);

        Debug.Log(totalBounds.ToString());

        SendAndReceive.TreeToJSON();
    }

    //private void CalcTotalBounds()
    //{ 
    //    root
    //}

    IEnumerator ApplyTexture(string id, string url)
    {
        //Debug.Log(id + "_" + url);
        GameObject targetBoundary = GameObject.Find(id);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        try
        {
            Texture myTexture = DownloadHandlerTexture.GetContent(www);
            //targetBoundary.GetComponent<MeshRenderer>().material = Resources.Load("Materials/TextureSurface", typeof(Material)) as Material;
            targetBoundary.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", myTexture);
        }
        catch
        {
            Debug.Log("ERROR File: " + url);
        }
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

    //public Material lineMat = new Material("Shader \"Lines/Colored Blended\" {" + "SubShader { Pass { " + "    Blend SrcAlpha OneMinusSrcAlpha " + "    ZWrite Off Cull Off Fog { Mode Off } " + "    BindChannels {" + "      Bind \"vertex\", vertex Bind \"color\", color }" + "} } }");
    private Material lineMaterial;
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

    // Will be called after all regular rendering is done
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

    public void UpdateStatesSize(float multiple)
    {
        int state_size = Convert.ToInt32(GetUnitSize() * multiple);
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
