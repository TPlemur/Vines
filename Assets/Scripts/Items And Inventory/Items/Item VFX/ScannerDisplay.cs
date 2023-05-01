using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScannerDisplay : MonoBehaviour
{
    [SerializeField]Image display;

    /// <summary>
    /// sets display to col,row
    /// </summary>
    /// <param name="col"> int 0,1,2 indicating col </param>
    /// <param name="row"> int 1,2,3,4 indicating row </param>
    public void SetDisplay(int col, int row)
    {
        //fail if out of range
        if (col < 0 || col > 2||row<1||row>4)
        {
            Debug.Log("Invalid scanner display");
            return;
        }

        //assebmle path
        string path = "ScannerDisplay/";
        if (col == 0) { path += "L"; }
        else if (col == 1) { path += "M"; }
        else { path += "R"; }
        path += row.ToString();

        //replace sprite
        Sprite newSprite = Resources.Load<Sprite>(path);
        display.sprite = newSprite;
    }

    public void BlankDisplay()
    {
        display.sprite = Resources.Load<Sprite>("ScannerDisplay/Background");
    }

    //DebugTesting
    /*
    [SerializeField] int row = 1;
    [SerializeField] int col = 1;
    [SerializeField] bool isBlank;

    private void Update()
    {
        if (isBlank) { BlankDisplay(); }
        else { SetDisplay(col, row); }
    }
    */
}
