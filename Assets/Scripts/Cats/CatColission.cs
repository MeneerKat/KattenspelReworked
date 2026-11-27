using UnityEngine;

public class CatCollision : MonoBehaviour, ICanHit, INukable
{
    private CatClass catClass;

    public void SetCatClass(CatClass cat)
    {
        catClass = cat;
    }

    public void OnHit()
    {
        if (catClass != null)
        {
            catClass.OnHit();
        }
    }

    public void NukeEffect()
    {
        if (catClass != null)
        {
            catClass.NukeEffect();
        }
    }
}
