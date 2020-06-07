using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishTrigger : MonoBehaviour
{

    private GameController gameController;

    public void Activate(GameController gameController)
    {
        this.gameController = gameController;
    }

    private void OnTriggerEnter(Collider other)
    {
        gameController.FinishGame();
    }
}
