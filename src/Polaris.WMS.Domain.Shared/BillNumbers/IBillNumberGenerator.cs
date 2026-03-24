using System.Threading.Tasks;

namespace Polaris.WMS.BillNumbers
{
    /// <summary>
    /// 单号生成器接口
    /// 定义了生成单号的行为，但不依赖具体技术实现
    /// </summary>
    public interface IBillNumberGenerator
    {
        /// <summary>
        /// 获取下一个业务单号
        /// </summary>
        /// <param name="prefix">业务前缀 (如 ASN, OUT)</param>
        /// <returns>格式化后的单号 (如 ASN-20260211-0001)</returns>
        Task<string> GetNextNumberAsync(string prefix);
    }
}
