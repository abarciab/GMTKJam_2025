using MyBox;
using UnityEngine;

public class RockExplosion : MonoBehaviour
{
    [SerializeField] private GameObject _rockPrefab;
    [SerializeField] private Vector2Int _rockCount;
    [SerializeField] private float _spawnRadius;
    [SerializeField] private float _verticalOffset;
    [SerializeField] private Vector2 _forceRange;
    [SerializeField] private Vector2 _scaleRange;
    [SerializeField] private float _maxPlayerDistance = 80;

    [ButtonMethod]
    public void SpawnRocks()
    {
        var distance = Vector3.Distance(transform.position, GameManager.i.Player.transform.position);
        if (distance > _maxPlayerDistance) return;

        var count = Random.Range(_rockCount.x, _rockCount.y);
        for (int i = 0; i < count; i++) {
            SpawnRock();
        }
    }

    private void SpawnRock()
    {
        var v2Pos = Random.insideUnitCircle.normalized * _spawnRadius;
        var pos = transform.position + new Vector3(v2Pos.x, 0, v2Pos.y);

        var newRock = Instantiate(_rockPrefab, pos, Quaternion.identity).GetComponent<Rigidbody>();
        var scale = Random.Range(_scaleRange.x, _scaleRange.y);
        newRock.transform.localScale = Vector3.one * scale;

        var dir = newRock.transform.position - transform.position;
        dir.y += _verticalOffset;
        newRock.AddForce(dir * Random.Range(_forceRange.x, _forceRange.y), ForceMode.VelocityChange);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _maxPlayerDistance);
    }

}
