using UnityEngine;
using UnityEngine.UI;

public class Cursor : MonoBehaviour
{
    private RectTransform canvasRect;
    private RectTransform rectTransform;

    void Start()
    {
        canvasRect = this.transform.parent.GetComponent<RectTransform>();
        rectTransform = this.GetComponent<RectTransform>();
    }

    void Update()
    {
        FollowMouse();
    }

    void FollowMouse()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 localPoint;

        // Convert mouse position to local position of Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePos, null, out localPoint);

        // Limit the mouse position to stay inside of the screen
        float halfHeight = rectTransform.rect.height * 0.5f;
        float minY = -canvasRect.rect.height * 0.5f + halfHeight;
        float maxY = canvasRect.rect.height * 0.5f - halfHeight;

        float clampedY = Mathf.Clamp(localPoint.y, minY, maxY);
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, clampedY);
    }
}