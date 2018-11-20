using bwoah_shared.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bwoah_shared.DataClasses
{
    public enum NicknameOperation
    {
        Add = 0,
        Remove = 1,
        Change = 2,
        NickInUse = 3
    }

    public class NicknameOperationsData : AData
    {
        public NicknameOperation OperationType { get; set; }
        public DateTime Time { get; set; }
        public string NewNickname { get; set; }
        public string OldNickname { get; set; }
    }
}
