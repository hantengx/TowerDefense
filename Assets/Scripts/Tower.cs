using UnityEngine;

public class Tower : GameTileContent
{
    [SerializeField, Range(1.5f, 10.5f)]
    private float targetingRange = 1.5f;

    private TargetPoint target;
    private const int enemyLayerMask = 1 << 9;
    private Collider[] targetsBuffer = new Collider[1];
    
    public override void GameUpdate()
    {
        if (TrackTarget() || AcquireTarget())
        {
            Debug.Log("Locked target!");
        }
    }

    private bool AcquireTarget()
    {
        Vector3 a = transform.localPosition;
        var b = a;
        b.y += 2f;
        int hits = Physics.OverlapCapsuleNonAlloc (a, b, targetingRange, this.targetsBuffer, enemyLayerMask);
        if (hits > 0)
        {
            target = targetsBuffer[0].GetComponent<TargetPoint>();
            Debug.Assert(target != null, "Targeted non-enemy!", targetsBuffer[0]);
            return true;
        }

        target = null;
        return false;
    }

    private bool TrackTarget()
    {
        if (target == null)
        {
            return false;
        }

        //o.125f collider radius
        Vector3 a = transform.localPosition;
        Vector3 b = target.Position;
        float deltaX = a.x - b.x;
        float deltaZ = a.z - b.z;
        float r = targetingRange + 0.125f * target.Enemy.Scale;
        if (deltaX * deltaX + deltaZ * deltaZ > r * r)
        {
            target = null;
            return false;
        }

        return true;
    }
    
    void OnDrawGizmosSelected () {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.localPosition;
        position.y += 0.01f;
        Gizmos.DrawWireSphere(position, targetingRange);

        if (target != null)
        {
            Gizmos.DrawLine(position, target.Position);
        }
    }
}
