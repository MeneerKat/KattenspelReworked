using UnityEngine;

public class CatCollision : MonoBehaviour, ICanHit
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
}
