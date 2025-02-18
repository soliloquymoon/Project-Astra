using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    // "1920(tick)#03(lane number)00(notetype)"
    public float noteSpeed = 400;
    private int laneNumber;
    private int noteType;
    private int noteKey;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition -= Vector3.right * noteSpeed * Time.deltaTime;

        // Destroy if the Note object reaches the anchored position -600f
        if (this.GetComponent<RectTransform>().anchoredPosition.x <= -600f)
            Destroy(this.gameObject);
    }
}
