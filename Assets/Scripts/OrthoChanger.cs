using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthoChanger : MonoBehaviour
{
    public void UpdateOrthoSize(string size)
    {
        Debug.Log("LOG: " + size);
        int outSize = 120;
        int.TryParse(size, out outSize);
        GetComponent<Camera>().orthographicSize = outSize;
    }
}
