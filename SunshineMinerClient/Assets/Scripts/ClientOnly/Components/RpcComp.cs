
public class RpcComp : RpcCompCommon
{
    protected override void EnableRpcMethod()
    {
        EnableRpcMethodWithType(RpcConst.Server);
    }

    protected override void EnableCompRpcMethod(Component comp)
    {
        EnableCompRpcMethodWithType(comp, RpcConst.Server);
    }

    protected override void DisableCompRpcMethod(Component comp)
    {
        DisableCompRpcMethodWithType(comp, RpcConst.Server);
    }
}
