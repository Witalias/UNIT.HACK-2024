using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _stoppingDistance;
    [SerializeField] private float _shootDelay;
    [SerializeField][Range(0.0f, 1.0f)] private float _damage;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private TriggerZone _detectionTrigger;
    [SerializeField] private TriggerZone _shootTrigger;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;
    [SerializeField] private Animator _animator;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
    [SerializeField] private Bullet _projectilePrefab;
    [SerializeField] private Transform _bulletSpawnPoint;

    private Vector3 _target;
    private Tween _currentTween;
    private Transform _playerTransform;
    private bool _reached;
    private bool _worried;
    private bool _canShoot;
    private float _health = 1.0f;

    private void Start()
    {
        _detectionTrigger.SubscribeOnEnter(OnPlayerDetectionTriggerEnter);
        _detectionTrigger.SubscribeOnExit(OnPlayerDetectionTriggerExit);
        _shootTrigger.SubscribeOnEnter(OnPlayerShootTriggerEnter);
        _shootTrigger.SubscribeOnExit(OnPlayerShootTriggerExit);
        Patrol();
    }

    private void Update()
    {
        if (!_reached && Vector3.Distance(transform.position, _target) < _stoppingDistance)
        {
            _reached = true;
            StopAndPatrol();
        }
        if (_worried)
        {
            if (!_canShoot)
                _rigidbody.velocity = (_playerTransform.position - transform.position).normalized * _speed;
            RotateTowards(_playerTransform.position, 0.0f);
        }
    }

    public void AddDamage(float value)
    {
        if (_health > 0.0f)
        {
            _health -= value;
            var color = _skinnedMeshRenderer.material.color;
            _skinnedMeshRenderer.material.color = Color.red;
            DOVirtual.DelayedCall(0.2f, () => _skinnedMeshRenderer.material.color = color);
            AudioManager.Instanse.Play(AudioType.EnemyDamage);
            StopAndPatrol();
            if (_health <= 0.0f)
            {
                _animator.SetTrigger("Death");
                _rigidbody.useGravity = true;
                DOVirtual.DelayedCall(3.0f, () => Destroy(gameObject));
            }
        }
    }

    private void StopAndPatrol()
    {
        _rigidbody.velocity = Vector3.zero;
        _currentTween?.Kill();
        _currentTween = DOVirtual.DelayedCall(4.0f, Patrol);
    }

    private void Patrol()
    {
        _reached = false;
        _target = GameData.Instance.GetRandomEnemyPatrolPoint() + Random.insideUnitSphere * 2.5f;
        _rigidbody.velocity = (_target - transform.position).normalized * _speed;
        RotateTowards(_target, 0.5f);
    }

    private void RotateTowards(Vector3 position, float duration)
    {
        var angle = Quaternion.LookRotation(position - transform.position).eulerAngles;
        transform.DORotate(angle, duration);
    }

    private void OnPlayerDetectionTriggerEnter(Player player)
    {
        _playerTransform = player.CenterPoint;
        _reached = true;
        _worried = true;
    }

    private void OnPlayerDetectionTriggerExit(Player _)
    {
        _worried = false;
        StopAndPatrol();
    }

    private void OnPlayerShootTriggerEnter(Player player)
    {
        _canShoot = true;
        _rigidbody.velocity = Vector3.zero;
        StopAllCoroutines();
        StartCoroutine(ProcessShoot());
    }

    private void OnPlayerShootTriggerExit(Player _)
    {
        _canShoot = false;
    }

    private IEnumerator ProcessShoot()
    {
        var wait = new WaitForSeconds(_shootDelay);
        while (_canShoot)
        {
            yield return wait;
            _animator.SetTrigger("Shoot");
            var projectile = Instantiate(_projectilePrefab, _bulletSpawnPoint.position, Quaternion.identity);
            projectile.Damage = _damage;
            projectile.FromEnemy = true;
            projectile.GetComponent<Rigidbody>().velocity = (_playerTransform.position - _bulletSpawnPoint.position).normalized * _projectileSpeed;
        }
    }
}
