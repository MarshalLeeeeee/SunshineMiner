using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Arg
{
    public int type;
    public object obj;
}
public class ArgInt : Arg
{
    public ArgInt(int i)
    {
        type = 1;
        obj = i;
    }
}
public class ArgFloat : Arg
{
    public ArgFloat(float f)
    {
        type = 2;
        obj = f;
    }
}
public class ArgString : Arg
{
    public ArgString(string s)
    {
        type = 3;
        obj = s;
    }
}

public class Msg
{
    public string srcId { get; }
    public string tgtId { get; }
    public string methodName { get; }
    public long expiredTime { get; }
    public Dictionary<string, Arg> args;
    public Msg(string srdId_, string tgtId_, string methodName_, long expiredTime_ = -1)
    {
        srcId = srdId_;
        tgtId = tgtId_;
        methodName = methodName_;
        expiredTime = expiredTime_;
        args = new Dictionary<string, Arg>();
    }
    public void AddArgInt(string name, int i)
    {
        args[name] = new ArgInt(i);
    }
    public void AddArgFloat(string name, float f)
    {
        args[name] = new ArgFloat(f);
    }
    public void AddArgString(string name, string s)
    {
        args[name] = new ArgString(s);
    }

}

public static class DataStreamer
{
    public static byte[] Serialize(Msg msg)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write(msg.srcId);
        writer.Write(msg.tgtId);
        writer.Write(msg.methodName);
        writer.Write(msg.expiredTime);
        writer.Write(msg.args.Count);
        foreach (var kvp in msg.args)
        {
            writer.Write(kvp.Key);
            writer.Write(kvp.Value.type);
            switch (kvp.Value.type)
            {
                case 1:
                    writer.Write((int)kvp.Value.obj);
                    break;
                case 2:
                    writer.Write((float)kvp.Value.obj);
                    break;
                case 3:
                    writer.Write((string)kvp.Value.obj);
                    break;
            }
            //writer.Write((byte)kvp.Value.obj);
        }
        return stream.ToArray();
    }

    public static Msg Deserialize(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);

        Msg msg = new Msg(reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadInt64());
        int argCnt = reader.ReadInt32();
        int i = 0;
        while (i < argCnt)
        {
            string name = reader.ReadString();
            int type = reader.ReadInt32();
            switch (type)
            {
                case 1:
                    msg.AddArgInt(name, reader.ReadInt32());
                    break;
                case 2:
                    msg.AddArgFloat(name, reader.ReadSingle());
                    break;
                case 3:
                    msg.AddArgString(name, reader.ReadString());
                    break;
            }
            i++;
        }
        return msg;
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
            return (false, new Msg("", "", ""));
        }

        int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
        if (messageLength <= 0)
        {
            return (false, new Msg("", "", ""));
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
            return (false, new Msg("", "", ""));
        }
        return (true, Deserialize(messageBuffer));
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
                Console.WriteLine("Head read failed");
                return (false, new Msg("", "", ""));
            }

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
            if (messageLength <= 0)
            {
                Console.WriteLine("Head length abnormal");
                return (false, new Msg("", "", ""));
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
                Console.WriteLine("Read data incomplete");
                return (false, new Msg("", "", ""));
            }
            return (true, Deserialize(messageBuffer));
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is IOException)
        {
            Console.WriteLine($"Read exception happens: {ex}");
            return (false, new Msg("", "", ""));
        }
    }

    public static bool WriteMsgToStream(NetworkStream stream, Msg msg)
    {
        byte[] buffer = DataStreamer.Serialize(msg);
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
            byte[] buffer = DataStreamer.Serialize(msg);
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
