using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;

public abstract class Bullet : NetworkBehaviour
{
    public float bulletspeed = 1;
    public float deadZone {  get;  set; }
    public readonly SyncVar<float> Atk = new SyncVar<float>();
    public Vector3 Direction { get; set; }
    public string enemyTag { get; set; }
    public string thisTag { get; set; }

    public readonly SyncVar<bool> isBuff = new SyncVar<bool>(false);

    [Server]
    public virtual void Initialize(float atk, float deadZone, Vector3 direction, string enemyTag, string thisTag)
    {
        Atk.Value = atk;
        this.deadZone = deadZone;
        Direction = direction;
        this.enemyTag = enemyTag;
        this.thisTag = thisTag;
        gameObject.tag = this.thisTag;
        Debug.Log("This bullet Atk = " + Atk.Value);
    }

    public abstract void Move();

    private void Update()
    {
        if (!ClientManager.Connection.IsHost) return;
        Move();
        CheckDeadZone();
    }

    [Server]
    protected void CheckDeadZone()
    {
        if (Direction == Vector3.left && transform.position.x < deadZone)
        {
            ServerManager.Despawn(gameObject);
        }
        else if (Direction == Vector3.right && transform.position.x > deadZone)
        {
            ServerManager.Despawn(gameObject);
        }
    }

    [Server]
    private void OnTriggerEnter(Collider collision)
    {
        if (ClientManager.Connection.IsHost)
            HandleCollision(collision);
    }

    [Server]
    protected virtual void HandleCollision(Collider collision)
    {
        return;
    }
}