    using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof(RawImage))]
public class ScrollRawImage : MonoBehaviour
{
    [SerializeField] private Vector2 _speed;

    private RawImage _img;

    private void Start()
    {
        _img = GetComponent<RawImage>();
    }

    void Update()
    {
        var rect = _img.uvRect;
        rect.x += _speed.x * Time.deltaTime;
        rect.y += _speed.y * Time.deltaTime;
        _img.uvRect = rect;
    }
}
