using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using StackExchange.Redis;

namespace Polaris.WMS.BillNumbers
{
    /// <summary>
    /// 基于 Redis 的单号生成器实现
    /// [Dependency] 属性会自动将其注册为 IBillNumberGenerator 的实现
    /// </summary>
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IBillNumberGenerator))]
    public class RedisBillNumberGenerator : IBillNumberGenerator, ITransientDependency
    {
        private readonly IConnectionMultiplexer _redis;

        // ABP 会自动注入已配置好的 Redis 连接
        public RedisBillNumberGenerator(IConnectionMultiplexer redis)
        {
            _redis = redis;
            }

        public async Task<string> GetNextNumberAsync(string prefix)
        {
            // 1. 获取 Redis 数据库 (默认 DB)
            var db = _redis.GetDatabase();

            // 2. 生成 Key: "WMS:BillNo:ASN:20260211"
            // 每天一个新的 Key，保证序号每天从 1 开始重置
            var dateStr = DateTime.Now.ToString("yyyyMMdd");
            var key = $"WMS:BillNo:{prefix}:{dateStr}";

            // 3. 原子递增 (INCR)
            // 如果 Key 不存在，Redis 会自动创建并从 0 加到 1
            // 这是一个原子操作，并发安全
            var seq = await db.StringIncrementAsync(key);

            // 4. 设置过期时间 (TTL)
            // 如果是当天的第一个号 (seq == 1)，设置 25 小时后过期
            // 这样 Redis 会自动清理旧日期的 Key，保持干净
            if (seq == 1)
            {
                await db.KeyExpireAsync(key, TimeSpan.FromHours(25));
            }

            // 5. 格式化返回: ASN-20260211-0001
            // PadLeft(4, '0') 确保序号是 4 位，不足补 0
            return $"{prefix}-{dateStr}-{seq.ToString().PadLeft(4, '0')}";
        }
    }
}

