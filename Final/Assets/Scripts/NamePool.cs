using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NamePool
{
    public string[] firstNamePool = {
        "Henry", "Otto", "Conrad", "Frederick", "Louis",
        "Albert", "Ernest", "Godfrey", "Herman", "Lothar",
        "Manfred", "Rudolph", "Siegfried", "Werner", "William",
        "Adelaide", "Agnes", "Beatrix", "Bertha", "Elisabeth",
        "Gertrude", "Hedwig", "Hildegard", "Irmgard", "Judith",
        "Kunigunde", "Matilda", "Mechthild", "Sophia", "Uta",
        "John", "Peter", "Paul", "Andrew", "Maximilian",
        "Leopold", "George", "Christopher", "Nicholas", "Albert",
        "Bruno", "Ekkehard", "Volmar", "Gebhard", "Hartmann",
        "Ingo", "Joachim", "Karl", "Lambert", "Matthias"
    };
    
    public string[] middleNamePool = {
        "of the", "the", "from", "son of", "daughter of"
    };
    
    public string[] lastNamePool = {
        "Welf"
    };
    
    public string[] titlePool = {
        "the Hero", "the Warrior", "the Mage", "the Rogue",
        "the Explorer", "the Guardian", "the Slayer", "the Wanderer"
    };
}