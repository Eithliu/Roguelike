using UnityEngine;

public class TurnManager
{
    public event System.Action OnTick;
    private int m_TurnCount;

    public TurnManager()
    {
        m_TurnCount = 1;
    }

    public void Tick()
    {
        OnTick?.Invoke();
        m_TurnCount++;
        Debug.Log("Il y a " + m_TurnCount + " tours");
    }


}
