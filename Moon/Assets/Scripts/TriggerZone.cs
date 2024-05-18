using System;
using UnityEngine;
using UnityEngine.Events;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] private UnityEvent OnPlayerEnter;
    [SerializeField] private UnityEvent OnPlayerExit;
    [SerializeField] private UnityEvent OnPlayerStay;

    private event Action<Player> OnEnter;
    private event Action<Player> OnExit;
    private event Action<Player> OnStay;

    private int _intersectionCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (_intersectionCount++ == 0)
            {
                OnEnter?.Invoke(player);
                OnPlayerEnter?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            if (--_intersectionCount == 0)
            {
                OnExit?.Invoke(player);
                OnPlayerExit?.Invoke();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            OnStay?.Invoke(player);
            OnPlayerStay?.Invoke();
        }
    }

    public void SubscribeOnEnter(Action<Player> action) => OnEnter += action;

    public void UnsubscribeOnEnter(Action<Player> action) => OnEnter -= action;

    public void SubscribeOnExit(Action<Player> action) => OnExit += action;

    public void UnsubscribeOnExit(Action<Player> action) => OnExit -= action;

    public void SubscribeOnStay(Action<Player> action) => OnStay += action;

    public void UnsubscribeOnStay(Action<Player> action) => OnStay -= action;
}
