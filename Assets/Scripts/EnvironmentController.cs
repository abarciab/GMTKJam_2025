using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentController : MonoBehaviour
{
    [SerializeField] private Transform _roadParent;
    [SerializeField] private AreaController _areaController;
    [SerializeField] private List<GameObject> _areaPrefabs = new List<GameObject>();
    private List<GameObject> _usedAreas = new List<GameObject>();

    private void Start()
    {
        if (_areaController) _areaController.EnterArea();
        else EnterNewArea();
    }

    public void EnterNewArea()
    {
        GameManager.i.Car.GetComponent<Rigidbody>().isKinematic = true;
        if (_areaController) Destroy(_areaController.gameObject);

        var options = _areaPrefabs.Where(x => x.GetComponent<AreaController>().name != _areaController.name).ToList();
        var selectedArea = options[Random.Range(0, options.Count)];
        _areaController = Instantiate(selectedArea, transform).GetComponent<AreaController>();

        _areaPrefabs.Remove(selectedArea);
        _usedAreas.Add(selectedArea);

        if (_areaPrefabs.Count == 0) _areaPrefabs = _usedAreas;

        _areaController.EnterArea();
        gameObject.SetActive(true);

        foreach (Transform child in _roadParent) child.gameObject.SetActive(child.GetSiblingIndex() == _areaController.roadIndex);
    }

}
