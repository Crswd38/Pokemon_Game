using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pokemon {

    public PokemonBase Base { get; set; }
    public int Level { get; set; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Pokemon(PokemonBase pBase, int pLevel) {
        Base = pBase;
        Level = pLevel;
        HP = MaxHp;

        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves) {
            if(move.Level <= Level) {
                Moves.Add(new Move(move.Base));
                if (Moves.Count >= 4) {
                    break;
                }
            }
        }
    }

    public int Attack {
        get { return Mathf.FloorToInt(Base.Attack * Level / 100f) + 5; }
    }

    public int Defense {
        get { return Mathf.FloorToInt(Base.Defense * Level / 100f) + 5; }
    }

    public int SpAttack {
        get { return Mathf.FloorToInt(Base.SpAttack * Level / 100f) + 5; }
    }

    public int SpDefense {
        get { return Mathf.FloorToInt(Base.SpDefense * Level / 100f) + 5; }
    }

    public int Speed {
        get { return Mathf.FloorToInt(Base.Speed * Level / 100f) + 5; }
    }

    public int MaxHp {
        get { return Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10; }
    }

    public bool TakeDamage(Move move, Pokemon attacker) {
        float modifiers = Random.Range(0.85f, 1f);
        int damage = Mathf.FloorToInt(((2 * attacker.Level / 5f + 2) * move.Base.Power * ((float)attacker.Attack / Defense)/50 + 2) * modifiers);

        HP -= damage;
        if(HP <= 0) {
            HP = 0;
            return true;
        }
        return false;
    }

    public Move GetRandomMove() {
        int r = Random.Range(0, Moves.Count);
        return Moves[r];
    }
}
