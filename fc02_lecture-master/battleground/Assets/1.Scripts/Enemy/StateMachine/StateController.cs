using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// state -> actions update -> transition(decision) check..
/// state에 필요한 기능들. 애니메이션 콜백들
/// 시야 체크, 찾아논 엄폐물 장소중 가장 가까운 위치를 찾는 기능
/// </summary>
public class StateController : MonoBehaviour
{
    public GeneralStates generalStates;
    public ClassStats statData;
    public string classID; //PISTOL, RIFLE ,AK,

    public ClassStats.Param classStats
    {
        get
        {
            foreach(ClassStats.Sheet sheet in statData.sheets)
            {
                foreach (ClassStats.Param param in sheet.list)
                {
                    if(param.ID.Equals(this.classID))
                    {
                        return param;
                    }
                }
            }
            return null;
        }
    }
    public State currentState;
    public State remainState;

    public Transform aimTarget;
    public List<Transform> patrolWaypoints;

    public int bullets;
    [Range(0,50)]
    public float viewRadius;//시야 반경
    [Range(0,360)]
    public float viewAngle;
    [Range(0,25)]
    public float perceptionRadius;

    [HideInInspector] public float nearRadius;
    [HideInInspector] public NavMeshAgent nav;
    [HideInInspector] public int wayPointIndex;
    [HideInInspector] public int maxiumBurst = 7;//유효한 총알 개수
    [HideInInspector] public float blindEngageTime = 30f;//플레이어가 사라졌을 때 플레이어를 찾는 시간
    [HideInInspector] public bool targetInSight;//타켓이 시야안에 있냐
    [HideInInspector] public bool focusSight; //시야를 포커스할것인가
    [HideInInspector] public bool reloading;//재장전중인가
    [HideInInspector] public bool hadClearShot; //before
    [HideInInspector] public bool haveClearShot; // now
    [HideInInspector] public int coverHash = -1;
    [HideInInspector] public EnemyVariables variables;
    [HideInInspector] public Vector3 personalTarget = Vector3.zero;

    private int magBullets;
    private bool aiActive;
    private static Dictionary<int, Vector3> coverSpot;//static
    private bool strafing;
    private bool aiming;
    private bool checkedOnLoop, blockedSight;

    [HideInInspector] public EnemyAnimation enemyAnimation;
    [HideInInspector] public CoverLookUp coverLookUp;

    public Vector3 CoverSpot
    {
        get { return coverSpot[this.GetHashCode()]; }
        set { coverSpot[this.GetHashCode()] = value; }
    }
    public void TransitionToState(State nextState,Decision decision)
    {
        if(nextState != remainState)
        {
            currentState = nextState;
        }
    }

    public bool Strafing
    {
        get => strafing;
        set
        {
            enemyAnimation.anim.SetBool("Strafe", value);
            strafing = value;
        }
    }
    public bool Aiming
    {
        get => aiming;
        set
        {
            if(aiming != value)
            {
                enemyAnimation.anim.SetBool("Aim", value);
                aiming = value;
            }

        }
    }

    public IEnumerator UnstuckAim(float delay)
    {
        yield return new WaitForSeconds(delay * 0.5f);
        Aiming = false;
        yield return new WaitForSeconds(delay * 0.5f);
        Aiming = true;
    }

    

}

