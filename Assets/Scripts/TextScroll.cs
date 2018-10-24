using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextScroll : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private float letterTime;
    [SerializeField] private string advanceAxis;
    #region Debug
    [SerializeField] private string[] testStrings;
    [SerializeField] private bool sendStrings;
    #endregion
    

    private float triggerTime;
    private int textboxes;
    private bool complete;
    private bool isActive;
    private string[] textStrings;
    private System.Action completeAction;

    void Awake()
    {
        textboxes = 0;
        isActive = false;
    }

    void Start()
    {
        text.enabled = false;
    }

    void Update()
    {
        bool advanceDown = Input.GetButtonDown(advanceAxis);
        #region Debug
        if(sendStrings)
        {
            ScrollText(testStrings);
        }
        sendStrings = false;
        #endregion

        if(isActive)
        {
            if(!text.enabled)
            {
                text.enabled = true;
            }
            int maxLength = textStrings[textStrings.Length - textboxes].Length;

            int length = (int)((Time.time - triggerTime)/letterTime);

            if(length > maxLength || complete)
            {
                if(advanceDown)
                {
                    if(textboxes == 1)
                    {
                        isActive = false;
                        text.enabled = false;
                        if(completeAction != null)
                        {
                            completeAction();
                        }
                    }
                    else
                    {
                        --textboxes;
                        triggerTime = Time.time;
                    }
                    complete = false;
                }
            }
            else
            {
                if(advanceDown)
                {
                    text.text = textStrings[textStrings.Length - textboxes];
                    complete = true;
                }
                else
                {
                    text.text = textStrings[textStrings.Length - textboxes].Substring(0, length);
                }
            }
        }
    }

    public void ScrollText(string[] text, System.Action completeCallback = null)
    {
        isActive = true;
        textStrings = text;
        textboxes = textStrings.Length;
        triggerTime = Time.time;
        complete = false;
        completeAction = completeCallback;
    }
}
