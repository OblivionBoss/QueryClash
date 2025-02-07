using UnityEngine;

public class SingleRightBullet : SingleBullet
{
    public override void Move()
    {
        transform.position += Vector3.left * bulletspeed * Time.deltaTime;
    }

    protected override void HandleCollision(Collider collision)
    {
        SingleSoldier soldier = collision.gameObject.GetComponent<SingleSoldier>();
        if (collision.gameObject.CompareTag("LeftTeam") && soldier != null && soldier.isPlaced)
        {
            soldier.ReduceHp(Atk);
            Destroy(gameObject);
        }
    }
}
