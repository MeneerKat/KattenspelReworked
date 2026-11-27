using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ModifierAdjustment : MonoBehaviour
{
    // Gameobject variables
    [SerializeField] GameObject bouncyButton;
    [SerializeField] GameObject evilButton;
    [SerializeField] GameObject coolButton;
    [SerializeField] GameObject mirrorButton;
    [SerializeField] GameObject surroundingButton;
    [SerializeField] GameObject highscoreText;

    // Functions
    // Set all score boost text to their corresponding values
    void Start()
    {
        bouncyButton.transform.Find("ScoreBoostValue").GetComponent<TextMeshProUGUI>().text = GameManager.bounceMultiplier.ToString() + "x";
        evilButton.transform.Find("ScoreBoostValue").GetComponent<TextMeshProUGUI>().text = GameManager.evilMultiplier.ToString() + "x";
        coolButton.transform.Find("ScoreBoostValue").GetComponent<TextMeshProUGUI>().text = GameManager.coolMultiplier.ToString() + "x";
        mirrorButton.transform.Find("ScoreBoostValue").GetComponent<TextMeshProUGUI>().text = GameManager.invisibleMultiplier.ToString() + "x";
        surroundingButton.transform.Find("ScoreBoostValue").GetComponent<TextMeshProUGUI>().text = GameManager.surroundingMultiplier.ToString() + "x";
        highscoreText.GetComponent<TextMeshProUGUI>().text = "Highscore: " + GameManager.highScore.ToString();
        UpdateCheckboxes();
    }

    // Updates all checkboxes to their corresponding colours
    void UpdateCheckboxes()
    {
        bouncyButton.transform.Find("Checkbox").GetComponent<UnityEngine.UI.Image>().color = GameManager.bouncing ? Color.green : Color.gray;
        evilButton.transform.Find("Checkbox").GetComponent<UnityEngine.UI.Image>().color = GameManager.evilCats ? Color.green : Color.gray;
        coolButton.transform.Find("Checkbox").GetComponent<UnityEngine.UI.Image>().color = GameManager.coolCats ? Color.green : Color.gray;
        mirrorButton.transform.Find("Checkbox").GetComponent<UnityEngine.UI.Image>().color = GameManager.invisibleCats ? Color.green : Color.gray;
        surroundingButton.transform.Find("Checkbox").GetComponent<UnityEngine.UI.Image>().color = GameManager.surroundingCats ? Color.green : Color.gray;

    }

    // Switch values when modifier gets clicked
    public void AdjustModifier(string ModifierName)
    {
        // Update modifiers
        if (ModifierName == "Bounce")
            GameManager.bouncing = !GameManager.bouncing;
        
        if (ModifierName == "Evil")
            GameManager.evilCats = !GameManager.evilCats;
        
        if (ModifierName == "Cool")
            GameManager.coolCats = !GameManager.coolCats;
        
        if (ModifierName == "Invisible")
            GameManager.invisibleCats = !GameManager.invisibleCats;
        
        if (ModifierName == "Surrounding")
            GameManager.surroundingCats = !GameManager.surroundingCats;

        // Update checkboxes
        UpdateCheckboxes();
    }
}
