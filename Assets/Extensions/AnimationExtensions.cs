using UnityEngine;
using System.Collections;

public static class AnimationExtensions
{
    public static void PlayBackwards (this Animation anim)
    {
        var name = anim.clip.name + "Reversed";
        if (anim[name] == null)
        {
            anim.AddClip(anim.clip, name);
            anim[name].speed = -1;
        }
        anim[name].time = anim.clip.length;;
        anim.Play(name);
    }
}
