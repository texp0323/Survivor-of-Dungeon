using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skill
{
    public string skillName;
    public string skillType;
    public bool targetingSkill;
    public string valueType;
    public float valueMultiply;
    public string lore;
    public Vector2 range;
    public float interval;
    public GameObject effect;
}

public class SkillArchive : MonoBehaviour
{
    public Skill[] skills;
}
