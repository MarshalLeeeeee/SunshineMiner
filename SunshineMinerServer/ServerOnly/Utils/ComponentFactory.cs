using System.Collections;
using System.Collections.Generic;

public class ComponentFactory : ComponentFactoryBase
{
    static public Component? CreateComponent(string compName)
    {
        switch (compName)
        {
            default:
                return ComponentFactoryBase.CreateComponent(compName);
        }
    }
}