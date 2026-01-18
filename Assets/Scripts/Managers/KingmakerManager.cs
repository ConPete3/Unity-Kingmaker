using UnityEngine;

/// <summary>
/// Main manager for the Kingmaker game. Initializes the game and creates the hex grid
/// with 7 territories (1 capital + 6 surrounding territories) at scene start.
/// </summary>
public class KingmakerManager : MonoBehaviour
{
    public static KingmakerManager Instance { get; private set; }

    [Header("Core Systems")]
    [SerializeField] private HexGridManager hexGridManager;
    [SerializeField] private PartyController partyController;

    [Header("Configuration")]
    [SerializeField] private HexGridConfig hexGridConfig;
    [Tooltip("Number of territory rings around the capital (1 = 7 total territories)")]
    [SerializeField, Range(1, 10)] private int territoryRings = 1;

    public HexGridManager HexGrid => hexGridManager;
    public PartyController Party => partyController;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Debug.Log("KingmakerManager: Awake - Initializing game systems...");
    }

    private void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// Initializes the game by generating the hex grid with territories
    /// and positioning the party at the capital.
    /// </summary>
    private void InitializeGame()
    {
        Debug.Log($"KingmakerManager: Initializing game with {CalculateTotalTerritories()} territories...");

        // Generate the hex grid
        if (hexGridManager != null)
        {
            hexGridManager.GenerateGrid();
            Debug.Log("KingmakerManager: Hex grid generated successfully.");
        }
        else
        {
            Debug.LogWarning("KingmakerManager: HexGridManager reference is missing!");
        }

        // Position party at the capital (center)
        if (partyController != null)
        {
            partyController.SetPosition(HexCoordinates.Zero);
            Debug.Log("KingmakerManager: Party positioned at capital.");
        }

        Debug.Log("KingmakerManager: Game initialized successfully!");
    }

    /// <summary>
    /// Calculates the total number of territories based on ring count.
    /// Ring 0 = 1 (capital), Ring 1 = 6, Ring 2 = 12, etc.
    /// Formula: 1 + 3*n*(n+1) where n = number of rings
    /// </summary>
    private int CalculateTotalTerritories()
    {
        int total = 1; // Capital
        for (int ring = 1; ring <= territoryRings; ring++)
        {
            total += 6 * ring;
        }
        return total;
    }

    /// <summary>
    /// Resets the game state.
    /// </summary>
    public void ResetGame()
    {
        if (hexGridManager != null)
            hexGridManager.GenerateGrid();

        if (partyController != null)
            partyController.SetPosition(HexCoordinates.Zero);

        Debug.Log("KingmakerManager: Game reset.");
    }
}
