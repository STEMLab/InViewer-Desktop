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
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
