using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BoardManager m_Board;
    private Vector2Int m_CellPosition;
    private bool m_isGameOver = false;
    private bool m_isMoving;
    private Vector3 m_MoveTarget;
    public float MoveSpeed = 5.0f;
    private Animator m_Animator;
    private bool m_isAttacked;


    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        m_CellPosition = cell;

        transform.position = m_Board.CellToWorld(cell);
    }

    public void MoveTo(Vector2Int cell, bool immediate)
    {
        m_CellPosition = cell;
        if (immediate)
        {
            m_isMoving = false;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }
        else
        {
            m_isMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }
        m_Animator.SetBool("Moving", m_isMoving);
    }

    public void GameOver()
    {
        m_isGameOver = true;
    }

    public void Init()  
    {
        m_isMoving = false;
        m_isGameOver = false;
    }

    public void Attack()
    {
        Attacking();
        m_Animator.SetTrigger("Attack");
    }

    public bool Attacking()
    {
        return true;
    }
    private void Update()
    {
        if (m_isGameOver) 
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }
        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;

        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }

        if (m_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);
            if (transform.position == m_MoveTarget)
            {
                m_isMoving = false;
                m_Animator.SetBool("Moving", false);
                var cellData = m_Board.GetCellData(m_CellPosition);
                if (cellData.ContainedObject != null)
                {
                    cellData.ContainedObject.PlayerEntered();
                }
            }
            return;
        }

        if (hasMoved)
        {
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);
            if (cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();
                
                if(cellData.ContainedObject == null)
                {
                    MoveTo(newCellTarget, false);
                }
                else if (cellData.ContainedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget, false);
                }
            }
        }
    }
}
