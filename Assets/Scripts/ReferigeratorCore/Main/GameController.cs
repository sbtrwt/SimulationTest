using TMPro;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] private ReferigeratorItem[] items;
    [SerializeField] private TMP_Text completionMessage;
    [SerializeField] private InstructionsManager instructionsManager;
    [SerializeField] private int maxCleanPoint = 6;
    [SerializeField] private int maxItemOut = 0;
    [SerializeField] private Transform cleaningPoint;
    [SerializeField] private GameOverUI gameOverUI;

    private bool isCleaned = false;
    private bool isOrganized = false;
    private int itemRemovedCount = 0;
    private int cleanPointCount = 0;
    // Define the event
    private void Start()
    {
        if (items != null)
        { 
            maxItemOut = items.Where(x => !x.IsOutside).Count(); 
        }
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
        if (items.Count() == items.Where(x => x.IsOutside).Count())
        {
            cleaningPoint.gameObject.SetActive(true);
            instructionsManager.NextStep();
        }
    }
    public void DecreseItemRemovedCount()
    {
      
        if (items.Where(x => !x.IsExpired).Count() == items.Where(x => !x.IsOutside && !x.IsExpired && x.IsRightPlace).Count())
        {
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
