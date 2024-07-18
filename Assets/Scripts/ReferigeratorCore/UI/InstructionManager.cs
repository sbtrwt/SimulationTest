// InstructionsManager.cs
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsManager : MonoBehaviour
{
    public TMP_Text instructionsText;

    private int currentStep = 0;
    private string[] steps = {
        "Step 1: Remove Items",
        "Step 2: Clean Shelves",
        "Step 3: Discard Expired Items",
        "Step 4: Organize Items"
    };

    void Start()
    {
        UpdateInstructions();
    }

    public void NextStep()
    {
        if (currentStep < steps.Length - 1)
        {
            currentStep++;
            UpdateInstructions();
        }
    }

    private void UpdateInstructions()
    {
        instructionsText.text = steps[currentStep];
    }
}
