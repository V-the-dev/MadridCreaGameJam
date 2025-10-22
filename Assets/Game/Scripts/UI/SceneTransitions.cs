using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitions : MonoBehaviour
{
    public Animator transitionAnim;

    public void LoadScene(string sceneToLoad)
    {
        StartCoroutine(StartSceneTransition(sceneToLoad));
    }
    
    IEnumerator StartSceneTransition(string sceneToLoad)
    {
        transitionAnim.SetTrigger("end");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
