using Fusion;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    [Header("移動速度"), Range(0, 100)]
    public float speed = 5;
    [Header("存活時間"), Range(0, 10)]
    public float lifeTime = 5;

    //Networked連線用屬性資料
    [Networked]
    private TickTimer life { get; set; }

    public void Init()
    {
        life = TickTimer.CreateFromSeconds(Runner, lifeTime);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner)) Runner.Despawn(Object);
        else transform.Translate(0, 0, speed * Runner.DeltaTime);
    }
}
