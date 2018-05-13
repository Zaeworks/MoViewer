using System;
using System.Linq;

namespace MoViewer
{
    class Msg
    {
        internal bool _plural = false;
        internal string _msgid_plural = null;
        internal string[] _msgstr;

        public int Offset { get; set; }
        public string Id { get; set; }
        public string Str { get => _msgstr[0]; }

        string _original;
        public string Original { get {
                if(_original is null)
                    _original = ((!_plural)? Id : $"{Id}\n[Plural]{_msgid_plural}")
                        .Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
                return _original;
            } }

        string _translation;
        public string Translation { get {
                if(_translation is null)
                    _translation = ((!_plural)? _msgstr[0]: string.Join("\n", _msgstr.Select((s, i) => $"[{i}]{s}")))
                        .Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
                return _translation;
            } }
    }
}
