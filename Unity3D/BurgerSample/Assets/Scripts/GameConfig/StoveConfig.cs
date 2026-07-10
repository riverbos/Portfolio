using UnityEngine;

[CreateAssetMenu(fileName = "StoveConfig", menuName = "Burger/Stove Config")]
public class StoveConfig : ScriptableObject
{
    [Min(0.1f)]
    public float cookingTime = 5f;
    [Min(1)]
    public int maxBurgersOnStove = 3;
    [Min(0f)]
    public float burgerHeight = 1.0f;

    private void OnValidate()
    {
        cookingTime = Mathf.Max(0.1f, cookingTime);
        maxBurgersOnStove = Mathf.Max(1, maxBurgersOnStove);
        burgerHeight = Mathf.Max(0f, burgerHeight);
    }
}
