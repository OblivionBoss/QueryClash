using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayHostIP : MonoBehaviour
{
    public TMP_Text ipDisplayText; // Assign this in the Inspector

    private void Start()
    {
        if (ipDisplayText != null)
        {
            ipDisplayText.text = "Host IP: " + GetOppIP.getOppIP.Opp_IP;
        }
    }
}

