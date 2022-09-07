using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFootStep : MonoBehaviour
{
    public SoundList[] stepSoundLists;
    private int index;
    private Animator anim;
    private bool isLeftFootAhead;
    private bool playedLeftFoot;
    private bool playedRightFoot;
    private Vector3 leftFootIKPos;
    private Vector3 rightFootIKPos;
 

    private void Awake()
    {
        anim = GetComponent<Animator>();

    }
    private void OnAnimatorIK(int layerIndex)
    {
        leftFootIKPos = anim.GetIKPosition(AvatarIKGoal.LeftFoot);
        rightFootIKPos = anim.GetIKPosition(AvatarIKGoal.RightFoot);
    }
    void PlayFootStep()
    {
        int oldIndex = index;
        while(oldIndex == index)
        {
            index = Random.Range(0, stepSoundLists.Length);
        }
        SoundManager.Instance.PlayOndShotEffect((int)stepSoundLists[index], transform.position, 1f);

    }
    private void Update()
    {
        float factor = 0.115f;
        //사운드가 계속 생김

        if(anim.velocity.magnitude > 1.4f)
        {
            if(Vector3.Distance(leftFootIKPos,anim.pivotPosition) <= factor && playedLeftFoot == false)
            {
                Debug.Log(Vector3.Distance(leftFootIKPos, anim.pivotPosition));
                PlayFootStep();
                playedLeftFoot = true;
                playedRightFoot = false;

            }
            else if(Vector3.Distance(rightFootIKPos,anim.pivotPosition)<=factor && playedRightFoot == false)
            {
                Debug.Log(Vector3.Distance(rightFootIKPos, anim.pivotPosition));
                PlayFootStep();
                playedLeftFoot = false;
                playedRightFoot = true;
            }
        }
    }
}
