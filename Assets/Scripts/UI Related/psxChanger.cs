using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class psxChanger : MonoBehaviour
{
    public Material psxMaterial;
    public Slider psxSlider;

    private void Start()
    {
        RenderTexture psx = psxMaterial.mainTexture as RenderTexture;
        psx.Release();
        psx.width = 142 * (int)PlayerPrefs.GetFloat("psxQuality", 3);
        psx.height = 80 * (int)PlayerPrefs.GetFloat("psxQuality", 3);
        psxSlider.value = PlayerPrefs.GetFloat("psxQuality", 3);
    }

    public void PSXSetQuality(float value)
    {
        //takes the psx testure as a RenderTexture and edits it
        RenderTexture psx = psxMaterial.mainTexture as RenderTexture;
        psx.Release();
        psx.width = (int)(142 * value);
        psx.height = (int)(80 * value);
        PlayerPrefs.SetFloat("psxQuality", value);
        PlayerPrefs.Save();
    }
}
