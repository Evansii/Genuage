/**
Vincent Casamayou
RIES GROUP
SMAP Animation System
29/05/2020
**/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

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

        //Fix for Scaling beginning to a 0 instead of 1
        curveScaleX.MoveKey(0, new Keyframe(0,1));
        curveScaleY.MoveKey(0, new Keyframe(0,1));
        curveScaleZ.MoveKey(0, new Keyframe(0,1));
        UpdateAnimation(); 

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

        curveScaleX.AddKey(animationTime, transform.localScale.x);
        curveScaleY.AddKey(animationTime, transform.localScale.y);
        curveScaleZ.AddKey(animationTime, transform.localScale.z);

        curvePositionX.AddKey(animationTime, transform.localPosition.x);
        curvePositionY.AddKey(animationTime, transform.localPosition.y);
        curvePositionZ.AddKey(animationTime, transform.localPosition.z);


        UpdateAnimation(); 
    
    }

    public void AddAnimationEvent(string eventName, string ColorMapName = "autumn")
    {
        AnimationEvent evt = new AnimationEvent();
        evt.time = animationTime;

        switch(eventName)
        {
            case "ColorMap":
                evt.stringParameter = ColorMapName;
                evt.functionName ="UpdateColorMap";
                break;

            default:
                Debug.Log("Wrong Event called");
                break;

        }

        clip.AddEvent(evt);

    }

    public void UpdateColorMap(string ColorMapName)
    {
        CloudUpdater.instance.ChangeCurrentColorMap(ColorMapName);
    }


    public void UpdateKeyframe(int index)
    {
        animationTime = keyframeTimestep * (float)index;

        Keyframe TMPkeyRotationW = new Keyframe(animationTime, transform.localRotation.w);
        curveRotationW.MoveKey(index, TMPkeyRotationW);
        Keyframe TMPkeyRotationX = new Keyframe(animationTime, transform.localRotation.x);
        curveRotationX.MoveKey(index, TMPkeyRotationX);
        Keyframe TMPkeyRotationY = new Keyframe(animationTime, transform.localRotation.y);
        curveRotationY.MoveKey(index, TMPkeyRotationY);
        Keyframe TMPkeyRotationZ = new Keyframe(animationTime, transform.localRotation.z);
        curveRotationZ.MoveKey(index, TMPkeyRotationZ);

        Keyframe TMPkeyScaleX = new Keyframe(animationTime,transform.localScale.x);
        curveScaleX.MoveKey(index, TMPkeyScaleX);
        Keyframe TMPkeyScaleY = new Keyframe(animationTime,transform.localScale.y);
        curveScaleY.MoveKey(index, TMPkeyScaleY);
        Keyframe TMPkeyScaleZ = new Keyframe(animationTime,transform.localScale.z);
        curveScaleZ.MoveKey(index, TMPkeyScaleZ);

        Keyframe TMPkeyPositionX = new Keyframe(animationTime, transform.localPosition.x);
        curvePositionX.MoveKey(index, TMPkeyPositionX);
        Keyframe TMPkeyPositionY = new Keyframe(animationTime, transform.localPosition.y);
        curvePositionY.MoveKey(index, TMPkeyPositionY);
        Keyframe TMPkeyPositionZ = new Keyframe(animationTime, transform.localPosition.z);
        curvePositionZ.MoveKey(index, TMPkeyPositionZ);

        UpdateAnimation();

    }



    public void UpdateAnimation()
    {
        clip.SetCurve("",typeof(Transform),"localRotation.w",curveRotationW);
        clip.SetCurve("",typeof(Transform),"localRotation.x",curveRotationX);
        clip.SetCurve("",typeof(Transform),"localRotation.y",curveRotationY);
        clip.SetCurve("",typeof(Transform),"localRotation.z",curveRotationZ);

        clip.SetCurve("",typeof(Transform),"localScale.x",curveScaleX);
        clip.SetCurve("",typeof(Transform),"localScale.y",curveScaleY);
        clip.SetCurve("",typeof(Transform),"localScale.z",curveScaleZ);

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

    public void RemoveAnimation()
    {
        for(int i = 0; i < curvePositionX.length ; i++)
        {
            curveRotationW.RemoveKey(i);
            curveRotationX.RemoveKey(i);
            curveRotationY.RemoveKey(i);
            curveRotationZ.RemoveKey(i);
            
            curveScaleX.RemoveKey(i);
            curveScaleY.RemoveKey(i);
            curveScaleZ.RemoveKey(i);
            
            curvePositionX.RemoveKey(i);
            curvePositionY.RemoveKey(i);
            curvePositionZ.RemoveKey(i);
        }

        
        indexkey = 0;

        UpdateAnimation();

        anim.RemoveClip(clip);
    }


}
