using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyMapper : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI forField;
    [SerializeField] TMPro.TextMeshProUGUI bacField;
    [SerializeField] TMPro.TextMeshProUGUI rigField;
    [SerializeField] TMPro.TextMeshProUGUI lefField;
    [SerializeField] TMPro.TextMeshProUGUI iteField;
    [SerializeField] TMPro.TextMeshProUGUI intField;
    [SerializeField] TMPro.TextMeshProUGUI croField;

    public static KeyCode forward;
    public static KeyCode backward;
    public static KeyCode right;
    public static KeyCode left;
    public static KeyCode itemWheel;
    public static KeyCode interact;
    public static KeyCode crouch;

    public enum keys
    {
        forward,
        backward,
        right,
        left,
        itemWheel,
        interact,
        crouch
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
                break;
            case keys.backward:
                PlayerPrefs.SetFloat("backward", (int)newCode);
                bacField.text = empty;
                backward = newCode;
                break;
            case keys.right:
                PlayerPrefs.SetFloat("right", (int)newCode);
                rigField.text = empty;
                right = newCode;
                break;
            case keys.left:
                PlayerPrefs.SetFloat("left", (int)newCode);
                lefField.text = empty;
                left = newCode;
                break;
            case keys.itemWheel:
                PlayerPrefs.SetFloat("itemWheel", (int)newCode);
                iteField.text = empty;
                itemWheel = newCode;
                break;
            case keys.interact:
                PlayerPrefs.SetFloat("interact", (int)newCode);
                intField.text = empty;
                interact = newCode;
                break;
            case keys.crouch:
                PlayerPrefs.SetFloat("crouch", (int)newCode);
                croField.text = empty;
                crouch = newCode;
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
    }

    //Helper function for getKeyCodes
    void getkeyCode(ref KeyCode target, string pPref, KeyCode defKey, TMPro.TextMeshProUGUI targetUI)
    {
        target = (KeyCode)PlayerPrefs.GetFloat(pPref, (int)defKey);
        targetUI.text = target.ToString();
    }




}
