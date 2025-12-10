using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamePool
{
    public string[] firstNamePool = {
        "Brave", "Shadow", "Iron", "Swift", "Dark", 
        "Mystic", "Wild", "Steel", "Frost", "Flame"
    };
    
    public string[] middleNamePool = {
        "of the", "the", "from", "son of", "daughter of"
    };
    
    public string[] lastNamePool = {
        "Mountain", "Forest", "Sea", "Sky", "Night",
        "Sun", "Moon", "Star", "River", "Stone"
    };
    
    public string[] titlePool = {
        "the Hero", "the Warrior", "the Mage", "the Rogue",
        "the Explorer", "the Guardian", "the Slayer", "the Wanderer"
    };
}