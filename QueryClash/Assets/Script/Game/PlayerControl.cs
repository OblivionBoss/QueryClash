using FishNet.Object;
using UnityEngine;

public class PlayerControl : NetworkBehaviour
{
    [SerializeField] private Camera PlayerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            PlayerCamera = Camera.main;
            PlayerCamera.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            PlayerCamera.transform.SetParent(transform);
            PlayerCamera.transform.localRotation = Quaternion.Euler(60f, 0, 0);

            CraftingManager cm = GameObject.Find("CraftingManager").GetComponent<CraftingManager>();
            cm.isLeftTeam = ClientManager.Connection.IsHost;

            HpDisplay hpd = GameObject.Find("HP UI Manager").GetComponent<HpDisplay>();
            if (!ClientManager.Connection.IsHost) hpd.swapHpUi();

            RDBManager rdbm = GameObject.Find("RDBManager").GetComponent<RDBManager>();
            rdbm.isNetwork = true;
        }
        else { gameObject.GetComponent<PlayerControl>().enabled = false; }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.None; // Free the cursor
        Cursor.visible = true;
    }
}