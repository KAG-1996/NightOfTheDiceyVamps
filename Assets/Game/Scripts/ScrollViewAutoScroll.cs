using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewAutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect; // Assign your ScrollRect here
    private GameObject previouslySelected;
    public float _yOffset = 0.5f;

    void Update()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

        if (currentSelected != null && currentSelected != previouslySelected && currentSelected.transform.parent == transform)
        {
            // Check if the selected object is a child of this content transform
            RectTransform selectedRectTransform = currentSelected.GetComponent<RectTransform>();
            if (selectedRectTransform != null)
            {
                // Calculate the position to scroll to
                Vector2 targetPosition = (Vector2)scrollRect.transform.InverseTransformPoint(scrollRect.content.position) -
                                        (Vector2)scrollRect.transform.InverseTransformPoint(selectedRectTransform.position);

                // Adjust for pivot and size of the selected item
                targetPosition.y += selectedRectTransform.rect.height * (_yOffset - selectedRectTransform.pivot.y);

                scrollRect.content.anchoredPosition = new Vector2(scrollRect.content.anchoredPosition.x, targetPosition.y);
            }
            previouslySelected = currentSelected;
        }
    }
}
