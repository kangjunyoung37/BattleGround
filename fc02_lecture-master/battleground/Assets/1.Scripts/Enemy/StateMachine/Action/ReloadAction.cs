using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PluggableAI/Actions/Reload")]
public class ReloadAction : Action
{
    public override void Act(StateController controller)
    {
        if(!controller.reloading && controller.bullets <= 0)
        {
            controller.enemyAnimation.anim.SetTrigger(KJY.AnimatorKey.Reload);
            controller.reloading = true;
            SoundManager.Instance.PlayOndShotEffect((int)SoundList.reloadWeapon, controller.enemyAnimation.gunMuzzle.position, 2f);
        }
    }
}
