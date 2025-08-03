using MyBox;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Drop
{
    public Item Item;
    [Range(0, 1)] public float Rarity = 0.5f;
}

public class Collectable : MonoBehaviour
{
    [SerializeField] private Animator _blockAnimator;
    [SerializeField] private List<GameObject> _modelVariants = new List<GameObject>();
    [SerializeField] private string _displayname;
    [SerializeField] private List<Drop> _drops;
    [SerializeField] private Vector2Int _dropQuantityRange = new Vector2Int(1, 1);
    [SerializeField] private bool _breakable;
    [SerializeField, ConditionalField(nameof(_breakable))] private float _breakTime;
    [SerializeField, ConditionalField(nameof(_breakable))] private Sound _breakSound;

    private float _idleTimer = 0.1f;
    private float _idleTimerLeft;

    public bool Breakable => _breakable;   
    public float BreakTime => _breakTime;
    public string DisplayName => _displayname;

    private void Start()
    {
        if (_breakSound) _breakSound = Instantiate(_breakSound);

        SetRandomModelVariantActive();
    }

    private void Update()
    {
        if (!_breakable) return;
        _idleTimerLeft -= Time.deltaTime;
        if (_idleTimerLeft <= 0) SetBreakProgress(0);
    }

    private void OnDisable()
    {
        if (_breakSound) _breakSound.Stop();
    }

    public List<Item> GetDrops()
    {
        var results = new List<Item>();
        var numDrops = Random.Range(_dropQuantityRange.x, _dropQuantityRange.y+1);
        for (int i = 0; i < numDrops; i++) {
            results.Add(new Item(GetDrop()));
        }
        return results;
    }

    private Item GetDrop()
    {
        var total = 0f;
        foreach (var drop in _drops) total += drop.Rarity;
        var roll = Random.Range(0, total);
        var partial = 0f;
        foreach (var drop in _drops) {
            partial += drop.Rarity;
            if (partial > roll) {
                var chosen = new Item(drop.Item);
                return chosen;
            }
        }
        return _drops[Random.Range(0, _drops.Count)].Item;
    }

    public void SetBreakProgress(float progress)
    {
        if (progress > 0) {
            _idleTimerLeft = _idleTimer;
            _breakSound.Play(restart: false);
        }

        _blockAnimator.SetBool("Breaking", progress > 0);
    }

    private void SetRandomModelVariantActive()
    {
        if (_modelVariants == null || _modelVariants.Count < 2) return;

        foreach (var variant in _modelVariants)
        {
            variant.SetActive(false);
        }

        int randomIndex = Random.Range(0, _modelVariants.Count);
        _modelVariants[randomIndex].SetActive(true);
    }
}
