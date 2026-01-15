using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Core Systems")]
    [SerializeField] private HexGridManager hexGridManager;
    [SerializeField] private PartyController partyController;
    [SerializeField] private ScreenManager screenManager;

    [Header("Configuration")]
    [SerializeField] private bool generateGridOnStart = true;

    public HexGridManager HexGrid => hexGridManager;
    public PartyController Party => partyController;
    public ScreenManager Screens => screenManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (generateGridOnStart && hexGridManager != null)
        {
            hexGridManager.GenerateGrid();
        }

        if (partyController != null)
        {
            partyController.SetPosition(HexCoordinates.Zero);
        }

        Debug.Log("Kingmaker initialized successfully!");
    }

    public void ResetGame()
    {
        if (hexGridManager != null)
            hexGridManager.GenerateGrid();

        if (partyController != null)
            partyController.SetPosition(HexCoordinates.Zero);

        if (screenManager != null)
            screenManager.ShowScreen(GameScreen.GlobalMap);
    }
}
