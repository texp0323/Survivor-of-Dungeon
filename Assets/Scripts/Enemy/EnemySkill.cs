using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    private EnemyAI enemyAI;
    private SkillArchive skillArchive;

    public int[] enemySkillsID;
    public Skill[] enemySkills;

    private Vector2 realPos;
    private int lastDir;

    [SerializeField] private Transform rangeBox;
    private float interval;

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();

        skillArchive = GameObject.FindWithTag("SkillArchive").GetComponent<SkillArchive>();
        for (int i = 0; i < 4; i++)
        {
            if (enemySkillsID[i] != 0)
                enemySkills[i] = skillArchive.skills[enemySkillsID[i]];
        }
        ResetSkill();

        rangeBox.localScale = enemySkills[0].range;
        interval = enemySkills[0].interval;
    }

    public void SkillAiming(int dir , Vector2 getPos)
    {
        realPos = getPos;
        lastDir = dir;

        if (dir == 1)
        {
            rangeBox.rotation = Quaternion.Euler(0f, 0f, 0f);
            rangeBox.position = new Vector3(realPos.x + interval, realPos.y, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (dir == 2)
        {
            rangeBox.rotation = Quaternion.Euler(0f, 0f, 180f);
            rangeBox.position = new Vector3(realPos.x - interval, realPos.y, 0);
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (dir == 3)
        {
            rangeBox.rotation = Quaternion.Euler(0f, 0f, 90f);
            rangeBox.position = new Vector3(realPos.x, realPos.y + interval, 0);
        }
        if (dir == 4)
        {
            rangeBox.rotation = Quaternion.Euler(0f, 0f, -90f);
            rangeBox.position = new Vector3(realPos.x, realPos.y - interval, 0);
        }
    }

    private void ResetSkill()
    {
        SkillAiming(1, realPos);
    }
}
