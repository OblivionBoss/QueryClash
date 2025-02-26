using UnityEngine;

public abstract class SingleBullet : MonoBehaviour
{
    public float bulletspeed = 1;
    public float deadZone {  get;  set; }
    public float Atk { get; set; }
    public Vector3 Direction { get; set; }
    public string enemyTag { get; set; }
    public string thisTag { get; set; }

    public bool isBuff = false;
   
    public virtual void Initialize(float atk, float deadZone, Vector3 direction, string enemyTag, string thisTag)
    {
        Atk = atk;
        this.deadZone = deadZone;
        Direction = direction;
        this.enemyTag = enemyTag;
        this.thisTag = thisTag;
    }

    public abstract void Move();
    private void Start()
    {
        gameObject.tag = this.thisTag;
        Debug.Log("This bullet Atk = " + Atk);
    }

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
        HandleCollision(collision);
    }

    protected abstract void HandleCollision(Collider collision);
}
