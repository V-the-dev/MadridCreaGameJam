using UnityEngine;
using UnityEngine.UI;

public class WatchUI : MonoBehaviour
{
    public float startAngleOffset = 0f;

    public Image watchHandle;
    public Image watchBG;
    
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
            currentTime += Time.deltaTime * TimeManager.Instance.timeMultipliyer;
        }
        else
        {
            currentTime += Time.deltaTime;
        }


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
        TimeManager.Instance.timeManipulated = false;
    }

    public void ToogleVisibility()
    {
        watchBG.enabled = true;
        watchHandle.enabled = true;
    }

    public void SetVisibility(bool visibility)
    {
        watchBG.enabled = visibility;
        watchHandle.enabled = visibility;
    }
}
