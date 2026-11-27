
using System.Collections.Generic;
using UnityEngine;

public class EvilCat : CatClass
{
    AudioClip hissSound;
    AudioClip evilMeowSound;

    public EvilCat(
        GameObject CatPrefab,
        List<Sprite> CatSprites,
        float HorizontalSpeed,
        float VerticalSpeed,
        float RotationSpeed,
        AudioClip PurrSound,
        AudioClip BoingSound,
        AudioClip CrashSound,
        AudioClip HissSound,
        AudioClip EvilMeowSound
    )
    : base(
        CatPrefab,
        CatSprites,
        HorizontalSpeed,
        VerticalSpeed,
        RotationSpeed,
        PurrSound,
        BoingSound,
        CrashSound
    )
    {
        hissSound = HissSound;
        evilMeowSound = EvilMeowSound;
    }

    // Override hit function
    public override void Hit()
    {
        GameManager.Instance.RemoveHealth(1);
        AudioSource.PlayClipAtPoint(hissSound, catObject.transform.position);
    }

    public override void Miss()
    {
        AudioSource.PlayClipAtPoint(evilMeowSound, catObject.transform.position);
    }
}
