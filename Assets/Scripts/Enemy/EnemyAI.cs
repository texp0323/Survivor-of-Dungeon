using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    public Node(bool _isWall, int _x, int _y) { isWall = _isWall; x = _x; y = _y; }

    public bool isWall;
    public Node ParentNode;

    // G : 시작으로부터 이동했던 거리, H : |가로|+|세로| 장애물 무시하여 목표까지의 거리, F : G + H
    public int x, y, G, H;
    public int F { get { return G + H; } }
}


public class EnemyAI : MonoBehaviour
{
    private EnemySkill enemySkill;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask enemyLayer;

    public Vector2Int topRight;
    private Vector2Int startPos, targetPos;
    private List<Node> FinalNodeList;

    private int sizeX, sizeY;
    private Node[,] NodeArray;
    private Node StartNode, TargetNode, CurNode;
    private List<Node> OpenList, ClosedList;

    private PlayerMovement player;
    private int moveCount;
    private int traceMode;
    private Vector2 realPos;

    private void Start()
    {
        traceMode = 0;
        enemySkill = GetComponent<EnemySkill>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    public int Move()
    {
        TraceModeSet();
        moveCount++;
        if (moveCount + 1 < FinalNodeList.Count)
        {
            Vector2 nextMovePos = new Vector2(FinalNodeList[moveCount].x, FinalNodeList[moveCount].y);
            if (!Physics2D.CircleCast(nextMovePos, 0.4f, Vector2.zero, 0, enemyLayer))
            {
                if (nextMovePos.x < transform.position.x) transform.localScale = new Vector3(1, 1, 1);
                if (nextMovePos.x > transform.position.x) transform.localScale = new Vector3(-1, 1, 1);
                transform.DOMove(nextMovePos, 0.2f);
                skillAim(nextMovePos);
                return 0;
            }
            else
            {
                moveCount--;
                skillAim(transform.position);
                return 1;
            }
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
        if (player.movePos.x == realPos.x)
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

    private void TraceModeSet()
    {
        if(Mathf.Abs(realPos.x - player.movePos.x) > Mathf.Abs(realPos.y - player.movePos.y)) traceMode = 1;
        else traceMode = 0;
    }

    public void PathFinding()
    {
        moveCount = 0;
        startPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        targetPos = new Vector2Int((int)player.movePos.x, (int)player.movePos.y);

        // NodeArray의 크기 정해주고, isWall, x, y 대입
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


        // 시작과 끝 노드, 열린리스트와 닫힌리스트, 마지막리스트 초기화
        StartNode = NodeArray[startPos.x, startPos.y];
        TargetNode = NodeArray[targetPos.x, targetPos.y];

        OpenList = new List<Node>() { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();


        while (OpenList.Count > 0)
        {
            // 열린리스트 중 가장 F가 작고 F가 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌리스트로 옮기기
            CurNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
                if (OpenList[i].F < CurNode.F || OpenList[i].F == CurNode.F && OpenList[i].H < CurNode.H) CurNode = OpenList[i];

            OpenList.Remove(CurNode);
            ClosedList.Add(CurNode);


            // 마지막
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


            if(traceMode == 0)
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
            // ↑ → ↓ ←
        }
    }

    private void OpenListAdd(int checkX, int checkY)
    {
        // 상하좌우 범위를 벗어나지 않고, 벽이 아니면서, 닫힌리스트에 없다면
        if (checkX >= 0 && checkX < topRight.x + 1 && checkY >= 0 && checkY < topRight.y + 1 && !NodeArray[checkX, checkY].isWall && !ClosedList.Contains(NodeArray[checkX, checkY]))
        {
            // 이웃노드에 넣고, 직선은 1
            Node NeighborNode = NodeArray[checkX, checkY];
            int MoveCost = CurNode.G + 1;


            // 이동비용이 이웃노드G보다 작거나 또는 열린리스트에 이웃노드가 없다면 G, H, ParentNode를 설정 후 열린리스트에 추가
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