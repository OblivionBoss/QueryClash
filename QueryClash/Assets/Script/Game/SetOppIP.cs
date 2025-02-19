using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FishNet.Transporting.Tugboat;
using FishNet;

public class SetOppIP : MonoBehaviour
{
    public Tugboat TB;

    //void Start()
    //{ 
    //    TB.SetClientAddress(GetOppIP.getOppIP.Opp_IP);
    //}

    public void OnClientAddress()
    {
        TB.SetClientAddress(GetOppIP.getOppIP.Opp_IP);
    }
}