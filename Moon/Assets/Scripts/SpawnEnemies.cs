using System.Collections;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [SerializeField] private Vector2 _spawnDelayMinMax;
    [SerializeField] private int _maxCount;
    [SerializeField] private int _initalCount;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;

    private void Start()
    {
        StartCoroutine(WaitAndSpawn());
        for (var i = 0; i < _initalCount; i++)
        {
            Spawn();
        }
    }

    private IEnumerator WaitAndSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(_spawnDelayMinMax.x, _spawnDelayMinMax.y));
            Spawn();
        }
    }

    private void Spawn()
    {
        Instantiate(_enemyPrefab, _spawnPoints[Random.Range(0, _spawnPoints.Length)].position, Quaternion.identity);
    }
}
