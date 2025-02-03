using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FishNet.Transporting.Tugboat;
using FishNet;

public class GetOppIP : MonoBehaviour
{
    public static GetOppIP getOppIP;
    public TMP_InputField inputField;
    public string Opp_IP;
    public Tugboat TG;

    private void Awake() {
        if (getOppIP == null) { getOppIP = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void GetIPv4() { 
        Opp_IP = inputField.text;
        Opp_IP = "localhost";
        //TG.SetClientAddress(Opp_IP);
        //InstanceFinder.NetworkManager.GetComponent<Tugboat>().SetClientAddress(Opp_IP);
        //InstanceFinder.NetworkManager.GetComponent<Tugboat>().SetClientAddress();
        SceneManager.LoadSceneAsync("GameFNetwork"); //SceneManager.LoadSceneAsync("Game"); 
    }
}
