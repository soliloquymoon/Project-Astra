using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    public float noteSpeed = 400;
    public float noteStartTime;
    public float targetTime;     // Target time the note should arrive
    public float startX;         // Initial x
    public float targetX = -650; // Target x (hit line)
    private int noteKey;
    public Image img;

    // Update is called once per frame
    void Update()
    {
        float musicTime = AudioManager.instance.GetMusicTime(); // 현재 음악 시간
        float progress = (musicTime - noteStartTime) / (targetTime - noteStartTime); // 진행률 (0~1)
        
        if (progress < 1.0f) 
        {
            // Move towards the hit line until the target time
            float newPositionX = Mathf.Lerp(startX, targetX, Mathf.Clamp01(progress));
            transform.localPosition = new Vector3(newPositionX, transform.localPosition.y, transform.localPosition.z);
        }
        else 
        {
            // Uniform linear motion after reaching the hit line
            transform.localPosition += Vector3.left * noteSpeed * Time.deltaTime;
        }

        // Destroy if the Note object passes beyond the hit line (-700f)
        if (transform.localPosition.x <= -700f)
            Destroy(this.gameObject);
    }

    public void setSprite(Sprite sprite)
    {
        this.img.sprite = sprite;
    }
}
