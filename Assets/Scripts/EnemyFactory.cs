﻿using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
    [SerializeField]
    private Enemy prefab = default;
    
    [SerializeField, FloatRangeSlider(0.5f, 2f)]
    private FloatRange scale = new FloatRange(1f);

    [SerializeField, FloatRangeSlider(-0.4f, 0.4f)]
    private FloatRange pathOffset = new FloatRange(0f);

    [SerializeField, FloatRangeSlider(0.2f, 5f)]
    private FloatRange speed = new FloatRange(1f);

    public Enemy Get()
    {
        var instance = CreateGameObjectInstance(prefab);
        instance.OriginFactory = this;
        instance.Initialize(scale.RandomValueRange, speed.RandomValueRange ,pathOffset.RandomValueRange);
        return instance;
    }

    public void Reclaim(Enemy enemy)
    {
        Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
        Destroy(enemy.gameObject);
    }
}
