
public class RpcComp: RpcCompCommon
{
    protected override void EnableRpcMethod()
    {
        EnableRpcMethodWithType(RpcConst.Client);
    }

    protected override void EnableCompRpcMethod(Component comp)
    {
        EnableCompRpcMethodWithType(comp, RpcConst.Client);
    }

    protected override void DisableCompRpcMethod(Component comp)
    {
        DisableCompRpcMethodWithType(comp, RpcConst.Client);
    }
}
