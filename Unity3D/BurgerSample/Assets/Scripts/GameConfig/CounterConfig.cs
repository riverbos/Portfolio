using UnityEngine;

[CreateAssetMenu(fileName = "CounterConfig", menuName = "Burger/Counter Config")]
public class CounterConfig : ScriptableObject
{
    [Min(1)]
    public int maxBurgersOnCounter = 20;
    [Min(0f)]
    public float stackHeight = 0.3f;
    [Min(0)]
    public int pointsPerBurger = 10;
    [Min(0.1f)]
    public float sellInterval = 8.0f;
    [Min(0f)]
    public float sellIntervalVariation = 2.0f;

    private void OnValidate()
    {
        maxBurgersOnCounter = Mathf.Max(1, maxBurgersOnCounter);
        stackHeight = Mathf.Max(0f, stackHeight);
        pointsPerBurger = Mathf.Max(0, pointsPerBurger);
        sellInterval = Mathf.Max(0.1f, sellInterval);
        sellIntervalVariation = Mathf.Clamp(sellIntervalVariation, 0f, sellInterval);
    }
}
