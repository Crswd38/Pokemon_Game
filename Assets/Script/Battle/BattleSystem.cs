using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour {
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;

    private void Start() {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle() {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }
    void PlayerAction() {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove() {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove() {
        var move = playerUnit.Pokemon.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");
        yield return new WaitForSeconds(1f);

        bool isFainted = enemyUnit.Pokemon.TakeDamage(move, enemyUnit.Pokemon);
        yield return enemyHud.UpdateHP();

        if(isFainted) {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} Fainted");
        }else {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove() {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}");
        yield return new WaitForSeconds(1f);

        bool isFainted = playerUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return playerHud.UpdateHP();

        if(isFainted) {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} Fainted");
        }else {
            PlayerAction();
        }
    }

    private void Update() {
        if(state == BattleState.PlayerAction) {
            HandleActionSelection();
        }else if(state == BattleState.PlayerMove) {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection() {
        if(Input.GetKeyDown(KeyCode.RightArrow)) {
            if(currentAction < 3) {
                currentAction += 1;
            }
        }else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (currentAction > 0) {
                currentAction -= 1;
            }
        }else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            if(currentAction < 2) {
                currentAction += 2;
            }
        }else if(Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentAction > 1) {
                currentAction -= 2;
            }
        }

        dialogBox.UpdateActionSelection(currentAction);

        if(Input.GetKeyDown(KeyCode.Z)) {
            if(currentAction == 0) {
                PlayerMove();
            }else if(currentAction == 1) {
                //bag
            }else if(currentAction == 2) {
                //pokemon
            }else if(currentAction == 3) {
                //run
            }
        }
    }

    void HandleMoveSelection() {
        if(Input.GetKeyDown(KeyCode.RightArrow)) {
            if(currentMove < playerUnit.Pokemon.Moves.Count - 1) {
                ++currentMove;
            }
        }else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            if (currentMove > 0) {
                --currentMove;
            }
        }else if(Input.GetKeyDown(KeyCode.DownArrow)) {
            if(currentMove < playerUnit.Pokemon.Moves.Count - 2) {
                currentMove += 2;
            }
        }else if(Input.GetKeyDown(KeyCode.UpArrow)) {
            if (currentMove > 1) {
                currentMove -= 2;
            }
        }
        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);
        if(Input.GetKeyDown(KeyCode.Z)) {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
}