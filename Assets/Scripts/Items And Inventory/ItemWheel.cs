using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWheel : MonoBehaviour
{
    [SerializeField] InventoryManager inv;
    [SerializeField] playerCamera playerCam;
    [SerializeField] Color highlightColor;
    [SerializeField] float animTime = 0.125f;
    [SerializeField] int smoothingTicks = 10;
    [SerializeField] float backlashFactor = 1;
    [SerializeField] float mouseScrollThreshold = 0.9f;

    [Header("Items WedgeUI:")]
    [SerializeField] GameObject EWedge;
    [SerializeField] GameObject FWedge;
    [SerializeField] GameObject BWedge;
    [SerializeField] GameObject PWedge;
    [SerializeField] GameObject SWedge;

    Color baseColor;
    static KeyCode menuKey = KeyCode.Q;
    bool menuOpen = false;
    List<Vector2> PrevMouseInpts = new List<Vector2>();

    //struct to hold info about wedges
    struct itemWedge
    {
        public GameObject obj;
        public Image img;
        public bool inInv;
        public System.Type type;
    }
    //wedge list
    itemWedge[] wedges = new itemWedge[5];
    int activeWedge = -1;

    // Start is called before the first frame update
    void Start()
    {
        //link ui to item type
        wedges[0] = new itemWedge() { obj = EWedge, img = EWedge.GetComponent<Image>(), inInv = false, type = typeof(ElectricalEquipment) };
        wedges[4] = new itemWedge() { obj = FWedge, img = FWedge.GetComponent<Image>(), inInv = false, type = typeof(Flashlight) };
        wedges[3] = new itemWedge() { obj = BWedge, img = BWedge.GetComponent<Image>(), inInv = false, type = typeof(ScannerBeacon) };
        wedges[1] = new itemWedge() { obj = PWedge, img = PWedge.GetComponent<Image>(), inInv = false, type = typeof(PVTM) };
        wedges[2] = new itemWedge() { obj = SWedge, img = SWedge.GetComponent<Image>(), inInv = false, type = typeof(Shield) };
        //grab base color
        baseColor = wedges[0].img.color;
    }

    // Update is called once per frame
    void Update()
    {
        //open menu
        if (Input.GetKeyDown(menuKey) || Input.GetMouseButtonDown(2))
        {
            openMenu();
        }
        //run menu
        else if (Input.GetKey(menuKey) || Input.GetMouseButton(2))
        {
            runMenu();
        }
        //close Menu
        else if ((Input.GetKeyUp(menuKey) || Input.GetMouseButtonUp(2)) && menuOpen)
        {
            closeMenu();
        }

        if(Input.mouseScrollDelta.y > mouseScrollThreshold)
        {
            StartCoroutine(mouseWheelScroll(true));
        }
        else if(Input.mouseScrollDelta.y < -mouseScrollThreshold)
        {
            StartCoroutine(mouseWheelScroll(false));
        }
    }

    //open the menu
    void openMenu()
    {
        //dissable playerCam look
        menuOpen = true;
        playerCam.enabled = false;
        //Find and activate which items are in inventory, and highlight the currently equiped
        for (int i = 0; i < wedges.Length; i++)
        {
            wedges[i].inInv = inv.Has(wedges[i].type);
            if (wedges[i].inInv) { wedges[i].obj.SetActive(true); }
            if (wedges[i].type == inv.typeofCurrent()) { activeWedge = i; }
        }
        StartCoroutine(LerpWedgeColor(activeWedge, baseColor, highlightColor));
    }

    //closes the menu
    void closeMenu()
    {
        menuOpen = false;
        playerCam.enabled = true;
        inv.setActiveItem(wedges[activeWedge].type);
        foreach (itemWedge w in wedges)
        {
            w.img.color = baseColor;
            w.obj.SetActive(false);
        }
    }

    //lerp the highlight of the selected wedge
    IEnumerator LerpWedgeColor(int i, Color start, Color end)
    {
        float timer = 0;
        while (timer < animTime)
        {
            timer += Time.deltaTime;
            wedges[i].img.color = Color.Lerp(start, end, timer / animTime);
            yield return 0;
        }
        wedges[i].img.color = end;
    }

    //run the item selection
    void runMenu()
    {
        //Get mouse input avg over a number of ticks
        PrevMouseInpts.Add(new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")));
        if (PrevMouseInpts.Count > smoothingTicks) { PrevMouseInpts.RemoveAt(0); }

        Vector2 mouseInput = new Vector2();
        foreach (Vector2 v in PrevMouseInpts) { mouseInput += v; }

        if (mouseInput.magnitude > backlashFactor)
        {
            //find angle from up and right vectors
            float angleUp = Vector2.Angle(new Vector2(0, 1), mouseInput);
            float angleRight = Vector2.Angle(new Vector2(1, 0), mouseInput);
            //Use this lookup table to match item to angles
            //case: up  -- angleUp < 30                  , EE - 0
            //case: dwn -- angleUp > 150                 , N/A
            //case: upR -- angleUp < 90  , angleR < 60   , PV - 1
            //case: upL -- angleUp < 90  , angleR > 120  , FL - 4
            //case: doR -- angleUp > 90  , angleR < 60   , SH - 2
            //case: doL -- angleUp > 90  , angleR > 120  , BE - 3
            int newActive = -1;
            if (angleUp < 30) { newActive = 0; }                         //set EE to active 
            else if (angleUp > 150) { newActive = activeWedge; }        //keep current wedge active
            else if (angleUp < 90 && angleRight < 60) { newActive = 1; } //set pvtm to active
            else if (angleUp < 90 && angleRight > 120) { newActive = 4; }//set flash to active
            else if (angleUp > 90 && angleRight < 60) { newActive = 2; } //set shield to active
            else { newActive = 3; }                                     //set beacon to active

            //change active if necessasary
            if (newActive != activeWedge && wedges[newActive].inInv)
            {
                StartCoroutine(LerpWedgeColor(activeWedge, highlightColor, baseColor));
                activeWedge = newActive;
                StartCoroutine(LerpWedgeColor(activeWedge, baseColor, highlightColor));
            }
        }
    }


    //scroll to next valid item, upOrDown being +1 or -1 to determine direction
    IEnumerator mouseWheelScroll(bool Positive)
    {
        //pop the menu open quickly
        for (int i = 0; i < wedges.Length; i++)
        {
            wedges[i].inInv = inv.Has(wedges[i].type);
            if (wedges[i].inInv) { wedges[i].obj.SetActive(true); }
            if (wedges[i].type == inv.typeofCurrent()) { activeWedge = i; }
        }
        StartCoroutine(LerpWedgeColor(activeWedge, baseColor, highlightColor));
        yield return new WaitForSeconds(animTime);

        //find new activeWedge
        int newWedge = 0;
        if (Positive)
        {
            newWedge = activeWedge + 1;
            while(!wedges[newWedge % wedges.Length].inInv) { newWedge++; }
            newWedge = newWedge % wedges.Length;
        }
        else
        {
            newWedge = activeWedge - 1;
            if(newWedge == -1) { newWedge = wedges.Length - 1; }
            while (!wedges[newWedge % wedges.Length].inInv) { newWedge--; }
            newWedge = newWedge % wedges.Length;
        }


        StartCoroutine(LerpWedgeColor(newWedge, baseColor, highlightColor));
        StartCoroutine(LerpWedgeColor(activeWedge, highlightColor, baseColor));
        activeWedge = newWedge;
        yield return new WaitForSeconds(animTime);

        //close the menu
        inv.setActiveItem(wedges[activeWedge].type);
        foreach (itemWedge w in wedges)
        {
            w.img.color = baseColor;
            w.obj.SetActive(false);
        }
    }

}
