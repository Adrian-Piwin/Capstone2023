using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypeOut : MonoBehaviour
{
    [Header("Type Settings")]
    public float typeSpeed;
    public float deleteSpeed;
    public float readWaitTime;
    public bool isTalking;

    public static TypeOut Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public IEnumerator Type(TextMeshProUGUI msgUI, string sentence, bool delete)
    {
        if (isTalking) yield break;
        isTalking = true;

        // Delete sentence if its there
        foreach (char c in msgUI.text)
        {
            msgUI.text = msgUI.text.Remove(msgUI.text.Length - 1, 1);
            yield return new WaitForSeconds(deleteSpeed);
        }

        // Type sentence
        foreach (char c in sentence)
        {
            msgUI.text = msgUI.text + c;
            yield return new WaitForSeconds(typeSpeed);
        }

        if (!delete)
        {
            isTalking = false;
            yield break;
        }

        yield return new WaitForSeconds(readWaitTime);

        // Delete sentence
        foreach (char c in sentence)
        {
            msgUI.text = msgUI.text.Remove(msgUI.text.Length - 1, 1);
            yield return new WaitForSeconds(deleteSpeed);
        }

        isTalking = false;
    }
}
