namespace Polaris.WMS.Permissions;

public static class WMSPermissions
{
    /// <summary>
    /// 权限根节点。
    /// </summary>
    public const string GroupName = "WMS";

    public static class Global
    {
        public const string Default = GroupName + ".Global";
        // 上帝视角：允许查看所有车间数据
        public const string ViewAllDepartmentsData = Default + ".ViewAllDepartmentsData";
        public const string ViewAllWarehousesData = Default + ".ViewAllWarehousesData";
    }

    #region 库存管理

    /// <summary>
    /// 一级菜单：库存管理。
    /// </summary>
    public static class InventoryOps
    {
        public const string Default = GroupName + ".InventoryOps";

        /// <summary>
        /// 二级菜单：库存查询。
        /// </summary>
        public static class InventoryViews
        {
            public const string View = InventoryOps.Default + ".InventoryViews";
        }

        /// <summary>
        /// 二级菜单：库存流水。
        /// </summary>
        public static class InventoryTransactions
        {
            public const string View = InventoryOps.Default + ".InventoryTransactions";
        }
    }

    #endregion

    #region 库内作业

    /// <summary>
    /// 一级菜单：库内作业。
    /// </summary>
    public static class InternalOps
    {
        public const string Default = GroupName + ".InternalOps";

        /// <summary>
        /// 二级菜单：调拨单管理。
        /// </summary>
        public static class TransferOrders
        {
            public const string Default = InternalOps.Default + ".TransferOrders";
            public const string Create = Default + ".Create";
            public const string Update = Default + ".Update";
            public const string Delete = Default + ".Delete";
            public const string Approve = Default + ".Approve";
            public const string DeleteDetails = Default + ".DeleteDetails";
            public const string CreateDetails = Default + ".CreateDetails";
        }

        /// <summary>
        /// 二级菜单：盘点管理。
        /// </summary>
        public static class Stocktake
        {
            public const string Default = InternalOps.Default + ".Stocktake";
        }

        /// <summary>
        /// 二级菜单：分拆合盘。
        /// </summary>
        public static class PalletMerge
        {
            public const string Default = InternalOps.Default + ".PalletMerge";
        }
    }

    #endregion
}

