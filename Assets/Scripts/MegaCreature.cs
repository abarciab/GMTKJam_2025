using MyBox;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class MegaCreature : MonoBehaviour
{
    [SerializeField] private List<Foot> _feet;
    [SerializeField] private float _initialDelay;
    [SerializeField] private float _resetDistance;
    [SerializeField] private Transform _body;

    private List<Vector3> _footStartingPositions = new List<Vector3>();
    private Vector3 _startingPosition;

    private void Start()
    {
        UpdateBodyPosition();
        _startingPosition = _body.position;
        foreach (Foot foot in _feet) {
            _footStartingPositions.Add(foot.transform.localPosition);
            foot.InitialWaitTime += _initialDelay;
        }
    }

    private void Update()
    {
        if (Vector3.Distance(_body.position, _startingPosition) > _resetDistance) {
            foreach (var f in _feet) {
                f.transform.position -= transform.forward * _resetDistance;
            }
            for (int i = 0; i < _footStartingPositions.Count; i++) {
                _feet[i].transform.localPosition = _footStartingPositions[i];
            }
        }
        UpdateBodyPosition();
    }

    [ButtonMethod]
    private void UpdateBodyPosition()
    {
        var total = new Vector3();
        foreach (var foot in _feet) total += foot.transform.position;
        var center = new Vector3(total.x / _feet.Count, transform.position.y, total.z / _feet.Count);
        _body.position = center;
        if (!Application.isPlaying) Utils.SetDirty(_body);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_body.position, _body.position + transform.forward * _resetDistance);
        }
        else {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_body.position, _body.position + transform.forward * -_resetDistance);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_body.position, _startingPosition);
        }
    }
}
