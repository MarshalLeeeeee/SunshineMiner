using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;


public class Msg
{
    public string tgtId { get; }
    public string methodName { get; }
    public SyncDataListNode arg = new SyncDataListNode();
    public Msg(string tgtId_, string methodName_)
    {
        tgtId = tgtId_;
        methodName = methodName_;
    }
}


public static class MsgStreamer
{
    public static byte[] Serialize(Msg msg)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write(msg.tgtId);
        writer.Write(msg.methodName);
        msg.arg.Serialize(writer);
        return stream.ToArray();
    }

    public static Msg? Deserialize(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);
        
        try
        {
            Msg msg = new Msg(reader.ReadString(), reader.ReadString());
            SyncDataNode arg = SyncStreamer.Deserialize(reader);
            if (arg != null && arg is SyncDataListNode listNode)
            {
                msg.arg = listNode;
                return msg;
            }
            return null;
        }
        catch
        {
            return null;
        }

    }

    public static (bool succ, Msg msg) ReadMsgFromStream(NetworkStream stream)
    {
        byte[] lengthBuffer = new byte[4];
        int lengthBytesRead = 0;
        while (lengthBytesRead < 4)
        {
            int read = stream.Read(lengthBuffer, lengthBytesRead, 4 - lengthBytesRead);
            if (read == 0) break;
            lengthBytesRead += read;
        }
        if (lengthBytesRead < 4)
        {
            return (false, new Msg("", ""));
        }

        int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
        if (messageLength <= 0)
        {
            return (false, new Msg("", ""));
        }
        byte[] messageBuffer = new byte[messageLength];
        int totalBytesRead = 0;
        while (totalBytesRead < messageLength)
        {
            int read = stream.Read(messageBuffer, totalBytesRead, messageLength - totalBytesRead);
            if (read == 0) break;
            totalBytesRead += read;
        }
        if (totalBytesRead < messageLength)
        {
            return (false, new Msg("", ""));
        }
        Msg? msg = Deserialize(messageBuffer);
        if (msg == null)
        {
            return (false, new Msg("", ""));
        }
        return (true, msg);
    }

    public static async Task<(bool succ, Msg msg)> ReadMsgFromStreamAsync(NetworkStream stream, CancellationToken cancellationToken = default)
    {
        try
        {
            byte[] lengthBuffer = new byte[4];
            int lengthBytesRead = 0;
            while (lengthBytesRead < 4)
            {
                int read = await stream.ReadAsync(
                    lengthBuffer,
                    lengthBytesRead,
                    4 - lengthBytesRead,
                    cancellationToken
                ).ConfigureAwait(false);

                if (read == 0) break;
                lengthBytesRead += read;
            }
            if (lengthBytesRead < 4)
            {
                Debugger.Log("Head read failed");
                return (false, new Msg("", ""));
            }

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            if (messageLength <= 0)
            {
                Debugger.Log("Head length abnormal");
                return (false, new Msg("", ""));
            }
            byte[] messageBuffer = new byte[messageLength];
            int totalBytesRead = 0;
            while (totalBytesRead < messageLength)
            {
                int read = await stream.ReadAsync(
                    messageBuffer,
                    totalBytesRead,
                    messageLength - totalBytesRead,
                    cancellationToken
                ).ConfigureAwait(false);

                if (read == 0) break;
                totalBytesRead += read;
            }
            if (totalBytesRead < messageLength)
            {
                Debugger.Log("Read data incomplete");
                return (false, new Msg("", ""));
            }
            //Debugger.Log($"Read data complete: messageLength: {messageLength} totalBytesRead:{totalBytesRead}");
            Msg? msg = Deserialize(messageBuffer);
            if (msg == null)
            {
                Debugger.Log("Read msg failed");
                return (false, new Msg("", ""));
            }
            //Debugger.Log($"Read msg method name: {msg.methodName}");
            return (true, msg);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is IOException)
        {
            Debugger.Log($"Read exception happens: {ex}");
            return (false, new Msg("", ""));
        }
    }

    public static bool WriteMsgToStream(NetworkStream stream, Msg msg)
    {
        byte[] buffer = MsgStreamer.Serialize(msg);
        if (buffer.Length <= 0) return false;

        byte[] lengthPrefix = BitConverter.GetBytes(buffer.Length);
        stream.Write(lengthPrefix, 0, 4);
        stream.Write(buffer, 0, buffer.Length);
        stream.Flush();
        return true;
    }

    public static async Task<bool> WriteMsgToStreamAsync(NetworkStream stream, Msg msg, CancellationToken cancellationToken = default)
    {
        try
        {
            byte[] buffer = MsgStreamer.Serialize(msg);
            if (buffer.Length <= 0) return false;

            byte[] lengthPrefix = BitConverter.GetBytes(buffer.Length);
            await stream.WriteAsync(lengthPrefix, 0, 4, cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is IOException)
        {
            return false;
        }
    }
}
