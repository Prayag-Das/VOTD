using UnityEngine;
using System.Collections;

public class OxygenLeakSeal : MonoBehaviour, IInteractable
{
    [Header("Required Items")]
    [SerializeField] private ItemData blowtorchItem;
    [SerializeField] private ItemData steelPlateItem;

    [Header("References")]
    [SerializeField] private PuzzleElement puzzleElement; // (Hook to correct Puzzle Element)
    [SerializeField] private InventorySystem inventory;
    [SerializeField] private PuzzleRoomManager roomManager;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject sealedPipeModel;

    private bool taskCompleted = false;

    private void Start()
    {
        if (inventory == null)
            inventory = FindObjectOfType<InventorySystem>();
    }
    
    public void Interact()
    {
        if (taskCompleted)
        {
            Debug.Log("Oxygen leak already sealed.");
            return;
        }

        if (inventory.HasItem(steelPlateItem) && inventory.HasItem(blowtorchItem))
        {
            inventory.RemoveItem(steelPlateItem);
            Debug.Log("Used steel plate.");

            puzzleElement.MarkCompleted();
            roomManager.CheckPuzzleProgress();

            if (sealedPipeModel != null)
            {
                sealedPipeModel.SetActive(true); //  Show sealed pipe or patch
            }

            //  Reduce sanity decay rate now that the leak is sealed
            SanitySystem sanity = FindFirstObjectByType<SanitySystem>();
            if (sanity != null)
            {
                sanity.SetDecayRate(0.4f);  // Example: slower drain
                Debug.Log("Sanity decay rate reduced after sealing the leak.");
            }


            taskCompleted = true; //  mark task as done
            Debug.Log("Oxygen leak sealed successfully.");
        }
        else
        {
            Debug.Log("Missing required items: steel plate and/or blowtorch.");
        }
    }
}
