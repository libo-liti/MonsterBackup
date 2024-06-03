using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GoblinChaseState : IMonsterState
{
    RaycastHit2D hitRight;
    RaycastHit2D hitLeft;
    public GoblinChaseState(GameObject monster) : base(monster) { }
    public override void StateEnter()
    {

    }
    public override void StateUpdate()
    {
        hitRight = Physics2D.Raycast(_monster.transform.position, Vector2.right, 2f, 1 << LayerMask.NameToLayer("Goblin"));
        hitLeft = Physics2D.Raycast(_monster.transform.position, Vector2.left, 2f, 1 << LayerMask.NameToLayer("Goblin"));

        if (GameObject.Find("Player").transform.position.x - _monster.transform.position.x > 0)
        {
            _monster.transform.position += Vector3.right * Time.deltaTime * 4f;
            _monster.transform.localScale = new Vector3(1, 1, 1);
        }
        if (GameObject.Find("Player").transform.position.x - _monster.transform.position.x < 0)
        {
            _monster.transform.position += Vector3.left * Time.deltaTime * 4f;
            _monster.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    public override void StateExit()
    {

    }
}
