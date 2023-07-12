using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerInfo playerInfo;
    private SkillArchive skillArchive;

    public int[] playerSkillsID;
    public Skill[] playerSkills;

    private int usingSkillNum;

    private Vector2 realPos;
    private Vector2 rangeScale;
    private Vector2 rangePos;
    private int lastDir;
    [SerializeField] private Transform rangeBox;
    [SerializeField] private LayerMask enemyLayer;
    private bool skillMode;
    private float interval;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerInfo = GetComponent<PlayerInfo>();

        skillArchive = GameObject.FindWithTag("SkillArchive").GetComponent<SkillArchive>();
        for (int i = 0; i < 4; i++)
        {
            if(playerSkillsID[i] != 0)
                playerSkills[i] = skillArchive.skills[playerSkillsID[i]];
        }
        ResetSkill();
    }
    void Update()
    {
        if(playerInfo.actAble)
        {
            LoadSkill();
            if (Input.GetKeyDown(KeyCode.RightArrow)) { SkillAiming(1); }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { SkillAiming(2); }
            if (Input.GetKeyDown(KeyCode.UpArrow)) { SkillAiming(3); }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { SkillAiming(4); }
        }
    }

    private void LoadSkill()
    {
        if(Input.GetKeyDown(KeyCode.Z)) // 1번 스킬
        {
            rangeScale = playerSkills[0].range;
            rangeBox.localScale = rangeScale;
            interval = playerSkills[0].interval;
            SkillModeChange(1);
            SkillAiming(lastDir);
        }
        if (Input.GetKeyDown(KeyCode.X)) // 2번 스킬
        {
            rangeScale = playerSkills[1].range;
            rangeBox.localScale = rangeScale;
            interval = playerSkills[1].interval;
            SkillModeChange(2);
            SkillAiming(lastDir);
        }
        if (Input.GetKeyDown(KeyCode.C)) // 3번 스킬
        {
            rangeScale = playerSkills[2].range;
            rangeBox.localScale = rangeScale;
            interval = playerSkills[2].interval;
            SkillModeChange(3);
            SkillAiming(lastDir);
        }
        if (Input.GetKeyDown(KeyCode.V)) // 4번 스킬
        {
            rangeScale = playerSkills[3].range;
            rangeBox.localScale = rangeScale;
            interval = playerSkills[3].interval;
            SkillModeChange(4);
            SkillAiming(lastDir);
        }

        if (Input.GetKeyDown(KeyCode.Q)) // 스킬 사용 취소
        {
            SkillModeChange(0);
            usingSkillNum = 0;
        }
    }

    private void SkillModeChange(int skillNum)
    {
        if (skillNum == 0)
        {
            skillMode = false;
        }
        else
        {
            if(!skillMode)
            {
                skillMode = true;
            }

            if (skillNum == usingSkillNum)
            {
                UseSkill();
            }
            else
            {
                usingSkillNum = skillNum;
            }
        }
        playerMovement.moveAble = !skillMode;
        rangeBox.gameObject.SetActive(skillMode);
    }

    private void UseSkill()
    {
        Debug.Log(playerSkills[usingSkillNum - 1].skillName + " 스킬사용 - " + usingSkillNum + "번째 스킬");
        if (playerSkills[usingSkillNum - 1].effect) { Instantiate(playerSkills[usingSkillNum - 1].effect, rangeBox.position, Quaternion.identity); }

        Collider2D[] hitEnemy;
        if (lastDir <= 2) {hitEnemy = Physics2D.OverlapBoxAll(rangePos, rangeScale, 0, enemyLayer); }
        else { hitEnemy = Physics2D.OverlapBoxAll(rangePos, new Vector2(rangeScale.y, rangeScale.x), 0, enemyLayer); }



        playerInfo.Act();
        SkillModeChange(0);
        usingSkillNum = 0;
    }

    public void SkillAiming(int dir)
    {
        realPos = playerMovement.movePos;
        lastDir = dir;

        if(dir == 1)
        {
            rangeBox.rotation = Quaternion.Euler(0f, 0f, 0f);
            rangePos = new Vector3(realPos.x + interval, realPos.y, 0);
            rangeBox.position = rangePos;
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (dir == 2)
        {
            rangeBox.rotation = Quaternion.Euler(0f, 0f, 180f);
            rangePos = new Vector3(realPos.x - interval, realPos.y, 0);
            rangeBox.position = rangePos;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (dir == 3)
        {
            rangeBox.rotation = Quaternion.Euler(0f, 0f, 90f);
            rangePos = new Vector3(realPos.x, realPos.y + interval, 0);
            rangeBox.position = rangePos;
        }
        if (dir == 4)
        {
            rangeBox.rotation = Quaternion.Euler(0f, 0f, -90f);
            rangePos = new Vector3(realPos.x, realPos.y - interval, 0);
            rangeBox.position = rangePos;
        }
    }

    private void ResetSkill()
    {
        SkillAiming(1);
    }
}
