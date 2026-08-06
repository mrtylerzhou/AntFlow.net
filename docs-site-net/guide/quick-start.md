# 快速开始

## 环境要求

在开始之前，请确保你的开发环境满足以下要求：

- **.NET SDK**：10.0 或更高版本
- **MySQL**：5.7 或更高版本
- **Node.js**：18+（用于前端开发）
- **Visual Studio 2022** 或 **JetBrains Rider** 或 **VS Code**

## 步骤一：克隆项目

```bash
# 克隆后端
git clone https://gitee.com/tylerzhou/AntFlowCore.git
cd AntFlowCore
```

## 步骤二：初始化数据库

1. 创建 MySQL 数据库：

```sql
CREATE DATABASE `antflow.net-next` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;
```

2. 执行初始化脚本：

```bash
mysql -u root -p antflow.net-next < script/bpm_init_db_mysql.sql
```

## 步骤三：配置连接字符串

编辑 `src/AntFlowCore.Web/appsettings.json`，修改数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "MySqlConnection": "server=localhost;userid=root;pwd=你的密码;port=3306;database=antflow.net-next;sslmode=none;Charset=utf8mb4"
  }
}
```

## 步骤四：启动后端

```bash
cd src/AntFlowCore.Web
dotnet run
```

后端启动后访问 `http://localhost:8001` 即可看到系统页面。

## 步骤五：启动前端

前端代码位于独立仓库（Vue 3）。前端启动请参见前端手册。

## 默认测试账号

项目内置了测试账号，登录页面可以选择用户，无需输入密码。点击登录按钮旁的"测试登录"即可快速进入系统。

::: tip 提示
开发环境下，登录页面通常会提供下拉选择账号的便利功能，方便开发者快速测试不同角色的审批流程。
:::

## 验证安装

打开浏览器访问 `http://localhost`，你应该看到以下界面：

- **流程管理系统** - 首页展示待办任务、已办任务等统计数据
- 点击左侧菜单可以导航到各个功能模块

![首页截图](/images/home.png)

## 登录页面

登录页面如下，可以通过下拉选择用户直接登录：

![登录页面](/images/login-page.png)

## 常见问题

**Q: 启动时提示数据库连接失败？**
A: 请检查 MySQL 服务是否启动，以及 appsettings.json 中的连接字符串是否正确。

**Q: 数据库表未创建？**
A: 首次启动时，请执行 `script/bpm_init_db_mysql.sql` 初始化数据库。

**Q: 前端页面无法加载？**
A: 前端代码位于独立仓库，需要单独克隆和启动。参见前端手册获取详情。

## 下一步

- [架构设计概览](/dev-guide/architecture)
- [流程设计器使用指南](/workflow-design/flow-designer)
- [审批人规则配置](/workflow-design/approver-rules)
