using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaComp : AreaCompBase
{
    public Vec3? prefabPosition
    {
        get
        {
            PrefabComp prefabComp = GetComponent<PrefabComp>("PrefabComp");
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
        Entity entity = this.entity;
        if (entity == null)
        {
            return;
        }
        if (Game.Instance.entityManager.primaryPid != entity.eid.Getter())
        {
            return;
        }
        Vec3? prefabPosition = this.prefabPosition;
        if (prefabPosition == null)
        {
            return;
        }
        Msg msg = new Msg(entity.eid.Getter(), "SyncPositionRemote");
        CustomList arg = new CustomList();
        arg.Add(new CustomFloat(prefabPosition.x));
        arg.Add(new CustomFloat(prefabPosition.y));
        arg.Add(new CustomFloat(prefabPosition.z));
        msg.arg = arg;
        Game.Instance.gate.AppendSendMsg(msg);
    }
}
