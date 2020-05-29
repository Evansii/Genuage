using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMAPAnimateCloud : MonoBehaviour
{
    public float indexkey = 0.0f;

    public float keyframeTimestep = 5.0f;

    public float timestep = 5f;

    public float animationTime;

    Animation anim;

    public Keyframe keyRotationW = new Keyframe();
    public Keyframe keyRotationX = new Keyframe();
    public Keyframe keyRotationY = new Keyframe();
    public Keyframe keyRotationZ = new Keyframe();

    public Keyframe keyScaleX = new Keyframe();
    public Keyframe keyScaleY = new Keyframe();
    public Keyframe keyScaleZ = new Keyframe();

    public Keyframe keyPositionX = new Keyframe();
    public Keyframe keyPositionY = new Keyframe();
    public Keyframe keyPositionZ = new Keyframe();

    public AnimationClip clip;

    AnimationCurve curveRotationW;
    AnimationCurve curveRotationX;
    AnimationCurve curveRotationY;
    AnimationCurve curveRotationZ;

    AnimationCurve curveScaleX;
    AnimationCurve curveScaleY;
    AnimationCurve curveScaleZ;

    AnimationCurve curvePositionX;
    AnimationCurve curvePositionY;
    AnimationCurve curvePositionZ;

    void Start()    
    {
        anim = gameObject.AddComponent(typeof(Animation)) as Animation;

        clip = new AnimationClip();
        clip.legacy = true;

        curveRotationW = new AnimationCurve(keyRotationW);
        curveRotationX = new AnimationCurve(keyRotationX);
        curveRotationY = new AnimationCurve(keyRotationY);
        curveRotationZ = new AnimationCurve(keyRotationZ);

        curveScaleX = new AnimationCurve(keyScaleX);
        curveScaleY = new AnimationCurve(keyScaleY);
        curveScaleZ = new AnimationCurve(keyScaleZ);

        curvePositionX = new AnimationCurve(keyPositionX);
        curvePositionY = new AnimationCurve(keyPositionY);
        curvePositionZ = new AnimationCurve(keyPositionZ);
        
        //UpdateAnimation(); 
        
    }

    // public void InitializeAnimation()
    // {
    //     keysRotationW.value = transform.localRotation.w;
    //     keysRotationW.time = 0f;
    //     keysRotationX.value = transform.localRotation.x;
    //     keysRotationX.time = 0f;
    //     keysRotationY.value = transform.localRotation.y;
    //     keysRotationY.time = 0f;
    //     keysRotationZ.value = transform.localRotation.w;
    //     keysRotationZ.time = 0f;   



    // }

    
    public void AddKeyframe()
    {

        indexkey ++;
        
        animationTime = keyframeTimestep*indexkey;
    
        
        curveRotationW.AddKey(animationTime, transform.localRotation.w);
        curveRotationX.AddKey(animationTime, transform.localRotation.x);
        curveRotationY.AddKey(animationTime, transform.localRotation.y);
        curveRotationZ.AddKey(animationTime, transform.localRotation.z);
 
        // curveScaleX.AddKey(animationTime, transform.localScale.x);
        // curveScaleY.AddKey(animationTime, transform.localScale.y);
        // curveScaleZ.AddKey(animationTime, transform.localScale.z);

        curvePositionX.AddKey(animationTime, transform.localPosition.x);
        curvePositionY.AddKey(animationTime, transform.localPosition.y);
        curvePositionZ.AddKey(animationTime, transform.localPosition.z);

        Debug.Log(transform.localScale.x);

        UpdateAnimation(); 
    
    }

    public void UpdateAnimation()
    {
        clip.SetCurve("",typeof(Transform),"localRotation.w",curveRotationW);
        clip.SetCurve("",typeof(Transform),"localRotation.x",curveRotationX);
        clip.SetCurve("",typeof(Transform),"localRotation.y",curveRotationY);
        clip.SetCurve("",typeof(Transform),"localRotation.z",curveRotationZ);

        // clip.SetCurve("",typeof(Transform),"localScale.x",curveScaleX);
        // clip.SetCurve("",typeof(Transform),"localScale.y",curveScaleY);
        // clip.SetCurve("",typeof(Transform),"localScale.z",curveScaleZ);

        clip.SetCurve("",typeof(Transform),"localPosition.x",curvePositionX);
        clip.SetCurve("",typeof(Transform),"localPosition.y",curvePositionY);
        clip.SetCurve("",typeof(Transform),"localPosition.z",curvePositionZ);

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

    public void SetAnimationSpeed(float animSpeed)
    {
        keyframeTimestep = timestep / animSpeed;
        Debug.Log("Timestep is "+ keyframeTimestep);
    }

}
