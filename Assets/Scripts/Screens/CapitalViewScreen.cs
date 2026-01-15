using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CapitalViewScreen : ScreenBase
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI capitalNameText;
    [SerializeField] private TextMeshProUGUI placeholderText;
    [SerializeField] private Button backButton;

    private void Awake()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    protected override void OnShow(object context)
    {
        if (capitalNameText != null)
            capitalNameText.text = "Your Capital";

        if (placeholderText != null)
            placeholderText.text = "Capital Management\n\n(Building and management features coming soon)";
    }

    protected override void OnHide()
    {
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
