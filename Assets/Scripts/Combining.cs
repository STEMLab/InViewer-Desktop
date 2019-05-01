//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Combining : MonoBehaviour
//{
//    // Start is called before the first frame update
//    void Start()
//    {
//        //var materialCellSpace = Resources.Load("Materials/CellSpace", typeof(Material)) as Material;

//        gameObject.AddComponent<MeshFilter>();
//        MeshRenderer myRenderer = gameObject.AddComponent<MeshRenderer>();

//        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
//        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

//        //transform.position = transform.GetComponent<MeshFilter>().mesh.bounds.center;

//        for (int i=0; i < meshFilters.Length; i++)
//        {
//            combine[i].mesh = meshFilters[i].sharedMesh;
//            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
//            //meshFilters[i].gameObject.SetActive(false);
//        }

//        foreach (Transform child in transform)
//        {
//            GameObject.Destroy(child.gameObject);
//        }
                        
//        transform.GetComponent<MeshFilter>().mesh = new Mesh();
//        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
               
//        if (transform.parent.name.Equals(CommonNames.ROOT_CELLSPACE))
//        {
//            myRenderer.material = Ignition.materialCellSpace;
//        }
//        else if (transform.parent.name.Equals(CommonNames.ROOT_GENERALSPACE))
//        {
//            myRenderer.material = Ignition.materialGeneralSpace;
//        }
//        else if (transform.parent.name.Equals(CommonNames.ROOT_TRANSITIONSPACE))
//        {
//            myRenderer.material = Ignition.materialTransitionSpace;
//        }

//        transform.gameObject.SetActive(true);
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
