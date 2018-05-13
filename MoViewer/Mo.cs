using System;
using System.IO;
using System.Linq;
using System.Text;

namespace MoViewer
{
    class Mo
    {

        public static Msg[] Parse(string path)
        {
            if (!File.Exists(path))
                throw new Exception("Can't find file " + path);

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                var br = new BinaryReader(stream);
                var magic = br.ReadUInt32();
                if (!(magic == 0x950412de || magic == 0x12de9504)) throw new Exception("Not mo file!");
                var revision = br.ReadInt32();
                Int32 N = br.ReadInt32(), O = br.ReadInt32(), T = br.ReadInt32();
                // S = br.ReadInt32(), H = br.ReadInt32()

                int head = -1;
                var msgs = new Msg[N];
                Encoding encoding = null;

                stream.Seek(O, SeekOrigin.Begin);
                Tuple<int, int>[] oData = new Tuple<int, int>[N];
                for (int i = 0; i < N; i++)
                {
                    oData[i] = new Tuple<int, int>(br.ReadInt32(), br.ReadInt32());
                    if (oData[i].Item1 == 0) head = i;
                }

                stream.Seek(T, SeekOrigin.Begin);
                Tuple<int, int>[] tData = new Tuple<int, int>[N];
                for (int i = 0; i < N; i++)
                    tData[i] = new Tuple<int, int>(br.ReadInt32(), br.ReadInt32());

                if (head != -1)
                {
                    stream.Seek(tData[head].Item2, SeekOrigin.Begin);
                    var msg = Encoding.ASCII.GetString(br.ReadBytes(tData[head].Item1)).Split(new[] { '\n' });
                    var charset = msg.Where(s => s.StartsWith("Content-Type")).First().Split(new[] { '=' }, 2)[1].Trim();
                    encoding = Encoding.GetEncoding(charset);
                }
                else
                    throw new Exception("Can't find header!");

                for (int i = 0; i < N; i++)
                {
                    var msg = new Msg() { Offset = i };
                    msgs[i] = msg;

                    stream.Seek(oData[i].Item2, SeekOrigin.Begin);
                    var ori = encoding.GetString(br.ReadBytes(oData[i].Item1));
                    stream.Seek(tData[i].Item2, SeekOrigin.Begin);
                    var tra = encoding.GetString(br.ReadBytes(tData[i].Item1));

                    if (ori.Contains('\0'))
                    {
                        var _ori = ori.Split(new[] { '\0' });
                        msg.Id = _ori[0];
                        msg._msgid_plural = _ori[1];
                        msg._plural = true;
                        msg._msgstr = tra.Split(new[] { '\0' });
                    }
                    else
                    {
                        msg.Id = ori;
                        msg._msgstr = new[] { tra };
                    }
                }
                return msgs;
            }

        }
    }
}
