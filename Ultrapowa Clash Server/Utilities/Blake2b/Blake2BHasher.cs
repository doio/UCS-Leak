using System;

namespace UCS.Utilities.Blake2b
{
    internal class Blake2BHasher : Hasher
    {
        readonly Blake2BCore core = new Blake2BCore();
        readonly ulong[] rawConfig;
        readonly byte[] key;
        readonly int outputSizeInBytes;
        static readonly Blake2BConfig DefaultConfig = new Blake2BConfig();

        public override void Init()
        {
            core.Initialize(rawConfig);
            if (key != null)
            {
                core.HashCore(key, 0, key.Length);
            }
        }

        public override byte[] Finish()
        {
            try
            {
                var fullResult = core.HashFinal();
                if (outputSizeInBytes != fullResult.Length)
                {
                    var result = new byte[outputSizeInBytes];
                    Array.Copy(fullResult, result, result.Length);
                    return result;
                }
                else
                    return fullResult;
            } catch (Exception) { return null; }
        }

        public Blake2BHasher(Blake2BConfig config)
        {
            if (config == null)
                config = DefaultConfig;
            rawConfig = Blake2IvBuilder.ConfigB(config, null);
            if (config.Key != null && config.Key.Length != 0)
            {
                key = new byte[128];
                Array.Copy(config.Key, key, config.Key.Length);
            }
            outputSizeInBytes = config.OutputSizeInBytes;
            Init();
        }

        public override void Update(byte[] data, int start, int count)
        {
            core.HashCore(data, start, count);
        }
    }
}