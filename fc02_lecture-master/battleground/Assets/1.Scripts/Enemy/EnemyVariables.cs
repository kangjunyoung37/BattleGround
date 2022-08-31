using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public class EnemyVariables 
{
    // 사격을 위해 결정할 멤버변수
    //커버를 위해서 결정할 멤버변수
    //반복을 위해서 결정할 멤버변수
    //패트롤을 위해서 결정할 멤버변수
    //공격을 위해서 결정할 멤버변수
    public bool feelAlert;
    public bool healAlert;
    public bool advanceCoverDecision;
    public int waitRounds;
    public bool repeatShot;
    public float waitInCoverTime;
    public float coverTime;
    public float patrolTimer;
    public float shotTimer;
    public float startShootTimer;
    public float currentShots;
    public float shotsInRounds;
    public float blindEngageTimer;
    

    
}
