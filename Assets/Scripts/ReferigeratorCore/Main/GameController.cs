using TMPro;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    [SerializeField] private ReferigeratorItem[] items;
    [SerializeField] private TMP_Text completionMessage;
    [SerializeField] private InstructionsManager instructionsManager;
    [SerializeField] private int maxCleanPoint = 6;
   
    [SerializeField] private Transform cleaningPoint;
    [SerializeField] private GameOverUI gameOverUI;

    public delegate void GameStateHandler();
    public static event GameStateHandler OnGameOver;

    private bool isCleaned = false;
    private bool isOrganized = false;
    private bool isItemOut = false;
    private bool isGameOver = false;
    private int cleanPointCount = 0;
    // Define the event
    private void Start()
    {
       
    }


    void Update()
    {
        if (!isGameOver &&  isCleaned && isOrganized)
        {
            OnGameOver?.Invoke();
            float minutes = Mathf.FloorToInt(Timer.TimeTaken / 60);
            float seconds = Mathf.FloorToInt(Timer.TimeTaken % 60);
            string message = "Refrigerator successfully cleaned and organized!\n Time taken - " + string.Format("{0:00}:{1:00}", minutes, seconds);
            DisplayCompletionMessage(message);
            gameOverUI.GameOver(message);
            isGameOver = true;
        }
    }
    public void SetCleanShelves(bool isCleanedToSet)
    {
       isCleaned = isCleanedToSet;
    }

    public void SetOrganizeItems(bool isOrganizedToSet)
    {
        isOrganized = isOrganizedToSet;
    }
    public void SetItemOut(bool isItemOutToSet)
    {
        isItemOut = isItemOutToSet;
    }

    private void DisplayCompletionMessage(string message)
    {
       

        completionMessage.text = message;
    }

    // Method to be called when an item is removed
    public void IncreaseItemRemovedCount()
    {
        if (items.Count() == items.Where(x => x.IsOutside).Count())
        {
            cleaningPoint.gameObject.SetActive(true);
           
            if(!isItemOut)
                instructionsManager.NextStep();
            SetItemOut(true);
        }
    }
    public void DecreseItemRemovedCount()
    {
        if (items.Where(x => !x.IsExpired).Count() == items.Where(x => !x.IsOutside && !x.IsExpired && x.IsRightPlace).Count())
        {  
            if (isCleaned)
            {
                if(!isOrganized)
                    instructionsManager.NextStep();
                SetOrganizeItems(true);
            }
        }
        else
        {
            if (!isCleaned)
            {
                cleaningPoint.gameObject.SetActive(false);
                SetItemOut(false);
                instructionsManager.PrevStep();
            }
        }
        
      
    }
    private void CountCleaningPoint() 
    {
        if (items.Count() == items.Where(x => x.IsOutside).Count())
        {
            cleanPointCount++;
            if (cleanPointCount >= maxCleanPoint - 1)
            {
                SetCleanShelves(true);
                cleanPointCount = maxCleanPoint;
                cleaningPoint.gameObject.SetActive(false);

                instructionsManager.NextStep();
            }
           
        }
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
