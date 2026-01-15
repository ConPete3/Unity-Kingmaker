using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileViewScreen : ScreenBase
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI tileNameText;
    [SerializeField] private TextMeshProUGUI tileDescriptionText;
    [SerializeField] private TextMeshProUGUI placeholderText;
    [SerializeField] private Button backButton;

    private HexTile currentTile;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    protected override void OnShow(object context)
    {
        currentTile = context as HexTile;

        if (currentTile != null && currentTile.TileData != null)
        {
            if (tileNameText != null)
                tileNameText.text = currentTile.TileData.TileName;

            if (tileDescriptionText != null)
                tileDescriptionText.text = currentTile.TileData.Description;

            if (placeholderText != null)
                placeholderText.text = $"Tile View: {currentTile.TileData.TileName}\n\nCoordinates: {currentTile.Coordinates}\n\n(Map content coming soon)";
        }
    }

    protected override void OnHide()
    {
        currentTile = null;
    }

    private void OnBackClicked()
    {
        ScreenManager.Instance.ShowScreen(GameScreen.GlobalMap);
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }
}
