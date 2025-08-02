using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BowController : MonoBehaviour
{
    [SerializeField] private Animator _bow;
    [SerializeField] private Transform _arrow;
    [SerializeField] private float _putAwayTime;
    [SerializeField] private Vector2 _arrowYLimits;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private List<Transform> _linePoints = new List<Transform>();

    private float _timeSinceUsed;
    private bool _drawn;

    private void Update()
    {
        if (_drawn) {
            _timeSinceUsed += Time.deltaTime;
            if (_timeSinceUsed > _putAwayTime) {
                _bow.SetTrigger("PutAway");
                _drawn = false;
            }
        }

        if (_arrow.gameObject.activeInHierarchy) {
            _line.SetPositions(_linePoints.Select(x => x.position).ToArray());
        }
        else {
            var points = new List<Vector3>() { _linePoints[0].position, _linePoints[2].position, _linePoints[2].position };
            _line.SetPositions(points.ToArray());
        }

    }

    public void UpdateArrow(float arrowPercent)
    {

        if (arrowPercent > 0) {
            if (!_drawn) _bow.SetTrigger("Draw");
            _drawn = true;
            _timeSinceUsed = 0;

            var pos = _arrow.localPosition;
            pos.y = Mathf.Lerp(_arrowYLimits.x, _arrowYLimits.y, arrowPercent);
            _arrow.localPosition = pos;

        }
        _arrow.gameObject.SetActive(arrowPercent > 0);
    }
}
