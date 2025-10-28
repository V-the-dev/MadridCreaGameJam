using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Video;

public class VidPlayer : MonoBehaviour
{
    public string videoFileName;
    
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = videoPlayer.GetComponent<VideoPlayer>();
    }

    private void OnEnable()
    {
        PlayVideo();
    }

    public void PlayVideo()
    {
        if (videoPlayer)
        {
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }
}
