using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    private BattleManager battleManager;

    public Stat stat;
    public int actCount;
    public bool actAble;

    private void Start()
    {
        battleManager = GameObject.FindWithTag("BattleManager").GetComponent<BattleManager>();
        TurnStart();
    }

    public void Act()
    {
        actCount--;
        if(actCount <= 0)
        {
            actAble = false;
            battleManager.TurnEnd("Player");
        }
    }

    public void TurnStart()
    {
        actAble = true;
        actCount = stat.speed;
    }
}
