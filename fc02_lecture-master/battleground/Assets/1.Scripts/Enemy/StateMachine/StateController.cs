using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// state -> actions update -> transition(decision) check..
/// state�� �ʿ��� ��ɵ�. �ִϸ��̼� �ݹ��
/// �þ� üũ, ã�Ƴ� ���� ����� ���� ����� ��ġ�� ã�� ���
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
    public float viewRadius;//�þ� �ݰ�
    [Range(0,360)]
    public float viewAngle;
    [Range(0,25)]
    public float perceptionRadius;

    [HideInInspector] public float nearRadius;
    [HideInInspector] public NavMeshAgent nav;
    [HideInInspector] public int wayPointIndex;
    [HideInInspector] public int maxiumBurst = 7;//��ȿ�� �Ѿ� ����
    [HideInInspector] public float blindEngageTime = 30f;//�÷��̾ ������� �� �÷��̾ ã�� �ð�
    [HideInInspector] public bool targetInSight;//Ÿ���� �þ߾ȿ� �ֳ�
    [HideInInspector] public bool focusSight; //�þ߸� ��Ŀ���Ұ��ΰ�
    [HideInInspector] public bool reloading;//���������ΰ�
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

