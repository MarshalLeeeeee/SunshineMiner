using System;

public class Manager : Entity
{
    public Manager()
    {
        Type type = GetType();
        eid = new PropStringNode(type.Name);
    }
}
