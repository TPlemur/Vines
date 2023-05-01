using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Fade Controller is a Monobehavior that can fade
 * in and out of any scene. If a scene already has
 * a canvas object, then the controller will use it.
 * Otherwise it will make it's own canvas.
 * 
 * The 'proper' way to create this controller is via
 * using this.AddComponent<FadeController>() over
 * a declared FadeController variable.
 */
public class FadeController : MonoBehaviour{
    private GameObject canvasHolder;
    private Canvas canvas;
    private GameObject fadeHolder;
    private Image fadeImage;
    private AnimationCurve fadeProgression;
    private CanvasGroup group;
    private float currentTime;

    /* Awake() will find and create the FadeController's
     * necessary objects for transistions.
     */
    void Awake()
    {
        canvas = GameObject.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            canvasHolder = new GameObject();
            canvas = canvasHolder.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }
        fadeHolder = new GameObject();
        fadeHolder.transform.SetParent(canvas.transform, false);
        group = fadeHolder.AddComponent<CanvasGroup>();
        group.alpha = 0;
        fadeImage = fadeHolder.AddComponent<Image>();
        fadeImage.color = Color.black;
        fadeImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    // FadeIn() will create a fade in transition over secondsToFade seconds
    public void FadeIn(float secondsToFade)
    {
        group.alpha = 1;
        group.blocksRaycasts = false;
        fadeProgression = AnimationCurve.EaseInOut(0, 1, secondsToFade, 0);
        currentTime = 0;
        StartCoroutine(Fading());
    }

    // FadeOut() will create a fade out transition over secondsToFade seconds
    public void FadeOut(float secondsToFade)
    {
        group.alpha = 0;
        group.blocksRaycasts = true;
        fadeProgression = AnimationCurve.EaseInOut(0, 0, secondsToFade, 1);
        currentTime = 0;
        StartCoroutine(Fading());
    }

    // Same as FadeOut with will also tranistion to a scene
    public void FadeOutToSceen(float secondsToFade, int sceneNumber)
    {
        FadeOut(secondsToFade);
        StartCoroutine(LoadSceneTimer(secondsToFade, sceneNumber));
    }

    /* Fading() is a coroutine that that changes the
     * fadeHolder's alpha over the values in fadeProgression
     * using the currentTime
     */
    private IEnumerator Fading()
    {
        currentTime += Time.deltaTime;
        group.alpha = fadeProgression.Evaluate(currentTime);
        while (group.alpha < 1 && group.alpha > 0)
        {
            yield return null;
            currentTime += Time.deltaTime;
            group.alpha = fadeProgression.Evaluate(currentTime);
        }
    }

    //Loads scene over the time secondsToSceen
    private IEnumerator LoadSceneTimer(float secondsToSceen, int sceneNumber)
    {
        yield return new WaitForSeconds(secondsToSceen);
        SceneManager.LoadScene(sceneNumber);
    }
}
