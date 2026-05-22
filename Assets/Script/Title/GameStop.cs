using UnityEngine;

public class GameStop : MonoBehaviour
{
    public bool isGameStop { get; private set; } = true;

    public void StopGame()
    {
        isGameStop = true;
    }

    public void StartGame()
    {
        isGameStop = false;
    }
}
