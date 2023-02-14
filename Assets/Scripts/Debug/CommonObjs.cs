using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommonObjs
{
    public static string TAG_STATE = "TAG_STATE";
    public static string TAG_CELLSPACE = "TAG_CELLSPACE";
    public static string TAG_GENERALSPACE = "TAG_GENERALSPACE";
    public static string TAG_TRANSITIONSPACE = "TAG_TRANSITIONSPACE";
    public static string TAG_CONNECTIONSPACE = "TAG_CONNECTIONSPACE";
    public static string TAG_ANCHORSPACE = "TAG_ANCHORSPACE";

    public static string TAG_TRANSITION = "TAG_TRANSITION";
    public static string TAG_CELLSPACEBOUNDARY = "TAG_CELLSPACEBOUNDARY";
    public static string TAG_TEXTURESURFACE = "TAG_TEXTURESURFACE";


    public static string ROOT = "Root";
    public static string ROOT_CELLSPACE = "CellSpace";
    public static string ROOT_GENERALSPACE = "GeneralSpace";
    public static string ROOT_TRANSITIONSPACE = "TransitionSpace";
    public static string ROOT_CONNECTIONSPACE = "ConnectionSpace";
    public static string ROOT_ANCHORSPACE = "AnchorSpace";

    public static string ROOT_CELLSPACEBOUNDARY = "CellSpaceBoundary";
    public static string ROOT_STATE = "State";
    public static string ROOT_TRANSITION = "Transition";

    public static GameObject gmlRoot;
    public static GameObject gmlRootCellSpace;
    public static GameObject gmlRootGeneralSpace;
    public static GameObject gmlRootTransitionSpace;

    public static GameObject gmlRootConnectionSpace;
    public static GameObject gmlRootAnchorSpace;


    public static GameObject gmlRootCellSpaceBoundary;
    public static GameObject gmlRootState;
    public static GameObject gmlRootTransition;
    public static GameObject gmlRootFloor;
    public static GameObject gmlRootID;

    public static Material materialCellSpace;
    public static Material materialGeneralSpace;
    public static Material materialTransitionSpace;
    public static Material materialConnectionSpace;
    public static Material materialAnchorSpace;


    public static Material materialCellSpaceBoundary;
    public static Material materialState;
    public static Material materialTextureSurface;

    private static Material materialLine;

    public static Shader shaderCullOFF;
    public static Shader shaderCullON;

    public static void Init()
    {
        gmlRoot = new GameObject(CommonNames.ROOT);
        gmlRootCellSpace = new GameObject(CommonNames.ROOT_CELLSPACE);
        gmlRootGeneralSpace = new GameObject(CommonNames.ROOT_GENERALSPACE);
        gmlRootTransitionSpace = new GameObject(CommonNames.ROOT_TRANSITIONSPACE);
        gmlRootConnectionSpace = new GameObject(CommonNames.ROOT_CONNECTIONSPACE);
        gmlRootAnchorSpace = new GameObject(CommonNames.ROOT_ANCHORSPACE);

        gmlRootCellSpaceBoundary = new GameObject(CommonNames.ROOT_CELLSPACEBOUNDARY);
        gmlRootState = new GameObject(CommonNames.ROOT_STATE);
        gmlRootTransition = new GameObject(CommonNames.ROOT_TRANSITION);
        gmlRootFloor = new GameObject(CommonNames.ROOT_FLOOR);
        gmlRootID = GameObject.Find("Root_ID");

        gmlRootCellSpace.transform.parent = gmlRoot.transform;
        gmlRootGeneralSpace.transform.parent = gmlRoot.transform;
        gmlRootTransitionSpace.transform.parent = gmlRoot.transform;
        gmlRootConnectionSpace.transform.parent = gmlRoot.transform;
        gmlRootAnchorSpace.transform.parent = gmlRoot.transform;
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

        materialConnectionSpace = Resources.Load("Materials/ConnectionSpace", typeof(Material)) as Material;
        materialAnchorSpace = Resources.Load("Materials/AnchorSpace", typeof(Material)) as Material;


        materialCellSpaceBoundary = Resources.Load("Materials/CellSpaceBoundary", typeof(Material)) as Material;
        materialState = Resources.Load("Materials/State", typeof(Material)) as Material;
        materialTextureSurface = new Material(Shader.Find("Unlit/Texture"));

        materialCellSpace.shader = shaderCullOFF;
        materialGeneralSpace.shader = shaderCullOFF;
        materialTransitionSpace.shader = shaderCullOFF;
        materialCellSpaceBoundary.shader = shaderCullOFF;
    }
}
