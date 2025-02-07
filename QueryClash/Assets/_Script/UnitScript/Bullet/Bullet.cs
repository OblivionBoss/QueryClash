using UnityEngine;
using FishNet.Object;

public abstract class Bullet : NetworkBehaviour
{
    public float bulletspeed = 1;
    public float deadZone {  get;  set; }
    public float Atk { get; set; }
    public Vector3 Direction { get; set; }
    public string enemyTag { get; set; }
    public string thisTag { get; set; }

    public bool isBuff = false;

    [Server]
    public virtual void Initialize(float atk, float deadZone, Vector3 direction, string enemyTag, string thisTag)
    {
        Atk = atk;
        this.deadZone = deadZone;
        Direction = direction;
        this.enemyTag = enemyTag;
        this.thisTag = thisTag;
        gameObject.tag = this.thisTag;
        Debug.Log("This bullet Atk = " + Atk);
    }

    public abstract void Move();

    //private void Start()
    //{
    //    gameObject.tag = this.thisTag;
    //    Debug.Log("This bullet Atk = " + Atk);
    //}

    private void Update()
    {
        Move();
        CheckDeadZone();
    }

    protected void CheckDeadZone()
    {
        if (Direction == Vector3.left && transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
        else if (Direction == Vector3.right && transform.position.x > deadZone)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (IsServer)
        {
            HandleCollision(collision);
        }
    }

    [Server]
    protected virtual void HandleCollision(Collider collision)
    {
        return;
    }
}
