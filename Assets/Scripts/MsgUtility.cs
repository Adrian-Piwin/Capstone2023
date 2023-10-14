using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public enum MsgType
{
    Error,
    Success,
    Info
}

public static class MsgTypeExtensions
{
    private static readonly Dictionary<MsgType, string> TypeColors = new Dictionary<MsgType, string>
    {
        { MsgType.Error, "#d9534f" },     
        { MsgType.Success, "#5cb85c" },   
        { MsgType.Info, "#5bc0de" }       
    };

    public static string GetColor(this MsgType messageType)
    {
        if (TypeColors.TryGetValue(messageType, out var color))
        {
            return color;
        }
        return "#000000"; // Default to black if the type is not found.
    }
}

public class MsgUtility : MonoBehaviour
{
    public static MsgUtility instance;
    public TextMeshProUGUI msgDisplay;
    public Image background;
    private Animator animator;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void DisplayMsg(string msg, MsgType type = MsgType.Info)
    {
        background.color = HexToColor(type.GetColor());
        msgDisplay.text = msg;
        animator.Play("DisplayMsg");
    }

    Color HexToColor(string hexColor)
    {
        Color color = Color.black; 

        if (ColorUtility.TryParseHtmlString(hexColor, out color))
        {
            return color;
        }

        return color;
    }
}