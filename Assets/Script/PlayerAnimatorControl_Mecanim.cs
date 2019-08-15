using UnityEngine;
using System.Collections;

public class PlayerAnimatorControl_Mecanim : MonoBehaviour
{
    private Animator animator;
	void Start () {
//         player = GameObject.FindWithTag("Player");
// 
//         if (player != null)
//             Debug.Log("Found Player");

        animator = GetComponent<Animator>();

        if (!animator)
        {
            Debug.Log("Not Found Animator Component");
        }
            
	}

    void UserAnimatorControl(int playerstata)
    {
        if (playerstata == 0)
        {
            animator.SetBool("Idle", true);
        }
        else
        {
            animator.SetBool("Idle", false);
        }

        if (playerstata > 0 && playerstata <10)
        {
            //Debug.Log("准备进入移动状态");
            animator.SetBool("Run", true);
        }
        else
        {
           // Debug.Log("准备退出移动状态");
            animator.SetBool("Run", false);
        }

        if (playerstata == 10)
        {
            //Debug.Log("准备进入攻击状态");
            animator.SetBool("Attack1", true);
        }
        else
        {
            //Debug.Log("准备退出攻击状态");
            animator.SetBool("Attack1", false);
        }

        if (playerstata == 11)
        {
            //Debug.Log("准备进入攻击状态");
            animator.SetBool("Attack2", true);
        }
        else
        {
            //Debug.Log("准备退出攻击状态");
            animator.SetBool("Attack2", false);
        }
    }


}
