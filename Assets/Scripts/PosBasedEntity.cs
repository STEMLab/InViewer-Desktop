using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//                        , SOLID    , SOLID       , SOLID          , POLYGON            POINT, LINESTRING
enum DATA_TYPE { Undefined, CELLSPACE, GENERALSPACE, TRANSITIONSPACE, CELLSPACEBOUNDARY, STATE, TRANSITION };

class PosBasedEntity
{
    public PosBasedEntity(string pId, string name, DATA_TYPE type, bool isInterior = false)
    {
        id = pId;
        localName = name;
        spaceType = type;
        interior = isInterior;

        vertices = new List<Vector3>();
    }
    public string id { get; }
    public string localName { get; }
    public DATA_TYPE spaceType { get; }
    public bool interior { get; }

    public List<Vector3> vertices { get; set; }
}