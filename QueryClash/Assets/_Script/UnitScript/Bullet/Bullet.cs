using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public float bulletspeed = 1;
    public float deadZone {  get;  set; }
    public float Atk { get; set; }
    public Vector3 Direction { get; set; }
    public virtual void Initialize(float atk,float deadZone, Vector3 direction)
    {
        Atk = atk;
        this.deadZone = deadZone;
        Direction = direction;

    }

    public abstract void Move();
    private void Start()
    {
        transform.position = new Vector3(
            transform.position.x + 0.5f,
            transform.position.y + 0.5f,
            transform.position.z + 0.5f
        );
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
