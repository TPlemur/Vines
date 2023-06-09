using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyMapper : MonoBehaviour
{
    [Header("Menu texts")]
    [SerializeField] TMPro.TextMeshProUGUI forField;
    [SerializeField] TMPro.TextMeshProUGUI bacField;
    [SerializeField] TMPro.TextMeshProUGUI rigField;
    [SerializeField] TMPro.TextMeshProUGUI lefField;
    [SerializeField] TMPro.TextMeshProUGUI iteField;
    [SerializeField] TMPro.TextMeshProUGUI intField;
    [SerializeField] TMPro.TextMeshProUGUI croField;
    [SerializeField] TMPro.TextMeshProUGUI tecField;
    [SerializeField] Slider Imode;

    [Header("In game UI")]


    [SerializeField] TMPro.TextMeshProUGUI forwardUi;
    [SerializeField] TMPro.TextMeshProUGUI backwardUi;
    [SerializeField] TMPro.TextMeshProUGUI rightUi;
    [SerializeField] TMPro.TextMeshProUGUI leftUi;
    [SerializeField] TMPro.TextMeshProUGUI ItemWheelUi;
    [SerializeField] TMPro.TextMeshProUGUI InteractUi;
    [SerializeField] TMPro.TextMeshProUGUI techNoteUi;
    [SerializeField] string techNoteText = " - SHOW INFO";

    public static KeyCode forward;
    public static KeyCode backward;
    public static KeyCode right;
    public static KeyCode left;
    public static KeyCode itemWheel;
    public static KeyCode interact;
    public static KeyCode crouch;
    public static KeyCode techNotes;
    public static KeyCode primary = KeyCode.Mouse0;
    public static KeyCode cycleRightKey = KeyCode.E;
    public static KeyCode cycleLeftKey = KeyCode.Q;

    public static bool itemModeIsRel;

    public enum keys
    {
        forward,
        backward,
        right,
        left,
        itemWheel,
        interact,
        crouch,
        techNotes
    }
    keys currentKey;
    KeyCode lastKeyPresed;
    bool foundKey = false;

    int[] allCodes;

    public void playerKeyInput(float targetBinding)
    {
        currentKey = (keys)targetBinding;
        StartCoroutine(waitForKey());
    }

    //set saved keys, and get array of all keys
    private void Awake()
    {
        getKeyCodes();
        allCodes = (int[])System.Enum.GetValues(typeof(KeyCode));

        if (Imode != null)
        {
            Imode.value = PlayerPrefs.GetInt("itemMode", 1);
            StartCoroutine(lateAwake());
        }
    }

    IEnumerator lateAwake()
    {
        yield return new WaitForSeconds(0.25f);
        itemModeIsRel = (Imode.value == 1);
    }

    //check if any keys are pressed
    void CheckForKeyPress()
    {
        if (Input.anyKeyDown)
        {
            foreach (int i in allCodes)
            {
                if (Input.GetKey((KeyCode)i))
                {
                    lastKeyPresed = (KeyCode)i;
                    foundKey = true;
                    break;
                }
            }
        }
    }

    //waits for next keyPress and sets approprate key binding
    IEnumerator waitForKey()
    {
        while (!foundKey)
        {
            CheckForKeyPress();
            yield return 0;
        }
        setKeycode(lastKeyPresed);
        foundKey = false;
    }

    //set a specific keycode specified by currentKey and the new keycode
    void setKeycode(KeyCode newCode)
    {
        string empty = "";
        empty += newCode.ToString();
        switch (currentKey)
        {
            case keys.forward:
                PlayerPrefs.SetFloat("forward", (int)newCode);
                forField.text = empty;
                forward = newCode;
                forwardUi.text = forward.ToString();
                break;
            case keys.backward:
                PlayerPrefs.SetFloat("backward", (int)newCode);
                bacField.text = empty;
                backward = newCode;
                backwardUi.text = backward.ToString();
                break;
            case keys.right:
                PlayerPrefs.SetFloat("right", (int)newCode);
                rigField.text = empty;
                right = newCode;
                rightUi.text = right.ToString();
                break;
            case keys.left:
                PlayerPrefs.SetFloat("left", (int)newCode);
                lefField.text = empty;
                left = newCode;
                leftUi.text = left.ToString();
                break;
            case keys.itemWheel:
                PlayerPrefs.SetFloat("itemWheel", (int)newCode);
                iteField.text = empty;
                itemWheel = newCode;
                ItemWheelUi.text = itemWheel.ToString();
                break;
            case keys.interact:
                PlayerPrefs.SetFloat("interact", (int)newCode);
                intField.text = empty;
                interact = newCode;
                InteractUi.text = interact.ToString();
                break;
            case keys.crouch:
                PlayerPrefs.SetFloat("crouch", (int)newCode);
                croField.text = empty;
                crouch = newCode;
                break;
            case keys.techNotes:
                PlayerPrefs.SetFloat("techNotes", (int)newCode);
                tecField.text = empty;
                techNotes = newCode;
                techNoteUi.text = techNotes.ToString() + techNoteText;
                break;
        }
    }

    //fetch keycodes from Playerprefs
    void getKeyCodes()
    {
        getkeyCode(ref forward,   "forward",   KeyCode.W,           forField);
        getkeyCode(ref backward,  "backward",  KeyCode.S,           bacField);
        getkeyCode(ref right,     "right",     KeyCode.D,           rigField);
        getkeyCode(ref left,      "left",      KeyCode.A,           lefField);
        getkeyCode(ref itemWheel, "itemWheel", KeyCode.Q,           iteField);
        getkeyCode(ref interact,  "interact",  KeyCode.E,           intField);
        getkeyCode(ref crouch,    "crouch",    KeyCode.LeftShift,   croField);
        getkeyCode(ref techNotes, "techNotes", KeyCode.T,           tecField);
        if (forwardUi != null)
        {
            forwardUi.text = forward.ToString();
            backwardUi.text = backward.ToString();
            rightUi.text = right.ToString();
            leftUi.text = left.ToString();
            ItemWheelUi.text = itemWheel.ToString();
            InteractUi.text = interact.ToString();
            techNoteUi.text = techNotes.ToString() + techNoteText;
        }
    }

    //Helper function for getKeyCodes
    void getkeyCode(ref KeyCode target, string pPref, KeyCode defKey, TMPro.TextMeshProUGUI targetUI)
    {
        target = (KeyCode)PlayerPrefs.GetFloat(pPref, (int)defKey);
        if (targetUI != null)
        {
            targetUI.text = target.ToString();
        }
    }

    //changes ItemWheel mode
    public void itemWheelMode(float mode)
    {

        if(Imode.value == 1)
        {
            itemModeIsRel = true;
            PlayerPrefs.SetInt("itemMode", 1);
        }
        else
        {
            itemModeIsRel = false;
            PlayerPrefs.SetInt("itemMode", 0);
        }
    }


}
