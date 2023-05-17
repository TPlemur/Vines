using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveToggle : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Image sliderFill;
    [SerializeField] float objHeight;
    [SerializeField] float maxSliderVis = 200;
    public ObjectiveScript.ojbectives objectiveType;
    public float animationTime = 1;

    public float popIn(float yoffset)
    {
        gameObject.SetActive(true);
        RectTransform rt = transform.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, rt.anchoredPosition.y + yoffset);
        StartCoroutine(popInNumer());
        return objHeight;
    }

    public void adjustY(float yoffSet)
    {
        RectTransform rt = transform.GetComponent<RectTransform>();
        StartCoroutine(yAdj(rt, yoffSet, animationTime));
    }

    public float popOut()
    {
        StartCoroutine(popOutNumer());
        return objHeight;
    }

    IEnumerator yAdj(RectTransform rt,float yAdj, float time)
    {
        yield return new WaitForSeconds(time);
        float oldY = rt.anchoredPosition.y;
        float newY = rt.anchoredPosition.y + yAdj;
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, Mathf.Lerp(oldY, newY, timer / time));
            yield return 0;
        }
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, newY);
    }

    IEnumerator popOutNumer()
    {
        StartCoroutine(lerpSliderVis(animationTime / 2, 0, maxSliderVis / 255));
        yield return new WaitForSeconds(animationTime / 2);
        StartCoroutine(lerpSliderVal(animationTime / 2, 1, 0));
        yield return new WaitForSeconds(animationTime / 2);
        this.gameObject.SetActive(false);
    }

    //do the pop in animation
    IEnumerator popInNumer()
    {
        StartCoroutine(lerpSliderVal(animationTime / 2, 0, 1));
        yield return new WaitForSeconds(animationTime / 2);
        StartCoroutine(lerpSliderVis(animationTime / 2, maxSliderVis / 255,0));
    }

    //Lerps slider value from start to end over time
    IEnumerator lerpSliderVal(float time, float start, float end)
    {
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            slider.value = Mathf.Lerp(start, end, timer / time);
            yield return 0;
        }
        slider.value = end;
    }

    //Lerps slider alpha from start to end over time
    IEnumerator lerpSliderVis(float time, float start, float end)
    {
        float timer = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            sliderFill.color = new Color(sliderFill.color.r, sliderFill.color.g, sliderFill.color.b, Mathf.Lerp(start, end, timer / time));
            yield return 0;
        }
        sliderFill.color = new Color(sliderFill.color.r, sliderFill.color.g, sliderFill.color.b, end);
    }

    // Start is called before the first frame update
    void Start()
    {
        popIn(5);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
