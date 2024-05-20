using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagedState : IMonsterState
{
    public DamagedState(GameObject monster) : base(monster) { }
    public override void StateEnter()
    {
        _monster.GetComponent<Animator>().SetBool("onDamaged", true);
    }
    public override void StateUpdate()
    {

    }
    public override void StateExit()
    {
        _monster.GetComponent<Animator>().SetBool("onDamaged", false);
    }
}
