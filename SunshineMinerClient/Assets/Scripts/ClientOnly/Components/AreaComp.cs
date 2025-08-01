
[Comp]
public class AreaComp : AreaCompCommon
{
    public Vec3? prefabPosition
    {
        get
        {
            PrefabComp prefabComp = GetEntityComponent<PrefabComp>();
            if (prefabComp != null)
            {
                return prefabComp.prefabPosition;
            }
            else
            {
                return null;
            }
        }
    }

    protected override void DoUpdateSelf()
    {
        base.DoUpdateSelf();
        if (entity == null)
        {
            return;
        }
        Vec3? prefabPosition = this.prefabPosition;
        if (prefabPosition == null)
        {
            return;
        }
        if (Game.Instance.entityManager.primaryPid == entity.eid.GetValue())
        {
            Msg msg = new Msg(entity.eid.GetValue(), "SyncPositionRemote");
            msg.arg.Add(new PropFloatNode(prefabPosition.x));
            msg.arg.Add(new PropFloatNode(prefabPosition.y));
            msg.arg.Add(new PropFloatNode(prefabPosition.z));
            Game.Instance.gate.AppendSendMsg(msg);
        }
        else
        {
            PrefabComp prefabComp = GetEntityComponent<PrefabComp>();
            if (prefabComp != null)
            {
                prefabComp.UpdatePosition(areaPosition);
            }
        }
    }
}
