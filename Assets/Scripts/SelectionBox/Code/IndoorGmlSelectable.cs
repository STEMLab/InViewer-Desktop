//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.UI.Extensions;
//using System.Collections;

//public class IndoorGmlSelectable : MonoBehaviour, IBoxSelectable {

//	#region Implemented members of IBoxSelectable
//	bool _selected = false;
//	public bool selected {
//		get {
//			return _selected;
//		}

//		set {
//			_selected = value;
//		}
//	}

//	bool _preSelected = false;
//	public bool preSelected {
//		get {
//			return _preSelected;
//		}
		
//		set {
//			_preSelected = value;
//		}
//	}
//    #endregion

//    Material myNewMaterial;
//    Color myOriginalMaterialColor;

//    void Awake()
//    {
//        if (transform.parent.name.Equals(CommonNames.ROOT_CELLSPACE))
//        {
//            myOriginalMaterialColor = Ignition.materialCellSpace.color;
//        }
//        else if (transform.parent.name.Equals(CommonNames.ROOT_CELLSPACEBOUNDARY))
//        {
//            myOriginalMaterialColor = Ignition.materialCellSpaceBoundary.color;
//        }
//        else if (transform.parent.name.Equals(CommonNames.ROOT_GENERALSPACE))
//        {
//            myOriginalMaterialColor = Ignition.materialGeneralSpace.color;
//        }
//        else if (transform.parent.name.Equals(CommonNames.ROOT_STATE))
//        {
//            myOriginalMaterialColor = Ignition.materialState.color;
//        }
//        else if (transform.parent.name.Equals(CommonNames.ROOT_TRANSITIONSPACE))
//        {
//            myOriginalMaterialColor = Ignition.materialTransitionSpace.color;
//        }
//        else
//        {
//            myOriginalMaterialColor = Color.white;
//        }
//    }

//	void Start ()
//    {
//        if (GetComponent<Renderer>() == null) return;
//        myNewMaterial = GetComponent<Renderer>().material;
//    }

//    void Update () {
//        if (GetComponent<Renderer>() == null) return;

//        Color color = myOriginalMaterialColor;

//        if (preSelected) {
//			color = Color.yellow;
//		}
//		if (selected) {
//			color = Color.green;
//		}

//        myNewMaterial.color = color;
//	}
//}
