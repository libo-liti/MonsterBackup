using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IMonsterState
{
    public AttackState(GameObject monster) : base(monster) { }
    public override void StateEnter()
    {
        //_monster.GetComponent<Animator>().SetBool("onAttack", true);
    }
    public override void StateUpdate()
    {

    }
    public override void StateExit()
    {
        //_monster.GetComponent<Animator>().SetBool("onAttack", false);
    }
}
