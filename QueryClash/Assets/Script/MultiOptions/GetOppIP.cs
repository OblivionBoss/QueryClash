using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using FishNet.Transporting.Tugboat;
using FishNet;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

public class GetOppIP : MonoBehaviour
{
    public static GetOppIP getOppIP;
    public TMP_InputField inputField;
    public string Opp_IP;

    private void Awake()
    {
        if (getOppIP == null) { getOppIP = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void GetIPv4()
    {
        Opp_IP = inputField.text;
        SceneManager.LoadSceneAsync("BossWaitingRoom");//SceneManager.LoadSceneAsync("GameFNetwork");
    }

    public void HostGame()
    {
        Opp_IP = GetLocalIPv4(); // Get the local IPv4 address
        inputField.text = Opp_IP; // Update input field (optional)
        Debug.Log("Hosting Game with IP: " + Opp_IP);

        // Load the game scene as a host
        SceneManager.LoadSceneAsync("BossWaitingRoom");//SceneManager.LoadSceneAsync("GameFNetwork");
    }

    private string GetLocalIPv4()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Ignore virtual adapters (like ZeroTier)
            if (ni.Description.ToLower().Contains("zerotier") || ni.Name.ToLower().Contains("zerotier"))
                continue;

            if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }
        }
        return "127.0.0.1"; // Default to localhost if no valid IP found
    }
}

