using UnityEngine;
using UnityEngine.UI;

public class GlobalMapScreen : ScreenBase
{
    [Header("References")]
    [SerializeField] private HexGridManager hexGridManager;
    [SerializeField] private PartyController partyController;
    [SerializeField] private Button enterCapitalButton;

    [Header("Highlight Colors")]
    [SerializeField] private Color moveableHighlight = new Color(0.5f, 1f, 0.5f, 0.5f);

    protected override void OnShow(object context)
    {
        if (hexGridManager != null)
            hexGridManager.OnTileClicked.AddListener(HandleTileClicked);

        if (partyController != null)
            partyController.OnPartyMoved.AddListener(UpdateUI);

        if (enterCapitalButton != null)
            enterCapitalButton.onClick.AddListener(OnEnterCapitalClicked);

        UpdateUI();
    }

    protected override void OnHide()
    {
        if (hexGridManager != null)
        {
            hexGridManager.OnTileClicked.RemoveListener(HandleTileClicked);
            hexGridManager.ClearAllHighlights();
        }

        if (partyController != null)
            partyController.OnPartyMoved.RemoveListener(UpdateUI);

        if (enterCapitalButton != null)
            enterCapitalButton.onClick.RemoveListener(OnEnterCapitalClicked);
    }

    private void HandleTileClicked(HexTile tile)
    {
        if (partyController == null) return;

        if (partyController.CanMoveTo(tile.Coordinates))
        {
            partyController.MoveTo(tile.Coordinates);
        }
        else if (!tile.TileData.IsCapital && partyController.IsPartyOnTile(tile.Coordinates))
        {
            ScreenManager.Instance.ShowScreen(GameScreen.TileView, tile);
        }
    }

    private void OnEnterCapitalClicked()
    {
        if (hexGridManager == null) return;

        HexTile capitalTile = hexGridManager.GetTile(HexCoordinates.Zero);
        ScreenManager.Instance.ShowScreen(GameScreen.CapitalView, capitalTile);
    }

    private void UpdateUI()
    {
        if (partyController == null || hexGridManager == null) return;

        bool isOnCapital = partyController.CurrentPosition == HexCoordinates.Zero;
        if (enterCapitalButton != null)
            enterCapitalButton.gameObject.SetActive(isOnCapital);

        hexGridManager.ClearAllHighlights();
        var moveableTiles = partyController.GetMoveableTiles();
        foreach (var tile in moveableTiles)
        {
            tile.SetHighlight(true, moveableHighlight);
        }
    }
}
