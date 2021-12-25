using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyCollection
{
    private List<Enemy> enemies = new List<Enemy>();

    public void Add(Enemy enemy)
    {
        enemies.Add(enemy);
    }

    public void GameUpdate()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (!enemies[i].GameUpdate())
            {
                var lastIndex = enemies.Count - 1;
                enemies[i] = enemies[lastIndex];
                enemies.RemoveAt(lastIndex);
            }
        }
    }
}
