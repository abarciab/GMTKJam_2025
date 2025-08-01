using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] private Vector2 _limits;
    [SerializeField] private Vector2 _yLimits;
    [SerializeField] private Vector2 _scaleLimits;
    [SerializeField] private GameObject _cloudPrefab;
    [SerializeField] private Vector2 _speedDir;
    [SerializeField] private int _numClouds = 20;

    private Vector3 _speed => new Vector3(_speedDir.x, 0, _speedDir.y) * Time.deltaTime;

    private void Start()
    {
        SpawnClouds();
    }

    private void Update()
    {
        foreach (Transform _child in transform) {
            var offset = (float)_child.GetSiblingIndex() / transform.childCount;
            _child.transform.position += _speed * offset;

            if (_child.transform.position.x > transform.position.x + _limits.x) _child.transform.position -= Vector3.right * _limits.x * 2;
            if (_child.transform.position.x < transform.position.x - _limits.x) _child.transform.position -= Vector3.left * _limits.x * 2;
            if (_child.transform.position.z > transform.position.z + _limits.y) _child.transform.position -= Vector3.forward * _limits.y * 2;
            if (_child.transform.position.z > transform.position.z + _limits.y) _child.transform.position -= Vector3.back * _limits.y * 2;
        }
    }

    private void SpawnClouds()
    {
        for (int i = 0; i < _numClouds; i++) {
            SpawnCloud();
        }
    }

    private void SpawnCloud()
    {
        var x = Random.Range(-_limits.x, _limits.x);
        var y = Random.Range(_yLimits.x, _yLimits.y);
        var z = Random.Range(-_limits.y, _limits.y);
        var position = transform.position + new Vector3(x, y, z);

        var rot = Quaternion.Euler(0, Random.Range(0, 360), 0);

        var newCloud = Instantiate(_cloudPrefab, position, rot, transform);
        newCloud.transform.localScale = Vector3.one * Random.Range(_scaleLimits.x, _scaleLimits.y);
    }

    private void OnDrawGizmosSelected()
    {
        var rightEdgeCenter = transform.position + Vector3.right * _limits.x;
        var leftEdgeCenter = transform.position + Vector3.left * _limits.x;
        
        var topRightCorner = rightEdgeCenter + Vector3.forward * _limits.y;
        var bottomRightCorner = rightEdgeCenter + Vector3.back * _limits.y;

        var topLeftCorner = leftEdgeCenter + Vector3.forward * _limits.y;
        var bottomLeftCorner = leftEdgeCenter + Vector3.back * _limits.y;

        Gizmos.DrawLine(bottomLeftCorner, bottomRightCorner); //bottom left, bottom right
        Gizmos.DrawLine(topLeftCorner, topRightCorner); //top left, top right

        Gizmos.DrawLine(topRightCorner, bottomRightCorner); //top right, bottom right
        Gizmos.DrawLine(topLeftCorner, bottomLeftCorner); //top left, bottom left
    }
}
