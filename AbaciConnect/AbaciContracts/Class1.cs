using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using System;
namespace AbaciConnect.Contracts
{
    public class Contract : SmartContract
    {
        private const string test_str = "Hello World";
        // Represents the owner of this contract, which is a fixed address
        public static readonly byte[] Owner =
          "ATrzHaicmhRj15C3Vv6e6gLfLqhSD2PtTr".ToScriptHash();
        // A constant factor number
        private const ulong factor = 100000000;
        public static String Main(string operation, object[] args)
        {
            Storage.Put("Hello", "World");
            return test_str;
        }
    }
}