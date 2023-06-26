using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInfo playerInfo;

    [SerializeField] private LayerMask wallLayer;

    public Vector2 movePos;
    public bool moveAble;

    private void Start()
    {
        movePos = transform.position;
        moveAble = true;
        playerInfo = GetComponent<PlayerInfo>();
    }
    void Update()
    {
        if (playerInfo.actAble && moveAble) { Move(); }
    }

    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if(!Physics2D.CircleCast(new Vector2(movePos.x + 1, movePos.y), 0.4f, Vector2.zero,0, wallLayer))
            {
                movePos.x = movePos.x + 1;
                transform.DOMove(movePos, 0.2f);
                transform.localScale = new Vector3(1, 1, 1);
                playerInfo.Act();
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!Physics2D.CircleCast(new Vector2(movePos.x - 1, movePos.y), 0.4f, Vector2.zero, 0, wallLayer))
            {
                movePos.x = movePos.x - 1;
                transform.DOMove(movePos, 0.2f);
                transform.localScale = new Vector3(-1, 1, 1);
                playerInfo.Act();
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!Physics2D.CircleCast(new Vector2(movePos.x, movePos.y + 1), 0.4f, Vector2.zero, 0, wallLayer))
            {
                movePos.y = movePos.y + 1;
                transform.DOMove(movePos, 0.2f);
                playerInfo.Act();
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (!Physics2D.CircleCast(new Vector2(movePos.x, movePos.y - 1), 0.4f, Vector2.zero, 0, wallLayer))
            {
                movePos.y = movePos.y - 1;
                transform.DOMove(movePos, 0.2f);
                playerInfo.Act();
            }
        }
    }
}
