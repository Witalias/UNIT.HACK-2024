using System;
using System.Collections;
using UnityEngine;

public class HealthBar : UIBar
{
    [SerializeField] private float _decreaseSpeed;

    private float _value = 1.0f;

    public static event Action OnEated;

    private void Start()
    {
        StartCoroutine(ProcessDeacrease());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Inventory.Instance.IsSelect(Inventory.FoodItems) && Inventory.Instance.TryPeekSelected())
        {
            AudioManager.Instanse.Play(AudioType.Eat);
            Add(0.2f);
            OnEated?.Invoke();
        }
    }

    public void Add(float value)
    {
        _value = Mathf.Clamp01(_value + value);
        SetValue(_value, 1.0f);
    }

    private IEnumerator ProcessDeacrease()
    {
        while (_value > 0)
        {
            _value -= Time.deltaTime * _decreaseSpeed;
            SetValue(_value, 1.0f);
            yield return null;
        }
        Death();
    }

    private void Death()
    {
        Debug.Log("Death");
    }
}
