using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonObjs
{
    public static string TAG_STATE = "TAG_STATE";
    public static string TAG_CELLSPACE = "TAG_CELLSPACE";
    public static string TAG_GENERALSPACE = "TAG_GENERALSPACE";
    public static string TAG_TRANSITIONSPACE = "TAG_TRANSITIONSPACE";
    public static string TAG_TRANSITION = "TAG_TRANSITION";
    public static string TAG_CELLSPACEBOUNDARY = "TAG_CELLSPACEBOUNDARY";
    public static string TAG_TEXTURESURFACE = "TAG_TEXTURESURFACE";


    public static string ROOT = "Root";
    public static string ROOT_CELLSPACE = "CellSpace";
    public static string ROOT_GENERALSPACE = "GeneralSpace";
    public static string ROOT_TRANSITIONSPACE = "TransitionSpace";
    public static string ROOT_CELLSPACEBOUNDARY = "CellSpaceBoundary";
    public static string ROOT_STATE = "State";
    public static string ROOT_TRANSITION = "Transition";

    public static GameObject gmlRoot;
    public static GameObject gmlRootCellSpace;
    public static GameObject gmlRootGeneralSpace;
    public static GameObject gmlRootTransitionSpace;
    public static GameObject gmlRootCellSpaceBoundary;
    public static GameObject gmlRootState;
    public static GameObject gmlRootTransition;
    public static GameObject gmlRootFloor;

    public static Material materialCellSpace;
    public static Material materialGeneralSpace;
    public static Material materialTransitionSpace;
    public static Material materialCellSpaceBoundary;
    public static Material materialState;
    public static Material materialTextureSurface;

    public static Material materialFloorCellSpace;
    public static Material materialFloorTransitionSpace;
    public static Material materialFloorGeneralSpace;


    private static Material materialLine;

    public static Shader shaderCullOFF;
    public static Shader shaderCullON;

    public static void Init()
    {
        gmlRoot = new GameObject(CommonNames.ROOT);
        gmlRootCellSpace = new GameObject(CommonNames.ROOT_CELLSPACE);
        gmlRootGeneralSpace = new GameObject(CommonNames.ROOT_GENERALSPACE);
        gmlRootTransitionSpace = new GameObject(CommonNames.ROOT_TRANSITIONSPACE);
        gmlRootCellSpaceBoundary = new GameObject(CommonNames.ROOT_CELLSPACEBOUNDARY);
        gmlRootState = new GameObject(CommonNames.ROOT_STATE);
        gmlRootTransition = new GameObject(CommonNames.ROOT_TRANSITION);
        gmlRootFloor = new GameObject(CommonNames.ROOT_FLOOR);

        gmlRootCellSpace.transform.parent = gmlRoot.transform;
        gmlRootGeneralSpace.transform.parent = gmlRoot.transform;
        gmlRootTransitionSpace.transform.parent = gmlRoot.transform;
        gmlRootCellSpaceBoundary.transform.parent = gmlRoot.transform;
        gmlRootState.transform.parent = gmlRoot.transform;
        gmlRootTransition.transform.parent = gmlRoot.transform;
        // gmlRootFloor.transform.parent = gmlRoot.transform;

        // Materials

        shaderCullON = Resources.Load("Materials/STEM_CullOn") as Shader;
        shaderCullOFF = Resources.Load("Materials/STEM_CullOFF") as Shader;

        materialCellSpace = Resources.Load("Materials/CellSpace", typeof(Material)) as Material;
        materialGeneralSpace = Resources.Load("Materials/GeneralSpace", typeof(Material)) as Material;
        materialTransitionSpace = Resources.Load("Materials/TransitionSpace", typeof(Material)) as Material;
        materialCellSpaceBoundary = Resources.Load("Materials/CellSpaceBoundary", typeof(Material)) as Material;
        materialState = Resources.Load("Materials/State", typeof(Material)) as Material;
        materialTextureSurface = new Material(Shader.Find("Unlit/Texture"));

        materialFloorCellSpace = Resources.Load("Materials/FloorCellSpace", typeof(Material)) as Material;
        materialFloorTransitionSpace = Resources.Load("Materials/FloorTransitionSpace", typeof(Material)) as Material;
        materialFloorGeneralSpace = Resources.Load("Materials/FloorGeneralSpace", typeof(Material)) as Material;

        materialCellSpace.shader = shaderCullOFF;
        materialGeneralSpace.shader = shaderCullOFF;
        materialTransitionSpace.shader = shaderCullOFF;
        materialCellSpaceBoundary.shader = shaderCullOFF;
    }
}
