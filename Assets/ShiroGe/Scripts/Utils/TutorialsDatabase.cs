using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class TutorialDatabaseRow
{
    public string key;
    public string title;
    public string value;
    public bool isShowed = false;
}

[CreateAssetMenu(fileName = "TutorialBase", menuName = "ShiroGe/TutorialBase")]
public class TutorialsDatabase : ScriptableObject
{
    public List<TutorialDatabaseRow> tutorialDatabase;
}