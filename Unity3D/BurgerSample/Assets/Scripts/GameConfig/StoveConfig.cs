using UnityEngine;

[CreateAssetMenu(fileName = "StoveConfig", menuName = "Burger/Stove Config")]
public class StoveConfig : ScriptableObject
{
    public float cookingTime = 5f;
    public int maxBurgersOnStove = 3;
    public float burgerHeight = 1.0f;
}
