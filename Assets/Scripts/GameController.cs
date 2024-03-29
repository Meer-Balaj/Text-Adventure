using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text displayText;
    public InputAction[] inputActions;
    
    [HideInInspector] public RoomNavigation roomNavigation;
    [HideInInspector] public InteractableItems interactableItems;
    [HideInInspector] public List<string> interactionDescriptionInRoom = new List<string>();
    List<string> actionLog = new List<string>();


    void Awake()
    {
        roomNavigation = GetComponent<RoomNavigation>();
        interactableItems = GetComponent<InteractableItems>();
    }

    private void Start()
    {
        DisplayRoomText(); 
        DisplayLoggedText();
    }

    public void DisplayLoggedText()
    {
        string logAsText = string.Join("\n", actionLog.ToArray());

        displayText.text = logAsText;
    }

    public void DisplayRoomText()
    {
        ClearCollectionsForNewRoom();
        UnPackRoom();

        string joinedInteractionDescription = string.Join("\n", interactionDescriptionInRoom.ToArray());

        string combinedText = roomNavigation.currentRoom.description + "\n" + joinedInteractionDescription;

        LogStringWithReturn(combinedText);
    }

    private void UnPackRoom()
    {
        roomNavigation.UnpackExitsInRoom();
        PrepareObjectsToTakeOrExamine(roomNavigation.currentRoom);
    }

    private void PrepareObjectsToTakeOrExamine(Room currentRoom)
    {
        for (int i = 0; i < currentRoom.interactableObjectsInRoom.Length ; i++)
        {
            string descriptionNotInInventory = interactableItems.GetObjectsNotInInventory(currentRoom, i);
            if(descriptionNotInInventory != null)
            {
                interactionDescriptionInRoom.Add(descriptionNotInInventory);
            }
            InteractableObject interactableInRoom = currentRoom.interactableObjectsInRoom[i];
            for (int j = 0; j < interactableInRoom.interactions.Length; j++)
            {
                Interaction interaction = interactableInRoom.interactions[j];
                if(interaction.inputAction.keyWord == "examine")
                {
                    interactableItems.examineDictionary.Add(interactableInRoom.noun, interaction.textResponse);
                }
                if(interaction.inputAction.keyWord == "take")
                {
                    interactableItems.takeDictionary.Add(interactableInRoom.noun, interaction.textResponse);
                }
            }
        }
    }

    public string TestVerbDictionaryWithNoun(Dictionary<string, string> verbDictionary, string verb, string noun)
    {
        if(verbDictionary.ContainsKey(noun))
        {
            return verbDictionary[noun];
        }
        return "You can't " + verb + " " + noun;
    }

    void ClearCollectionsForNewRoom()
    {
        interactableItems.ClearCollections();
        interactionDescriptionInRoom.Clear();
        roomNavigation.ClearExits();
    }

    public void LogStringWithReturn(string stringToAdd)
    {
        actionLog.Add(stringToAdd + "\n");
    }

    void Update()
    {
        
    }
}
