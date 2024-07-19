// InstructionsManager.cs
using Simulation.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InstructionsManager : MonoBehaviour
{
    public TMP_Text instructionsText;

    private int currentStep = 0;
    private string[] steps = {
        "Step 1: Remove Items to right counter",
        "Step 2: Clean Shelves! \nUse tools from left.\nUse spray & cloth.",
        "Step 3: Discard Expired Items(colored red)."+
        "\nStep 4: Organize Items back to referigerator. User this order. \n Top : Cylinder, Middle : Cube, Bottom : Sphere"
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
            SoundManager.Instance.Play(SoundType.Complete);
        }
    }
    public void PrevStep()
    {
        if (currentStep > 0 )
        {
            currentStep--;
            UpdateInstructions();
        }
    }
    private void UpdateInstructions()
    {
        instructionsText.text = steps[currentStep];
    }
}
