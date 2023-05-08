using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessChanger : MonoBehaviour
{
    float preGenBrightness;
    float postGenBrightness;

    [SerializeField] Slider preSlide;
    [SerializeField] Slider postSlide;

    // Start is called before the first frame update
    void Start()
    {
        preGenBrightness = PlayerPrefs.GetFloat("preGenBrightness", 5);
        postGenBrightness = PlayerPrefs.GetFloat("postGenBrightness", 160);
        preSlide.value = preGenBrightness;
        postSlide.value = postGenBrightness;
        RenderSettings.ambientLight = new Color(preGenBrightness / 255f, preGenBrightness / 255f, preGenBrightness / 255f);
    }

    public void setPre(float value)
    {
        preGenBrightness = value;
        PlayerPrefs.SetFloat("preGenBrightness", value);
        if (!GameStateManager.GeneratorOn)
        {
            RenderSettings.ambientLight = new Color(preGenBrightness / 255f, preGenBrightness / 255f, preGenBrightness / 255f);

        }
    }

    public void setPost(float value)
    {
        postGenBrightness = value;
        PlayerPrefs.SetFloat("postGenBrightness", value);
        if (GameStateManager.GeneratorOn)
        {
            RenderSettings.ambientLight = new Color(postGenBrightness / 255f, postGenBrightness / 255f, postGenBrightness / 255f);

        }
    }
}
