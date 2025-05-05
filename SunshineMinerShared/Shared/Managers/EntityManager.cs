using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class EntityManager : Manager
{
    public override void Update()
    {
        Game.Instance?.eventManager.EventTest();
    }
}
