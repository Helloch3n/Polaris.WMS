# WMS 项目开发指南与上下文

您是一位资深软件架构师和全栈开发人员，正在协助开发一套仓储管理系统 (WMS)。本项目采用**模块化单体 (Modular Monolith)** 架构，并使用**文件夹逻辑隔离**。

## 1. 技术栈
- **后端**: .NET 10, C# 13, ABP Framework (v10), Entity Framework Core, PostgreSQL。
- **前端**: Vue 3 (Composition API), TypeScript, Vite, Element Plus, Pinia。
- **工具**: Docker, Visual Studio 2026, VS Code。

## 2. 架构与 DDD 规则 (至关重要)
本项目为单体应用，但通过命名空间/文件夹在逻辑上划分为多个**限界上下文 (Bounded Contexts)**。

### 限界上下文划分
1. **BaseData (基础数据)**: `WMS.Domain/BaseData` (物料、供应商、客户)。
2. **Inventory (库存)**: `WMS.Domain/Inventory` (库位、库存余额、库存流水)。*这是库存数据的唯一真理来源 (Source of Truth)。*
3. **Inbound (入库)**: `WMS.Domain/Inbound` (ASN/入库单、上架任务)。
4. **Outbound (出库)**: `WMS.Domain/Outbound` (发货单、波次、拣货单)。

### 严格的边界规则
- **仅通过 ID 引用**: 严禁在不同上下文的聚合根之间建立对象导航属性 (Navigation Properties)。
  - *错误示例*: 在 `PutAwayTask` (上架任务) 中定义 `public Location TargetLocation { get; set; }`。
  - *正确示例*: 在 `PutAwayTask` (上架任务) 中定义 `public Guid TargetLocationId { get; set; }`。
- **禁止跨域逻辑**: 不要将 A 上下文的 Repository 注入到 B 上下文的 Domain Service 中。请使用 `IntegrationEvents` (集成事件) 或 `AppService` 接口进行交互。
- **优先收敛命名空间边界**: 在进行其他 DDD 优化之前，首先收敛命名空间边界。

## 3. 后端开发规范 (.NET/ABP)

### 领域层 (Domain Layer)
- **聚合根**: 继承自 `FullAuditedAggregateRoot<Guid>`。属性必须使用 `private set`。
- **构造函数**: 确保构造函数受保护 (internal/private)，对于复杂的创建逻辑，使用静态工厂方法 (例如 `CreateAsync`)。
- **并发控制**: 必须在关键的库存数量字段上添加 `[ConcurrencyCheck]` 属性。
- **管理者**: 复杂的业务逻辑请使用 `DomainService` (例如 `InventoryManager`)，而不是写在应用服务 (Application Service) 中。

### 应用层 (Application Layer)
- **DTOs**: 输入/输出必须始终使用 DTO。严禁直接暴露 Entity。
- **映射**: 使用 `ObjectMapper` 进行对象映射。
- **验证**: 实现 `IValidationEnabled` 接口或使用 FluentValidation。
- **编排服务**: `ReelAppService` 只负责编排，方法间避免相互引用，注释仅添加在重要编排流程。使用“第一步/第二步”或“1./2.”的流程化备注格式。

### 基础设施层 (Infrastructure)
- **EF Core**: 对所有实体映射使用 `IEntityTypeConfiguration<T>`。必须配置严格的数据类型 (例如 `builder.Property(x => x.Sku).HasMaxLength(50).IsRequired();`)。

## 4. 前端开发规范 (Vue/TS)

- **风格**: 使用 `<script setup lang="ts">`。
- **API 客户端**: 假设 API 客户端是基于 ABP Swagger 自动生成的。
- **状态管理**: 使用 Pinia 管理全局状态 (用户会话、权限)。
- **UI 组件**: 使用 Element Plus。
  - 表格: 必须实现分页和加载状态 (loading states)。
  - 表单: 使用 `el-form` 并配合响应式验证规则。
- **国际化**: 使用 `i18n` 键值，不要在代码中硬编码中文字符串。

## 5. 代码风格与质量
- **Async/Await**: 从 Controller 到 Database 调用的全链路必须使用 async/await。
- **命名规范**:
  - 接口: `I...`
  - 异步方法: `...Async`
  - 前端处理函数: `handle...` (例如 `handleSave`)。
- **注释**: 所有公开的后端 API 必须添加 XML 注释。
- **业务注释**: 业务代码需添加简洁明了的注释，优先说明关键业务步骤与约束。
- **异常文案**: 业务异常提示默认使用中文，保持统一可读性。领域异常信息优先使用中文。

## 6. WMS 特定业务逻辑 (项目上下文)
- **库存锁定**: 库存不仅仅是“数量 (Qty)”。它包含“已分配量 (AllocatedQty)”（被订单预占）和“可用量 (AvailableQty)”。
- **库位逻辑**: 库位可以被锁定。在生成上架建议前，必须检查 `IsLocked` 状态。