using UnityEngine;

public class RightBullet : Bullet
{
    public override void Move()
    {
        transform.position += Vector3.left * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        Soldier soldier = collision.gameObject.GetComponent<Soldier>();
        if (collision.gameObject.CompareTag("LeftTeam") && soldier != null && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk);
            Destroy(gameObject);
        }
    }
}
