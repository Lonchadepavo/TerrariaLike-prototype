﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeNameBehaviour : StateMachineBehaviour {
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("show_biome_name", false);
        
    }
}
