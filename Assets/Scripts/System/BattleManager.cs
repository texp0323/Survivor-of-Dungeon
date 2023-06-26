using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    private Transform player;
    private PlayerInfo playerInfo;

    [SerializeField] private Vector2Int mapSize;

    private List<GameObject> enemys;
    [SerializeField] List<GameObject> enemyList;

    private string turn;
    private int round;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerInfo = player.GetComponent<PlayerInfo>();
        BattleSet();
    }

    private void BattleSet()
    {
        playerInfo.TurnStart();
        turn = "Player";
        EnemyListSet();
    }

    public void EnemyListSet()
    {
        enemys = new List<GameObject>();
        enemyList = new List<GameObject>();

        enemys = GameObject.FindGameObjectsWithTag("Enemy").ToList();

        while (enemys.Count > 0)
        {
            GameObject selectEnemy = enemys[0];
            foreach (GameObject enemy in enemys)
            {
                enemy.GetComponent<EnemyAI>().topRight = mapSize;

                if (enemy.GetComponent<EnemyInfo>().stat.speed > selectEnemy.GetComponent<EnemyInfo>().stat.speed)
                {
                    selectEnemy = enemy;
                }
            }
            enemys.Remove(selectEnemy);
            enemyList.Add(selectEnemy);
        }
    }

    public void TurnEnd(string type)
    {
        if (type == "Player")
        {
            Debug.Log("플레이어 턴 종료");
            StartCoroutine(EnemysAct());
            turn = "Enemy";
        }
        else
        {
            Debug.Log("적 턴 종료");
            playerInfo.TurnStart();
            turn = "Player";
        }
    }

    IEnumerator EnemysAct()
    {
        int maxActCount = 0;
        foreach (GameObject enemy in enemyList)
        {
            EnemyInfo enemyInfo = enemy.GetComponent<EnemyInfo>();
            if (enemyInfo.TurnStart() > maxActCount)
            {
                maxActCount = enemyInfo.actCount;
            }
        }

        while (maxActCount > 0)
        {
            foreach (GameObject enemy in enemyList)
            {
                EnemyInfo enemyInfo = enemy.GetComponent<EnemyInfo>();
                if (enemyInfo.actCount >= maxActCount)
                {
                    if(enemyInfo.Act())
                    {
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
            maxActCount--;
        }

        TurnEnd("Enemy");
    }

}
