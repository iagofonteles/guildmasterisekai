using System;
using System.Security.Cryptography;

namespace GuildMasterIsekai
{
    public class UIntGenerator : IDisposable
    {
        private readonly byte[] _bytes = new byte[4];
        private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        public uint Generate()
        {
            _rng.GetBytes(_bytes);
            return BitConverter.ToUInt32(_bytes, 0);
        }

        public void Dispose() => _rng?.Dispose();
    }
}