using System.Collections;
using System.Collections.Generic;

public class ComponentFactory: ComponentFactoryBase
{
    static public Component? CreateComponent(string compName)
    {
        switch (compName)
        {
            case "PrefabComp":
                return new PrefabComp();
            default:
                return ComponentFactoryBase.CreateComponent(compName);
        }
    }
}