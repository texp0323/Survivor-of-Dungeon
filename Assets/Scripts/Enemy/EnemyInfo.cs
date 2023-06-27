using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    private EnemyAI enemyAI;

    public Stat stat;
    [HideInInspector] public int actCount;

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
    }

    public bool Act()
    {
        if(enemyAI.Move() == 0)
        {
            actCount--;
            return true;
        }
        else
        {
            actCount = 0;
            return false;
        }
    }

    public int TurnStart()
    {
        actCount = stat.speed;
        return actCount;
    }
}
