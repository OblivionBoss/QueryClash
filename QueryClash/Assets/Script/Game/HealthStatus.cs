using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object.Synchronizing;
using FishNet.Object;
using TMPro;

public class HealthStatus : NetworkBehaviour
{
    private readonly SyncVar<int> Health = new SyncVar<int>(100); 
    private TextMeshProUGUI healthText;

    private void Start() { healthText = GameObject.FindGameObjectWithTag("HealthText").GetComponent<TextMeshProUGUI>(); }

    private void Update()
    {
        if (!base.IsOwner) return;
        healthText.text = Health.Value.ToString();
    }
}
