using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildBoarChaseState : IMonsterState
{
    Vector3 playerPos;
    bool isRight;
    public WildBoarChaseState(GameObject monster) : base(monster) { }
    public override void StateEnter()
    {
        playerPos = GameObject.Find("Player").transform.position;
        if (playerPos.x - _monster.transform.position.x < 0)
            isRight = false;
        else
            isRight = true;
    }
    public override void StateUpdate()
    {
        if (isRight)
            _monster.transform.position += Vector3.right * Time.deltaTime * 20;
        else
            _monster.transform.position += Vector3.left * Time.deltaTime * 20;
    }
    public override void StateExit()
    {

    }
}
