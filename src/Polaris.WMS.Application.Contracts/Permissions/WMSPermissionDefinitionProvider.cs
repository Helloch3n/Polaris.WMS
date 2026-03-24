using Polaris.WMS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Polaris.WMS.Permissions;

/// <summary>
/// WMS 权限定义提供器。
/// 维护约定：权限树结构需与 <see cref="WMSPermissions"/> 常量层级保持一致。
/// </summary>
public class WMSPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        // 第一步：定义根分组
        var wmsGroup = context.AddGroup(WMSPermissions.GroupName, L("Permission:WMS"));

        // 第二步：按一级菜单拆分定义，便于统一维护
        DefineInventoryOpsPermissions(wmsGroup);
        DefineInternalOpsPermissions(wmsGroup);
        DefineGlobalOpsPermissions(wmsGroup);
    }
    #region 全局
    private static void DefineGlobalOpsPermissions(PermissionGroupDefinition wmsGroup)
    {
        var globalOpsPermission = wmsGroup.AddPermission(
           WMSPermissions.Global.Default,
           L("Permission:GlobalOps"));

        globalOpsPermission.AddChild(
           WMSPermissions.Global.ViewAllDepartmentsData,
           L("Permission:ViewAllDepartmentsData"));

        globalOpsPermission.AddChild(
           WMSPermissions.Global.ViewAllWarehousesData,
           L("Permission:ViewAllWarehousesData"));
    }
    #endregion
        #region 库存管理权限

    private static void DefineInventoryOpsPermissions(PermissionGroupDefinition wmsGroup)
    {
        var inventoryOpsPermission = wmsGroup.AddPermission(
            WMSPermissions.InventoryOps.Default,
            L("Permission:InventoryOps"));

        inventoryOpsPermission.AddChild(
            WMSPermissions.InventoryOps.InventoryViews.View,
            L("Permission:InventoryViews"));

        inventoryOpsPermission.AddChild(
            WMSPermissions.InventoryOps.InventoryTransactions.View,
            L("Permission:InventoryTransactions"));
    }

    #endregion

    #region 库内作业权限

    private static void DefineInternalOpsPermissions(PermissionGroupDefinition wmsGroup)
    {
        var internalOpsPermission = wmsGroup.AddPermission(
            WMSPermissions.InternalOps.Default,
            L("Permission:InternalOps"));

        var transferOrders = internalOpsPermission.AddChild(
            WMSPermissions.InternalOps.TransferOrders.Default,
            L("Permission:TransferOrders"));

        internalOpsPermission.AddChild(
            WMSPermissions.InternalOps.Stocktake.Default,
            L("Permission:Stocktake"));

        internalOpsPermission.AddChild(
            WMSPermissions.InternalOps.PalletMerge.Default,
            L("Permission:PalletMerge"));

        transferOrders.AddChild(WMSPermissions.InternalOps.TransferOrders.Create, L("Permission:Create"));
        transferOrders.AddChild(WMSPermissions.InternalOps.TransferOrders.Update, L("Permission:Update"));
        transferOrders.AddChild(WMSPermissions.InternalOps.TransferOrders.Delete, L("Permission:Delete"));
        transferOrders.AddChild(WMSPermissions.InternalOps.TransferOrders.Approve, L("Permission:Approve"));
        transferOrders.AddChild(WMSPermissions.InternalOps.TransferOrders.CreateDetails, L("Permission:CreateDetails"));
        transferOrders.AddChild(WMSPermissions.InternalOps.TransferOrders.DeleteDetails, L("Permission:DeleteDetails"));
    }

    #endregion

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WMSResource>(name);
    }
}

