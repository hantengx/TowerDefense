using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TargetPoint : MonoBehaviour
{
    public Enemy Enemy { get; private set; }

    public Vector3 Position => transform.position;

    private void Awake()
    {
        Enemy = transform.root.GetComponent<Enemy>();
        Debug.Assert(Enemy != null, "Target point without Enemy root!", this);
        Debug.Assert(gameObject.layer == 9, "Target point no wrong layer!", this);
    }
}
