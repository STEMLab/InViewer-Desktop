using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TextureCoord : MonoBehaviour
{
    public float width;
    public float height;

    private Material materialTextureSurface;

    private void Awake()
    {
        materialTextureSurface = new Material(Shader.Find("Unlit/Texture"));
    }

    private void ConvertGML(string source, ref List<Vector3> outlines, ref List<Vector2> uvs)
    {
        string convtertedGML = source;
        convtertedGML = convtertedGML.Replace("<gml:pos srsDimension=3>", string.Empty);
        convtertedGML = convtertedGML.Replace("<gml:pos srsDimension=2>", string.Empty);
        convtertedGML = convtertedGML.Replace("</gml:pos>", string.Empty);
        convtertedGML = convtertedGML.Replace("\r", string.Empty);

        string[] lines = convtertedGML.Split('\n');

        //List<Vector3> outlines = new List<Vector3>();
        //List<Vector2> uvs = new List<Vector2>();

        foreach (string line in lines)
        {
            string[] token = line.Trim().Split(' ');

            if(token.Length == 3)
            {
                outlines.Add(new Vector3(Convert.ToSingle(token[0]), Convert.ToSingle(token[2]), Convert.ToSingle(token[1])));
            }
            else if(token.Length == 2)
            {
                uvs.Add(new Vector3(Convert.ToSingle(token[0]), Convert.ToSingle(token[1])));
            } 
        }
    }

    IEnumerator ApplyTexture(GameObject targetObj, string url)
    {
        //Debug.Log(id + "_" + url);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(@"E:\Data\Centralplaza_IndoorGML\res\textures\" + url);
        yield return www.SendWebRequest();

        try
        {
            Texture myTexture = DownloadHandlerTexture.GetContent(www);
            //targetBoundary.GetComponent<MeshRenderer>().material = Resources.Load("Materials /TextureSurface", typeof(Material)) as Material;

            targetObj.GetComponent<Renderer>().material = materialTextureSurface;

            targetObj.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", myTexture);
        }
        catch
        {
            Debug.Log("ERROR File: " + url);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //LowLevelType();
        //HighLevelType();
        //QuickParser qp = new QuickParser(@"E:\Data\Centralplaza_IndoorGML\CentralPlaza-texture.gml");
        

    }

    private void HighLevelType()
    {
        List<Vector3> listOutlines = new List<Vector3>();
        List<Vector2> listUvs = new List<Vector2>();

        ConvertGML(@"
                                                                                < gml:pos srsDimension=3>19.097 -8.63288 -1.43055</gml:pos>
										<gml:pos srsDimension=3>39.8068 -8.77212 -1.90824</gml:pos>
										<gml:pos srsDimension=3>39.8136 -7.772 -1.90404</gml:pos>
										<gml:pos srsDimension=3>39.8343 -4.77176 -1.89145</gml:pos>
										<gml:pos srsDimension=3>43.6518 -4.78922 -1.97947</gml:pos>
										<gml:pos srsDimension=3>43.7492 3.56406 -1.94534</gml:pos>
										<gml:pos srsDimension=3>39.9381 3.60195 -1.85738</gml:pos>
										<gml:pos srsDimension=3>39.9898 7.7111 -1.84068</gml:pos>
										<gml:pos srsDimension=3>-4.91297 7.9579 -0.805201</gml:pos>
										<gml:pos srsDimension=3>-4.92803 4.83191 -0.818466</gml:pos>
										<gml:pos srsDimension=3>-3.94026 4.82609 -0.841246</gml:pos>
										<gml:pos srsDimension=3>-3.95074 3.11801 -0.848443</gml:pos>
										<gml:pos srsDimension=3>-4.42202 3.12099 -0.837573</gml:pos>
										<gml:pos srsDimension=3>-4.45797 -3.76 -0.866708</gml:pos>
										<gml:pos srsDimension=3>-3.99901 -3.76 -0.877281</gml:pos>
										<gml:pos srsDimension=3>-3.99399 -5.23896 -0.883837</gml:pos>
										<gml:pos srsDimension=3>-4.84513 -5.24504 -0.864257</gml:pos>
										<gml:pos srsDimension=3>-4.80288 -8.44405 -0.87916</gml:pos>
										<gml:pos srsDimension=3>5.7054 -8.50294 -1.12149</gml:pos>
										<gml:pos srsDimension=3>6.10652 -8.65801 -1.13141</gml:pos>
										<gml:pos srsDimension=3>6.16534 -10.9872 -1.1429</gml:pos>
										<gml:pos srsDimension=3>9.68799 -11.1717 -1.22486</gml:pos>
										<gml:pos srsDimension=3>9.8024 -8.87222 -1.21748</gml:pos>
										<gml:pos srsDimension=3>12.2722 -8.53998 -1.27293</gml:pos>
										<gml:pos srsDimension=3>12.164 -26.805 -1.35</gml:pos>
										<gml:pos srsDimension=3>18.908 -26.851 -1.506</gml:pos>
										<gml:pos srsDimension=3>19.097 -8.63288 -1.43055</gml:pos>


<gml:pos srsDimension=2>0.493558 0.476661</gml:pos>
						<gml:pos srsDimension=2>0.919009 0.480721</gml:pos>
						<gml:pos srsDimension=2>0.919149 0.451991</gml:pos>
						<gml:pos srsDimension=2>0.919574 0.365805</gml:pos>
						<gml:pos srsDimension=2>0.997999 0.366318</gml:pos>
						<gml:pos srsDimension=2>1.000000 0.126359</gml:pos>
						<gml:pos srsDimension=2>0.921707 0.125260</gml:pos>
						<gml:pos srsDimension=2>0.922769 0.007219</gml:pos>
						<gml:pos srsDimension=2>0.000309 0.000000</gml:pos>
						<gml:pos srsDimension=2>0.000000 0.089798</gml:pos>
						<gml:pos srsDimension=2>0.020292 0.089968</gml:pos>
						<gml:pos srsDimension=2>0.020077 0.139035</gml:pos>
						<gml:pos srsDimension=2>0.010395 0.138948</gml:pos>
						<gml:pos srsDimension=2>0.009657 0.336613</gml:pos>
						<gml:pos srsDimension=2>0.019085 0.336615</gml:pos>
						<gml:pos srsDimension=2>0.019188 0.379100</gml:pos>
						<gml:pos srsDimension=2>0.001703 0.379272</gml:pos>
						<gml:pos srsDimension=2>0.002571 0.471168</gml:pos>
						<gml:pos srsDimension=2>0.218448 0.472890</gml:pos>
						<gml:pos srsDimension=2>0.226688 0.477346</gml:pos>
						<gml:pos srsDimension=2>0.227896 0.544255</gml:pos>
						<gml:pos srsDimension=2>0.300264 0.549565</gml:pos>
						<gml:pos srsDimension=2>0.302614 0.483510</gml:pos>
						<gml:pos srsDimension=2>0.353353 0.473973</gml:pos>
						<gml:pos srsDimension=2>0.351130 0.998659</gml:pos>
						<gml:pos srsDimension=2>0.489675 1.000000</gml:pos>
						<gml:pos srsDimension=2>0.493558 0.476661</gml:pos>


", ref listOutlines, ref listUvs);

        string myTexture = "surface0009.jpg";

        Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();

        listOutlines.Reverse();
        listUvs.Reverse();

        poly.outside = listOutlines;
        poly.outsideUVs = listUvs;

        var partObj = Poly2Mesh.CreateGameObject(poly, "NewPoly");

        StartCoroutine(ApplyTexture(partObj, myTexture));
    }

    private void LowLevelType()
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        var mesh = new Mesh();
        mf.mesh = mesh;

        List<Vector3> listOutlines = new List<Vector3>();
        List<Vector2> listUvs = new List<Vector2>();

        ConvertGML(@"
                	<gml:pos srsDimension=3>18.908 -26.851 -1.506</gml:pos>
										<gml:pos srsDimension=3>12.164 -26.805 -1.35</gml:pos>
										<gml:pos srsDimension=3>11.987 -26.96 -1.626</gml:pos>

						<gml:pos srsDimension=2>0.434783 1.000000</gml:pos>
						<gml:pos srsDimension=2>1.000000 0.025708</gml:pos>
						<gml:pos srsDimension=2>0.000000 0.000000</gml:pos>

", ref listOutlines, ref listUvs);

        string myTexture = "surface0005.jpg";

        Poly2Mesh.Polygon poly = new Poly2Mesh.Polygon();

        poly.outside = listOutlines;
        poly.outsideUVs = listUvs;


        Vector3[] vertices = listOutlines.ToArray();
        Vector2[] uvs = listUvs.ToArray();

        //Vector3 tmpVector3 = vertices[2];
        //vertices[2] = vertices[3];
        //vertices[3] = tmpVector3;

        //Vector2 tmpVector2 = uvs[2];
        //uvs[2] = uvs[3];
        //uvs[3] = tmpVector2;

        // 4각형일 경우 3과 4을 바꿈.


        mesh.vertices = vertices;

        int[] tri = new int[3];

        tri[0] = 0;
        tri[1] = 2;
        tri[2] = 1;

        //tri[3] = 2;
        //tri[4] = 3;
        //tri[5] = 1;

        mesh.triangles = tri;

        Vector3[] normals = new Vector3[3];

        normals[0] = -Vector3.forward;
        normals[1] = -Vector3.forward;
        normals[2] = -Vector3.forward;
        //normals[3] = -Vector3.forward;
        //normals[4] = -Vector3.forward;

        mesh.normals = normals;

        //Vector2[] uv = new Vector2[4];

        //uv[0] = new Vector2(0, 0);
        //uv[1] = new Vector2(1f, 0);
        //uv[2] = new Vector2(0, 1);
        //uv[3] = new Vector2(0.5f, 1);

        mesh.uv = uvs;

        StartCoroutine(ApplyTexture(gameObject, myTexture));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
