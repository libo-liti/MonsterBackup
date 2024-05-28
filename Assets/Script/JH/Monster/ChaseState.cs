using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IMonsterState
{
    public ChaseState(GameObject monster) : base(monster) { }
    public override void StateEnter()
    {
        _monster.GetComponent<Animator>().SetBool("onChase", true);
    }
    public override void StateUpdate()
    {
        if (GameObject.Find("Player").transform.position.x - _monster.transform.position.x > 0)
        {
            _monster.transform.position += Vector3.right * Time.deltaTime * 6;
            _monster.transform.localScale = new Vector3(1, 1, 1);
        }
        if (GameObject.Find("Player").transform.position.x - _monster.transform.position.x < 0)
        {
            _monster.transform.position += Vector3.left * Time.deltaTime * 6;
            _monster.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public override void StateExit()
    {
        _monster.GetComponent<Animator>().SetBool("onChase", false);
    }
}
