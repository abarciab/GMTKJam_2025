using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InputController))]
public class GameManager : MonoBehaviour
{
    public static GameManager i;

    public void LoadMenu() => Utils.TransitionToScene(0);
    public void EndGame() => Utils.TransitionToScene(2);
    public Transform Player => null;
    public Transform Camera => null;

    private void Awake()
    {
        if (i != null) Destroy(i.gameObject);
        i = this;
    }
}
