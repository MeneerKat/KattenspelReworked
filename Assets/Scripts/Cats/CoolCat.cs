
using System.Collections.Generic;
using UnityEngine;

public class CoolCat : CatClass
{
    AudioClip healSound;

    public CoolCat(
        GameObject CatPrefab,
        List<Sprite> CatSprites,
        float HorizontalSpeed,
        float VerticalSpeed,
        float RotationSpeed,
        AudioClip PurrSound,
        AudioClip BoingSound,
        AudioClip CrashSound,

        AudioClip HealSound
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
        healSound = HealSound;
    }

    // Override hit function
    public override void Hit()
    {
        GameManager.Instance.HealHealth(GameManager.Instance.coolHealing);
        AudioSource.PlayClipAtPoint(healSound, catObject.transform.position);
    }
}
