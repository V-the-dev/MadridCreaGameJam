using System;
using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    public string videoURL;

    public void PlayVideo()
    {
        VideoPlayer videoPlayer = GetComponent<VideoPlayer>();
        
        if (videoPlayer)
        {
            videoPlayer.url = videoURL;
            videoPlayer.Play();
        }
    }
}
