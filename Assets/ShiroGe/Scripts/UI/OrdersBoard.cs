using System;
using System.Collections.Generic;
using ShiroGe.Scripts.Quests;
using ShiroGe.Scripts.Quests.Orders;
using UnityEngine;

public class OrdersBoard : MonoBehaviour
{
    [SerializeField] private GameObject orderPanelSmallPrefab;
    [SerializeField] private GameObject content;

    public List<CurrentQuestPanel> CurrentQuestPanels { get; private set; } = new List<CurrentQuestPanel>();

    public void AddOrder(QuestOrderBase newOrderQuest)
    {
        GameObject newQuestPanel = Instantiate(orderPanelSmallPrefab, content.transform);
        CurrentQuestPanel questPanelScript = newQuestPanel.GetComponent<CurrentQuestPanel>();
        questPanelScript.SetQuest(newOrderQuest);
        
        CurrentQuestPanels.Add(questPanelScript);
    }
}
