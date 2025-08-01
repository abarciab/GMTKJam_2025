using MyBox;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private Vector2 _viewLimits;
    [SerializeField] private float _speechBubbleLerpFactor;

    private Trade _trade;
    private List<string> _lines = null;
    private string _name;
    private Transform _camera;
    private Vector3 _originalScale;
    private bool _spoken;

    public bool Spoken => _spoken;
    public string HoverName => (!_spoken ? "Talk to " : "Trade with ") + _name;

    private void Start()
    {
        _originalScale = _canvas.localScale;
        _camera = Camera.main.transform;
        _canvas.localScale = Vector3.zero;
    }

    private void Update()
    {
        var distance = Vector3.Distance(transform.position, _camera.position);

        var targetScale = Vector3.zero;
        if (distance < _viewLimits.x) {
            targetScale = Vector3.zero;
        }
        else if (distance <= _viewLimits.y) {
            targetScale = Vector3.Lerp(_originalScale, Vector3.zero, 1 - (distance / _viewLimits.y));
        }
        _canvas.localScale = Vector3.Lerp(_canvas.localScale, targetScale, _speechBubbleLerpFactor * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _viewLimits.x);
        Gizmos.DrawWireSphere(transform.position, _viewLimits.y);
    }

    public void SetData(List<string> lines, Trade trade)
    {
        _trade = new Trade(trade);
        _name = lines[0];
        lines.RemoveAt(0);
        _lines = lines;
    }

    public void StartInteraction()
    {
        if (!_spoken) StartConversation();
        else StartTrade();
    }

    private void StartTrade()
    {
        var requestedItems = _trade.Request.Items.Select(x => x.Data.Type).ToList();
        var requestContainsUnknownItems = requestedItems.Where(x => !GameManager.i.ItemDiscovered(x)).Count() > 0;
        if (requestContainsUnknownItems) {
            var lines = new List<string>() { "I'm sorry, you don't even know about the resources I'm after. Come back later" };
            UIManager.i.Do(UIAction.START_CONVERSATION, lines);
            return;
        }

        UIManager.i.Do(UIAction.START_TRADE, _trade);
    }

    private void StartConversation()
    {
        _spoken = true;
        UIManager.i.Do(UIAction.START_CONVERSATION, _lines);
    }

}
