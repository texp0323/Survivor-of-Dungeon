using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : �������κ��� �̵��ߴ� �Ÿ�, H : |����|+|����| ��ֹ� �����Ͽ� ��ǥ������ �Ÿ�, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}


public class EnemyAI : MonoBehaviour
{
    private EnemySkill enemySkill;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private int traceMode;

    public Vector2Int topRight;
    private Vector2Int startPos, targetPos;
    private List<Node> FinalNodeList;

    private int sizeX, sizeY;
    private Node[,] NodeArray;
    private Node StartNode, TargetNode, CurNode;
    private List<Node> OpenList, ClosedList;

    private PlayerMovement player;
    private int moveCount;
    private int traceDir;
    private Vector2 realPos;

    private void Start()
    {
        traceDir = 0;
        enemySkill = GetComponent<EnemySkill>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    public int Move()
    {
        PathFinding();
        TraceDirSet();
        moveCount++;
        if (moveCount + 1 < FinalNodeList.Count)
        {
            Vector2 nextMovePos = new Vector2(FinalNodeList[moveCount].x, FinalNodeList[moveCount].y);
            if (nextMovePos.x < transform.position.x) transform.localScale = new Vector3(1, 1, 1);
            if (nextMovePos.x > transform.position.x) transform.localScale = new Vector3(-1, 1, 1);
            transform.DOMove(nextMovePos, 0.2f);
            skillAim(nextMovePos);
            return 0;
        }
        else
        {
            skillAim(transform.position);
            return 2;
        }
    }

    private void skillAim(Vector2 getPos)
    {
        realPos = getPos;

        if (Mathf.Abs(realPos.x - player.movePos.x) < Mathf.Abs(realPos.y - player.movePos.y))
        {
            if (player.movePos.y > realPos.y)
            {
                enemySkill.SkillAiming(3, realPos);
            }
            else enemySkill.SkillAiming(4, realPos);
        }
        else
        {
            if (player.movePos.x > realPos.x)
            {
                enemySkill.SkillAiming(1, realPos);
            }
            else enemySkill.SkillAiming(2, realPos);
        }
    }

    private void TraceDirSet()
    {
        if(traceMode == 0)
        {
            if (Mathf.Abs(realPos.x - player.movePos.x) > Mathf.Abs(realPos.y - player.movePos.y))
                traceDir = 1;
            else traceDir = 0;
        }
        else
        {
            if (Mathf.Abs(realPos.x - player.movePos.x) > Mathf.Abs(realPos.y - player.movePos.y))
                traceDir = 0;
            else traceDir = 1;
        }
    }

    public void PathFinding()
    {
        moveCount = 0;
        startPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        targetPos = new Vector2Int((int)player.movePos.x, (int)player.movePos.y);

        // NodeArray�� ũ�� �����ְ�, isWall, x, y ����
        sizeX = topRight.x + 1;
        sizeY = topRight.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                if (Physics2D.CircleCast(new Vector2(i, j),0.4f,Vector2.zero,0, wallLayer)) isWall = true;

                NodeArray[i, j] = new Node(isWall, i, j);
            }
        }


        // ���۰� �� ���, ��������Ʈ�� ��������Ʈ, ����������Ʈ �ʱ�ȭ
        StartNode = NodeArray[startPos.x, startPos.y];
        TargetNode = NodeArray[targetPos.x, targetPos.y];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();


        while (OpenList.Count > 0)
        {
            // ��������Ʈ �� ���� F�� �۰� F�� ���ٸ� H�� ���� �� ������� �ϰ� ��������Ʈ���� ��������Ʈ�� �ű��
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F < CurNode.F || OpenList[i].F == CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // ������
            if (CurNode == TargetNode)
            {
                Node TargetCurNode = TargetNode;
                while (TargetCurNode != StartNode)
                {
                    FinalNodeList.Add(TargetCurNode);
                    TargetCurNode = TargetCurNode.ParentNode;
                }
                FinalNodeList.Add(StartNode);
                FinalNodeList.Reverse();
                return;
            }


            if(traceDir == 0)
            {
                OpenListAdd(CurNode.x, CurNode.y + 1);
                OpenListAdd(CurNode.x, CurNode.y - 1);
                OpenListAdd(CurNode.x + 1, CurNode.y);
                OpenListAdd(CurNode.x - 1, CurNode.y);
            }
            else
            {
                OpenListAdd(CurNode.x + 1, CurNode.y);
                OpenListAdd(CurNode.x - 1, CurNode.y);
                OpenListAdd(CurNode.x, CurNode.y + 1);
                OpenListAdd(CurNode.x, CurNode.y - 1);
            }
            // �� �� �� ��
        }
    }

    private void OpenListAdd(int checkX, int checkY)
    {
        // �����¿� ������ ����� �ʰ�, ���� �ƴϸ鼭, ��������Ʈ�� ���ٸ�
        if (checkX >= 0 && checkX < topRight.x + 1 && checkY >= 0 && checkY < topRight.y + 1 && !NodeArray[checkX, checkY].isWall && !ClosedList.Contains(NodeArray[checkX, checkY]))
        {
            // �̿���忡 �ְ�, ������ 1
            Node NeighborNode = NodeArray[checkX, checkY];
            int MoveCost = CurNode.G + 1;


            // �̵������ �̿����G���� �۰ų� �Ǵ� ��������Ʈ�� �̿���尡 ���ٸ� G, H, ParentNode�� ���� �� ��������Ʈ�� �߰�
            if (MoveCost < NeighborNode.G || !OpenList.Contains(NeighborNode))
            {
                NeighborNode.G = MoveCost;
                NeighborNode.H = (Mathf.Abs(NeighborNode.x - TargetNode.x) + Mathf.Abs(NeighborNode.y - TargetNode.y));
                NeighborNode.ParentNode = CurNode;

                OpenList.Add(NeighborNode);
            }
        }
    }
}