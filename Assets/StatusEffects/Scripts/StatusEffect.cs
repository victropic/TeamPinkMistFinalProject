using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "ScriptableObjects/StatusEffect", order = 1)]
public class StatusEffect : ScriptableObject
{
    public enum StatEffected {
        Health, 
        Energy,
        MaxHealth,
        MaxEnergy
    }
    
    public string statusName;
    public StatEffected statEffected;
    public float amount;
    public float duration;
    public bool stackable;

}
