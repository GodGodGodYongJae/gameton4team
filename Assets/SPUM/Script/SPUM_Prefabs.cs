using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
public class SPUM_Prefabs : MonoBehaviour
{
    public float _version;
    public SPUM_SpriteList _spriteOBj;
    public bool EditChk;
    public string _code;
    public Animator _anim;
    public bool _horse;
    public bool isRideHorse{
        get => _horse;
        set {
            _horse = value;
            UnitTypeChanged?.Invoke();
        }
    }
    public string _horseString;

    public UnityEvent UnitTypeChanged = new UnityEvent();
    private AnimationClip[] _animationClips;
    public AnimationClip[] AnimationClips => _animationClips;
    private Dictionary<string, int> _nameToHashPair = new Dictionary<string, int>();
    private Dictionary<string, AnimationClip> _nameToClipPair = new Dictionary<string, AnimationClip>();
    private void InitAnimPair(){
        _nameToHashPair.Clear();
        _animationClips = _anim.runtimeAnimatorController.animationClips;
        foreach (var clip in _animationClips)
        {
            int hash = Animator.StringToHash(clip.name);
            _nameToHashPair.Add(clip.name, hash);
            _nameToClipPair.Add(clip.name, clip);
        }
    }
    private void Awake() {
        InitAnimPair();
        _spriteOBj.ResyncData();
    }
    private void Start() {
        UnitTypeChanged.AddListener(InitAnimPair);
    }
    // 이름으로 애니메이션 실행
    public void PlayAnimation(string name, Action callback = null){

       // Debug.Log("CODE RUN : "+name);
        
        foreach (var animationName in _nameToHashPair)
        {
            if(animationName.Key.ToLower().Contains(name.ToLower()) ){
                _anim.Play(animationName.Value, 0);
                callback?.Invoke();
                break;
            }
        }

    }

    public int GetAnimFrmae(string name)
    {
        foreach (var animationName in _nameToClipPair)
        {
            if (animationName.Key.ToLower().Contains(name.ToLower()))
            {
                AnimatorClipInfo[] animationClip = _anim.GetCurrentAnimatorClipInfo(0);
                int currentFrame = (int)(animationClip[0].weight*(animationName.Value.length * animationName.Value.frameRate));
                return currentFrame;
            }
        }
        return 0;
    }

    
}
