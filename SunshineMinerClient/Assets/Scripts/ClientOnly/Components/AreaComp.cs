
public class AreaComp : AreaCompCommon
{
    public Vec3? prefabPosition
    {
        get
        {
            PrefabComp prefabComp = GetComponent<PrefabComp>();
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

    public override void Update()
    {
        base.Update();
        if (entity == null)
        {
            return;
        }
        Vec3? prefabPosition = this.prefabPosition;
        if (prefabPosition == null)
        {
            return;
        }
        if (Game.Instance.entityManager.primaryPid == entity.eid.Getter())
        {
            Msg msg = new Msg(entity.eid.Getter(), "SyncPositionRemote");
            msg.arg.Add(new CustomFloat(prefabPosition.x));
            msg.arg.Add(new CustomFloat(prefabPosition.y));
            msg.arg.Add(new CustomFloat(prefabPosition.z));
            Game.Instance.gate.AppendSendMsg(msg);
        }
        else
        {
            PrefabComp prefabComp = GetComponent<PrefabComp>();
            if (prefabComp != null)
            {
                prefabComp.UpdatePosition(areaPosition);
            }
        }
    }
}
