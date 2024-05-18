using UnityEngine;

public class RandomMeshOnStart : MonoBehaviour
{
    [SerializeField] private MeshFilter _meshFilter;
    [SerializeField] private MeshCollider _meshCollider;
    [SerializeField] private Mesh[] _meshes;

    private void Start()
    {
        var mesh = _meshes[Random.Range(0, _meshes.Length)];
        _meshFilter.mesh = mesh;
        _meshCollider.sharedMesh = mesh;
    }
}
