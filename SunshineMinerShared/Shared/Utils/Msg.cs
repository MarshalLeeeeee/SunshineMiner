using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


public class Msg
{
    public string srcId { get; }
    public string tgtId { get; }
    public string methodName { get; }
    public long expiredTime { get; }
    public CustomType arg;
    public Msg(string srdId_, string tgtId_, string methodName_, long expiredTime_ = -1)
    {
        srcId = srdId_;
        tgtId = tgtId_;
        methodName = methodName_;
        expiredTime = expiredTime_;
        arg = new CustomType();
    }
}


public static class MsgStreamer
{
    public static byte[] Serialize(Msg msg)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);

        writer.Write(msg.srcId);
        writer.Write(msg.tgtId);
        writer.Write(msg.methodName);
        writer.Write(msg.expiredTime);
        msg.arg.Serialize(writer);
        return stream.ToArray();
    }

    public static Msg Deserialize(byte[] data)
    {
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);

        Msg msg = new Msg(reader.ReadString(), reader.ReadString(), reader.ReadString(), reader.ReadInt64());
        CustomType arg = CustomTypeStreamer.Deserialize(reader);
        msg.arg = arg;
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
