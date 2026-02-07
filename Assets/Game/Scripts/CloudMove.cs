using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloudMove : MonoBehaviour
{
    public float moveSpeed = 50f; // Speed of movement
    public Image[] _clouds; // Assign your Image component in the Inspector

    // Update is called once per frame
    void Update()
    {
        foreach(var a in _clouds)
        {
            a.rectTransform.anchoredPosition += new Vector2(moveSpeed * Time.deltaTime, 0);
            if (a.rectTransform.anchoredPosition.x > 1400)
                a.rectTransform.anchoredPosition = new Vector2(-1400, a.rectTransform.anchoredPosition.y);
        }
    }
}
