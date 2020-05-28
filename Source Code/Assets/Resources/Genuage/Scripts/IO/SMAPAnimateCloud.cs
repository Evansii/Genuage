using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMAPAnimateCloud : MonoBehaviour
{
    public float indexkey = 0.0f;

    public float animationSpeed =2.5f;

    Animation anim;

    [System.NonSerialized]
    public Keyframe keysRotationW;
    public Keyframe keysRotationX;
    public Keyframe keysRotationY;
    public Keyframe keysRotationZ;

    [System.NonSerialized] 
    public AnimationClip clip;

    AnimationCurve curveRotationW;
    AnimationCurve curveRotationX;
    AnimationCurve curveRotationY;
    AnimationCurve curveRotationZ;



    public float[] initialRot;

    public List<float[]> SavedRot;

    void Start()    
    {
        anim = gameObject.AddComponent(typeof(Animation)) as Animation;

        clip = new AnimationClip();
        clip.legacy =true;

        keysRotationW = new Keyframe();
        keysRotationX = new Keyframe();
        keysRotationY = new Keyframe();
        keysRotationZ = new Keyframe();
        
        //InitializeAnimation();

        curveRotationW = new AnimationCurve(keysRotationW);
        curveRotationX = new AnimationCurve(keysRotationX);
        curveRotationY = new AnimationCurve(keysRotationY);
        curveRotationZ = new AnimationCurve(keysRotationZ);

        //UpdateAnimation();

        //initialRot = new float[4]; 
        
    }

    public void InitializeAnimation()
    {
        keysRotationW.value = transform.localRotation.w;
        keysRotationW.time = 0f;
        keysRotationX.value = transform.localRotation.x;
        keysRotationX.time = 0f;
        keysRotationY.value = transform.localRotation.y;
        keysRotationY.time = 0f;
        keysRotationZ.value = transform.localRotation.w;
        keysRotationZ.time = 0f;   
    }

    
    public void AddKeyframe()
    {
    
        curveRotationW.AddKey(animationSpeed*indexkey, transform.localRotation.w);
        curveRotationX.AddKey(animationSpeed*indexkey, transform.localRotation.x);
        curveRotationY.AddKey(animationSpeed*indexkey, transform.localRotation.y);
        curveRotationZ.AddKey(animationSpeed*indexkey, transform.localRotation.z);

        UpdateAnimation(); 

        indexkey ++;

    }

    public void UpdateAnimation()
    {
        clip.SetCurve("",typeof(Transform),"localRotation.w",curveRotationW);
        clip.SetCurve("",typeof(Transform),"localRotation.x",curveRotationX);
        clip.SetCurve("",typeof(Transform),"localRotation.y",curveRotationY);
        clip.SetCurve("",typeof(Transform),"localRotation.z",curveRotationZ);

        anim.AddClip(clip,clip.name);
        anim.clip = clip;   

    }


    public void PlayAnimation()
    {  
        if(!anim.isPlaying)
        {
            Debug.Log("Animation playing");
            anim.Play();

        }
        else
        {
            Debug.Log("Animation stop");
            anim.Stop();
        }
    }
}
