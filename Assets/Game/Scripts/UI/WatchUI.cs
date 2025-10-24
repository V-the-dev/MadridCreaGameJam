using System;
using UnityEngine;
using UnityEngine.UI;

public class WatchUI : MonoBehaviour
{
    public float startAngleOffset = 0f;

    public Image watchHandle;
    
    private float currentTime;
    private float totalTime;

    private void Start()
    {
        currentTime = 0;
        totalTime = TimeManager.Instance.GetTotalTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (totalTime <= 0f) return;

        if (TimeManager.Instance.timeManipulated)
        {
            //Do something here to the current time
        }

        currentTime += Time.deltaTime;

        currentTime = Mathf.Clamp(currentTime, 0f, totalTime);

        float t = currentTime / totalTime;

        float angle = t * -360f;

        if (watchHandle != null)
        {
            watchHandle.rectTransform.localEulerAngles = new Vector3(0, 0, angle + startAngleOffset);
        }
        else
        {
            transform.localEulerAngles = new Vector3(0, 0, angle + startAngleOffset);
        }
    }

    public void ResetTimer()
    {
        currentTime = 0f;
    }
}
