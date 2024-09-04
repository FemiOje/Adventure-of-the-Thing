using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelsData", menuName = "ScriptableObjects/LevelsData", order = 1)]
public class LevelsData : ScriptableObject
{
    public List<LevelData> levels;
}
