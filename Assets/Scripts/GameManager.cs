using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public BoardManager BoardManager;
    public PlayerController PlayerController;

    public TurnManager TurnManager { get; private set; }

    public UIDocument UIDoc;
    private Label m_FoodLabel;
    private int m_currentLevel;

    private int m_FoodAmount = 10;
    private VisualElement m_GameOverPanel;
    private Label m_GameOverMessage;

    void OnTurnHappen()
    {
        ChangeFood(-1);
        
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartNewGame()
    {
        m_GameOverPanel.style.visibility = Visibility.Hidden;

        BoardManager.Clean();
        BoardManager.Init();

        m_currentLevel = 1;
        m_FoodAmount = 25;
        m_FoodLabel.text = "Food : " + m_FoodAmount;

        PlayerController.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
    }

    void Start()
    {
        TurnManager = new TurnManager();
        TurnManager.OnTick += OnTurnHappen;

        m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");

        m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
        m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

        StartNewGame();
    }

    public void ChangeFood(int amount)
    {
        m_FoodAmount += amount;
        m_FoodLabel.text = "Food : " + m_FoodAmount;
        if (m_FoodAmount <= 0)
        {
            PlayerController.GameOver();
            m_GameOverPanel.style.visibility = Visibility.Visible;
            if (m_currentLevel == 1)
            {
                m_GameOverMessage.text = "Game Over!\n\n You have run through " + m_currentLevel + " level!";    
            }
            else 
            {
                m_GameOverMessage.text = "Game Over!\n\n You have run through " + m_currentLevel + " levels!";
            }
        }
    }

    public void NewLevel()
    {
        BoardManager.Clean();
        BoardManager.Init();
        PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        m_currentLevel++;
    }
}
