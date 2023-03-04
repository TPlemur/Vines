using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonHiover : MonoBehaviour
{
    public Material Hovered;
    public Material UnHovered;
    public void OnMouseEnter()
    {
        this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().canvasRenderer.SetMaterial(Hovered, 0);
    }

    public void OnMouseExit()
    {
        this.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().canvasRenderer.SetMaterial(UnHovered, 0);
    }
}
