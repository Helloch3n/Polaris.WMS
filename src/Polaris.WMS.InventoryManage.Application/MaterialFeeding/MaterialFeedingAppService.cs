using Polaris.WMS.Inventories.Invnentory;
using Polaris.WMS.Inventories.Containers;
using Polaris.WMS.Inventories.Transaction;
using Polaris.WMS.InventoryManage.Application.Contracts.MaterialFeeding;
using Polaris.WMS.InventoryManage.Application.Contracts.MaterialFeeding.Dtos;
using Polaris.WMS.InventoryManage.Domain.inventories;
using Polaris.WMS.InventoryManage.Domain.inventories.Args;
using Polaris.WMS.InventoryManage.Domain.Containers;
using Polaris.WMS.MasterData.Containers;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Polaris.WMS.InventoryManage.Application.MaterialFeeding;

public class MaterialFeedingAppService(
    IRepository<Container, Guid> containerRepository,
    IRepository<Inventory, Guid> inventoryRepository,
    InventoryTransactionManager transactionService,
    ContainerManager containerManager,
    InventoryManager inventoryManager
) : ApplicationService, IMaterialFeedingAppService
{
    public async Task FeedAndLockAsync(FeedMaterialInputDto input)
    {
        // 1. 获取并校验盘具 (Container)
        var container = await containerRepository.FirstOrDefaultAsync(x => x.ContainerCode == input.ContainerCode);
        if (container == null)
        {
            throw new UserFriendlyException($"未找到编号为 {input.ContainerCode} 的盘具。");
        }

        if (container.Status == ContainerStatus.Empty)
        {
            throw new UserFriendlyException($"该盘具为空盘，无法进行投料");
        }

        if (container.IsLocked)
        {
            throw new UserFriendlyException($"该盘具被锁定，无法进行投料");
        }

        // 2. 获取该盘具上的所有库存明细
        var inventories = await inventoryRepository.GetListAsync(x => x.ContainerId == container.Id);
        if (!inventories.Any())
        {
            throw new UserFriendlyException($"该盘具上没有可用库存，无法投料。");
        }

        if (inventories.Any(x => x.Status != InventoryStatus.Good))
        {
            throw new UserFriendlyException($"该容器中存在非合格状态用的库存！不允许进入投料！请等待检验结果或先完成物理拆包（分盘）");
        }

        if (inventories.Any(x => x.LockedQuantity != 0))
        {
            throw new UserFriendlyException($"该容器中存在被其他单据占用的库存！请先完成物理拆包（分盘），或取消其他单据后，再将整包物料投入生产！");
        }

        // 3. 业务防呆校验：确保存储在这个盘上的库存都没有被锁定
        // if (inventories.Any(x => x.Status == InventoryStatus.Locked))
        // {
        //     throw new UserFriendlyException($"盘具内存在已被锁定的库存记录，数据异常。");
        // }

        // 4. 核心状态流转：锁定盘具
        // 推荐在 Domain Entity 内部写方法，而不是在这里直接 set 属性
        container.Lock($"{input.OrderNo}单据生产投料，进行锁定");
        await containerRepository.UpdateAsync(container);

        // 5. 核心状态流转：锁定库存
        foreach (var inv in inventories)
        {
            // 实体内部方法：修改状态为 Locked，绑定工单号
            inv.Reserve(inv.AvailableQuantity);
            await inventoryRepository.UpdateAsync(inv);

            // 6. 记录事务流水 (极其重要)
            // 调用领域服务写入 Transaction 记录，保证追溯性
            // var createArgs = new CreateInventoryTranscationArgs
            // {
            //     Id = GuidGenerator.Create(),
            //     Type = TransactionType.Feed,
            //     BillNo = string.IsNullOrWhiteSpace(input.OrderNo)
            //         ? $"Material-Feed-{Clock.Now:yyyyMMddHHmmssfff}"
            //         : input.OrderNo,
            //     InventoryId = inv.Id,
            //     ContainerId = inv.ContainerId,
            //     ProductId = inv.ProductId,
            //     Quantity = inv.Quantity,
            //     QuantityAfter = inv.Quantity,
            //     FromLocationId = null,
            //     ToLocationId = null,
            //     FromWarehouseId = null,
            //     ToWarehouseId = null,
            //     SN = inv.SN,
            //     BatchNo = inv.BatchNo,
            //     CraftVersion = inv.CraftVersion,
            //     Status = inv.Status,
            //     Remark = "材料投入"
            // };
            // await transactionService.CreateAsync(createArgs);
        }
    }

    public async Task UnlockAndReturnAsync(UnlockMaterialInputDto input)
    {
        // 1. 获取盘具 (Container)
        var container = await containerRepository.FirstOrDefaultAsync(x => x.ContainerCode == input.ContainerCode);
        if (container == null)
        {
            throw new UserFriendlyException($"未找到编号为 {input.ContainerCode} 的盘具。");
        }

        // 2. 状态防呆校验
        if (!container.IsLocked)
        {
            throw new UserFriendlyException($"盘具 {input.ContainerCode} 当前不是锁定状态，无需退料释放。");
        }

        // 3. 归属权校验：防止 A 机台错误释放了 B 机台锁定的盘具
        // 假设你在实体中保存了当前锁定的工单号 (CurrentWorkOrderNo)
        // if (container.CurrentWorkOrderNo != input.WorkOrderNo)
        // {
        //     throw new UserFriendlyException($"该盘具被工单 {container.CurrentWorkOrderNo} 锁定，当前请求的工单号不匹配，拒绝释放！");
        // }

        // 4. 获取该盘具上所有剩余的库存明细
        var inventories = await inventoryRepository.GetListAsync(x => x.ContainerId == container.Id);

        // 5. 如果盘具空了（被扣完了），触发空托盘回收逻辑
        // if (!inventories.Any() || inventories.All(x => x.Quantity <= 0))
        // {
        //     // 处理盘具生命周期
        //     await containerManager.HandleReelAfterInventoryDepletedAsync(container.Id);
        //     // 可以选择在这里发布一个领域事件，通知 WMS 生成一个回收空托盘的搬运任务
        //     // await _localEventBus.PublishAsync(new ContainerEmptiedEventData { ContainerId = container.Id, LocationId = container.LocationId });
        //     return;
        // }

        // 6. 核心状态流转：解锁盘具，恢复可用
        container.UnLock();
        await containerRepository.UpdateAsync(container);

        // 7. 核心状态流转：解锁库存，记录流水
        foreach (var inv in inventories)
        {
            if (inv.Quantity > 0)
            {
                inv.Unreserve(inv.Quantity);
                await inventoryRepository.UpdateAsync(inv);
                // 8. 记录事务流水 (极其重要)
                // 调用领域服务写入 Transaction 记录，保证追溯性
                // var createArgs = new CreateInventoryTranscationArgs
                // {
                //     Id = GuidGenerator.Create(),
                //     Type = TransactionType.Return,
                //     BillNo = string.IsNullOrWhiteSpace(input.OrderNo)
                //         ? $"Material-Feed-{Clock.Now:yyyyMMddHHmmssfff}"
                //         : input.OrderNo,
                //     InventoryId = inv.Id,
                //     ContainerId = inv.ContainerId,
                //     ProductId = inv.ProductId,
                //     Quantity = inv.Quantity,
                //     QuantityAfter = inv.Quantity,
                //     FromLocationId = null,
                //     ToLocationId = null,
                //     FromWarehouseId = null,
                //     ToWarehouseId = null,
                //     SN = inv.SN,
                //     BatchNo = inv.BatchNo,
                //     CraftVersion = inv.CraftVersion,
                //     Status = inv.Status,
                //     Remark = "材料退回"
                // };
                // await transactionService.CreateAsync(createArgs);
            }
        }
    }

    public async Task ConsumeAsync(ConsumeMaterialInputDto input)
    {
        // 1. 基础校验 (盘具存在、锁定状态、工单匹配等，同之前...)
        var container = await containerRepository.FirstOrDefaultAsync(x => x.ContainerCode == input.ContainerCode);
        if (container == null)
        {
            throw new UserFriendlyException($"未找到编号为 {input.ContainerCode} 的盘具。");
        }

        if (!container.IsLocked)
        {
            throw new UserFriendlyException($"编号为 {input.ContainerCode} 的盘具未被锁定，不能进行扣料。");
        }

        var inventories = await inventoryRepository.GetListAsync(x =>
            x.ContainerId == container.Id && x.LockedQuantity > 0 && x.Quantity > 0);


        // 【策略 A】：辅材精确扣料模式 (如果传入了具体的 SN 或批次)
        if (!string.IsNullOrWhiteSpace(input.InventorySN) || !string.IsNullOrWhiteSpace(input.BatchNo))
        {
            await ExecutePreciseDeductionAsync(input, inventories);
        }
        // 【策略 B】：主材级联扣料模式 (如果没有传具体明细，说明是线盘/吨包)
        else
        {
            await ExecuteCascadingDeductionAsync(input, inventories);
        }
    }

    private async Task ExecutePreciseDeductionAsync(ConsumeMaterialInputDto input, List<Inventory> inventories)
    {
        // 根据传入的 SN 或批次，精准锁定托盘上的某一个具体明细
        var targetInventory = inventories.FirstOrDefault(x =>
            (string.IsNullOrWhiteSpace(input.InventorySN) || x.SN == input.InventorySN) &&
            (string.IsNullOrWhiteSpace(input.BatchNo) || x.BatchNo == input.BatchNo));

        if (targetInventory == null)
        {
            throw new UserFriendlyException($"扣料失败：在该托盘上未找到 SN 为 {input.InventorySN} 或批次号为{input.BatchNo}的库存！");
        }

        if (targetInventory.Quantity < input.ConsumeQty)
        {
            throw new UserFriendlyException(
                $"超耗异常：箱码 {input.InventorySN} 仅余 {targetInventory.Quantity}，无法扣减 {input.ConsumeQty}！");
        }

        // 直接对这一箱/这一个批次进行精确扣减
        await inventoryManager.DeductQuantityAsync(targetInventory.Id, input.ConsumeQty, TransactionType.Issue);
    }

    private async Task ExecuteCascadingDeductionAsync(ConsumeMaterialInputDto input, List<Inventory> inventories)
    {
        // 没有传入 SN 或批次，说明是线盘/吨包，按照先进先出原则，从上到下依次扣减}
        var remainingQtyToDeduct = input.ConsumeQty;

        foreach (var inv in inventories.OrderByDescending(x => x.Sequence))
        {
            if (remainingQtyToDeduct <= 0)
            {
                break; // 已经扣减完了
            }

            var availableForDeduction = Math.Min(inv.Quantity, remainingQtyToDeduct);
            await inventoryManager.DeductQuantityAsync(inv.Id, availableForDeduction, TransactionType.Issue);
            remainingQtyToDeduct -= availableForDeduction;
        }

        if (remainingQtyToDeduct > 0)
        {
            throw new UserFriendlyException(
                $"超耗异常：该托盘仅余 {input.ConsumeQty - remainingQtyToDeduct}，无法扣减 {input.ConsumeQty}！");
        }
    }
}