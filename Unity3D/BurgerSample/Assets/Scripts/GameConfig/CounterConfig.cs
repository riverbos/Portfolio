using UnityEngine;

[CreateAssetMenu(fileName = "CounterConfig", menuName = "Burger/Counter Config")]
public class CounterConfig : ScriptableObject
{
    public int maxBurgersOnCounter = 20;
    public float stackHeight = 0.3f;
    public int pointsPerBurger = 10;
    public float sellInterval = 8.0f;
    public float sellIntervalVariation = 2.0f;
}
