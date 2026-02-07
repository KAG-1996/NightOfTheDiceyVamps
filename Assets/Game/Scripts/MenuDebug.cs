using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDebug : MonoBehaviour
{
    public void Z_OC(GameObject go)
    {
        go.SetActive(!go.activeInHierarchy);
    }
}
