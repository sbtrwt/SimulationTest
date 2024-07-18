using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private TMP_Text completionMessage;
    [SerializeField] private InstructionsManager instructionsManager;
    [SerializeField] private int maxCleanPoint = 6;
    [SerializeField] private Transform cleaningPoint;
    [SerializeField] private GameOverUI gameOverUI;

    private bool isCleaned = false;
    private bool isOrganized = false;
    private int itemRemovedCount = 0;
    private int cleanPointCount = 0;
    // Define the event
    private void Start()
    {
      
    }


    void Update()
    {
        if ( isCleaned && isOrganized)
        {
            DisplayCompletionMessage();
            gameOverUI.GameOver("Refrigerator successfully cleaned and organized!");
        }
    }
    public void CleanShelves()
    {
        isCleaned = true;
    }

    public void OrganizeItems()
    {
        isOrganized = true;
    }

    private void DisplayCompletionMessage()
    {
        completionMessage.text = "Refrigerator successfully cleaned and organized!";
    }

    // Method to be called when an item is removed
    public void IncreaseItemRemovedCount()
    {
        itemRemovedCount++;
        if(items != null && itemRemovedCount >= items.Length)
        {
            itemRemovedCount = items.Length;
            cleaningPoint.gameObject.SetActive(true);
            instructionsManager.NextStep();
        }
        Debug.Log("Item removed. Current count: " + itemRemovedCount);
    }
    public void DecreseItemRemovedCount()
    {
        itemRemovedCount--;
        if (itemRemovedCount <= 0)
        {
            itemRemovedCount = 0;
            Debug.Log("Item removed. Current count: " + itemRemovedCount);
            if (isCleaned)
            {
                OrganizeItems();
                instructionsManager.NextStep();
            }
        }
      
    }
    private void CountCleaningPoint() 
    {
        cleanPointCount++;
        if (cleanPointCount >= maxCleanPoint-1)
        {
            CleanShelves();
            cleanPointCount = maxCleanPoint;
            cleaningPoint.gameObject.SetActive(false);
            instructionsManager.NextStep();
        }
        Debug.Log("Cleanin action count : " + cleanPointCount);
    }
    private void OnEnable()
    {
        DraggableItem.OnItemRemoved += IncreaseItemRemovedCount;
        CleaningItem.OnCleanedPoint += CountCleaningPoint;
        DraggableItem.OnItemOrganized += DecreseItemRemovedCount;
    }

    private void OnDisable()
    {
        DraggableItem.OnItemRemoved -= IncreaseItemRemovedCount;
        CleaningItem.OnCleanedPoint -= CountCleaningPoint;
        DraggableItem.OnItemOrganized -= DecreseItemRemovedCount;

    }
}
