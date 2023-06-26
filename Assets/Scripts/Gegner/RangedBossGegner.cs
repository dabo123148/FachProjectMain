using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedBossGegner : RangedGegner
{
    public bool SpezialAttackAvalible = false;
    public float SpezialAttackCooldown = 30;
    private void Start()
    {
        StartCoroutine(SpezialAttackCooldownTimer());
    }
    public void ExecuteSpezialAttack()
    {
        StartCoroutine(SpezialAttackCooldownTimer());
        ReorderAttack();
    }
    public void ReorderAttack()
    {
        OrderAllGegnerToCorners();
        GoToMiddle();
    }
    public void GoToMiddle()
    {
        Order(new Vector2(0, 0));
    }
    public void OrderAllGegnerToCorners()
    {
        foreach(Gegner g in GameObject.FindObjectsOfType<Gegner>())
        {
            if (!g.gameObject.GetComponent<RangedBossGegner>())
            {
                float xpos = Random.Range(20f, 15f);
                float ypos = Random.Range(20f, 15f);
                if (Random.Range(0, 2) == 0) xpos *= -1;
                if (Random.Range(0, 2) == 0) ypos *= -1;
                g.Order(new Vector2(xpos, ypos));
            }
        }
    }
    IEnumerator SpezialAttackCooldownTimer()
    {
        SpezialAttackAvalible = false;
        yield return new WaitForSeconds(SpezialAttackCooldown);
        SpezialAttackAvalible = true;
    }
}
