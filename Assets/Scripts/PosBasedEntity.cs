using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//                        , SOLID    , SOLID       , SOLID          , POLYGON            POINT, LINESTRING
enum DATA_TYPE { Undefined, CELLSPACE, GENERALSPACE, TRANSITIONSPACE, CELLSPACEBOUNDARY, STATE, TRANSITION };

class PosBasedEntity
{
    public PosBasedEntity(string pId, string name, DATA_TYPE type)
    {
        id = pId;
        localName = name;
        spaceType = type;

        texture = string.Empty;

        exterior = new List<Vector3>();
        interiors = new List<List<Vector3>>();
        //interiors.Add(new List<Vector3>());

        texture_coordinates = new List<Vector2>();
    }
    public string id { get; }
    public string localName { get; }
    public DATA_TYPE spaceType { get; }
    public Vector3 center { get; }
    public List<Vector3> exterior { get; set; }
    public List<List<Vector3>> interiors { get; set; }

    public string texture { get; set; }
    public List<Vector2> texture_coordinates { get; set; }
}