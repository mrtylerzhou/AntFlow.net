# 故障排除

## 概述

本文档提供 AntFlowCore 常见问题的诊断方法和解决方案，涵盖启动、运行、性能、数据一致性等方面。

## 启动问题

### 1. 数据库连接失败

**症状**：启动时报错 `MySqlException` 或 `Unable to connect to any of the MySQL hosts specified`

**排查步骤**：

```bash
# 1. 检查数据库服务是否运行
systemctl status mysql    # Linux
net start MySQL80         # Windows

# 2. 测试网络连通性
telnet <db-host> 3306

# 3. 验证用户名密码
mysql -h <db-host> -u <username> -p

# 4. 检查数据库是否存在
mysql -e "SHOW DATABASES;" | grep antflowcore
```

**解决方案**：

```json
// 检查连接字符串格式
{
  "ConnectionString": "Server=localhost;Port=3306;Database=antflowcore;Uid=root;Pwd=password;Charset=utf8mb4;SslMode=None;"
}
```

常见原因：
- 数据库服务未启动
- 连接字符串中主机名/端口错误
- 用户名或密码错误
- 数据库未创建
- 防火墙阻止连接
- SSL 模式配置不当

### 2. 端口被占用

**症状**：`Address already in use` 或 `Failed to bind to address`

**排查步骤**：

```bash
# Linux: 查看端口占用
netstat -tlnp | grep 8080
lsof -i :8080

# Windows: 查看端口占用
netstat -ano | findstr 8080
tasklist | findstr <PID>
```

**解决方案**：

```json
// 修改监听端口
{
  "Kestrel": {
    "Endpoints": {
      "Http": { "Url": "http://0.0.0.0:8081" }
    }
  }
}
```

### 3. 程序集加载失败

**症状**：`FileNotFoundException` 或 `Could not load file or assembly`

**排查步骤**：

```bash
# 检查依赖是否完整
dotnet restore
dotnet list package

# 检查运行时是否安装
dotnet --list-runtimes
```

**解决方案**：

```bash
# 清理并重新构建
dotnet clean
dotnet restore
dotnet build -c Release
```

## 运行时问题

### 1. 流程启动失败

**症状**：调用流程启动接口返回错误或超时

**排查步骤**：

```csharp
// 开启详细日志
logger.LogDebug("流程启动参数: {Params}", 
    JsonSerializer.Serialize(startParams));

// 检查流程配置
var conf = await fsql.Select<BpmnConf>()
    .Where(o => o.FormCode == formCode)
    .FirstAsync();
    
if (conf == null)
{
    logger.LogError("流程配置不存在: {FormCode}", formCode);
}
```

**常见原因**：
- 流程模板未生效（`EffectiveBpmn` 未调用）
- 表单码与流程配置不匹配
- 审批人规则配置错误
- 条件表达式语法错误

### 2. 审批操作异常

**症状**：同意/拒绝操作失败

**排查步骤**：

```sql
-- 检查任务状态
SELECT id, process_number, task_name, status, assignee 
FROM bpm_af_task 
WHERE process_number = 'PROC20240101001';

-- 检查执行实例
SELECT * FROM bpm_af_execution 
WHERE process_number = 'PROC20240101001';
```

**常见原因**：
- 任务已被其他用户处理（状态已变更）
- 当前用户不是任务处理人
- 流程实例已被终止
- 节点配置变更导致流转失败

### 3. 条件判断不生效

**症状**：条件分支未按预期执行

**排查步骤**：

```csharp
// 检查条件变量值
var variables = await fsql.Select<BpmVariable>()
    .Where(o => o.ProcessNumber == processNumber)
    .ToListAsync();

logger.LogDebug("流程变量: {Variables}", 
    JsonSerializer.Serialize(variables));
```

**常见原因**：
- 条件变量名与表单字段名不匹配
- 变量值类型与条件类型不兼容
- 条件表达式语法错误
- AND/OR 逻辑组合配置错误

## 性能问题

### 1. 接口响应慢

**排查步骤**：

```bash
# 1. 查看慢查询日志
grep "Slow SQL" logs/app-*.log

# 2. 检查数据库连接池状态
mysql -e "SHOW STATUS LIKE 'Threads_connected';"
mysql -e "SHOW PROCESSLIST;"

# 3. 分析线程栈
dotnet-dump collect -p <PID>
dotnet-dump analyze <dump-file>
```

**解决方案**：

```json
// 增大连接池
{
  "ConnectionString": "Server=localhost;...;Min Pool Size=10;Max Pool Size=200;"
}
```

### 2. 内存泄漏

**症状**：内存持续增长，GC 频繁

**排查步骤**：

```bash
# 1. 监控 GC 行为
dotnet-counters collect -p <PID>

# 2. 分析堆内存
dotnet-dump collect -p <PID>
dotnet-dump analyze <dump-file>
# 在分析器中执行: dumpheap -stat
```

**常见原因**：
- 事件订阅未取消
- 静态集合持续增长
- 数据库连接未释放
- 大对象未及时释放

### 3. CPU 占用高

**排查步骤**：

```bash
# 1. 查看线程 CPU 占用
dotnet-stack collect -p <PID>

# 2. 分析热点方法
dotnet-trace collect -p <PID> --providers Microsoft-DotNETCore-SampleProfiler
```

## 数据一致性问题

### 1. 任务状态不一致

**症状**：任务显示已完成但流程未继续流转

**排查步骤**：

```sql
-- 检查任务与执行实例状态一致性
SELECT t.id, t.status as task_status, e.status as exec_status
FROM bpm_af_task t
JOIN bpm_af_execution e ON t.process_number = e.process_number
WHERE t.process_number = 'PROC20240101001';
```

**修复方案**：

```sql
-- 修复孤立任务（谨慎操作）
UPDATE bpm_af_task 
SET status = 'COMPLETED' 
WHERE process_number = 'PROC20240101001' 
  AND status = 'PENDING' 
  AND assignee IS NULL;
```

### 2. 流程变量丢失

**症状**：条件判断时获取不到变量值

**排查步骤**：

```sql
-- 检查变量表
SELECT * FROM bpm_variable 
WHERE process_number = 'PROC20240101001'
ORDER BY create_time;
```

**常见原因**：
- 变量在节点跳转时被清理
- 事务回滚导致变量未提交
- 并发写入冲突

## 部署问题

### 1. Docker 容器启动失败

**排查步骤**：

```bash
# 查看容器日志
docker logs antflowcore-api

# 检查容器状态
docker ps -a
docker inspect antflowcore-api

# 进入容器排查
docker exec -it antflowcore-api /bin/bash
```

**常见原因**：
- 数据库连接字符串配置错误
- 端口映射冲突
- 内存限制过小
- 时区配置问题

### 2. Nginx 502 Bad Gateway

**排查步骤**：

```bash
# 检查后端服务是否运行
curl http://127.0.0.1:8080/health

# 查看 Nginx 错误日志
tail -f /var/log/nginx/error.log

# 检查超时设置
grep proxy_read_timeout /etc/nginx/nginx.conf
```

**解决方案**：

```nginx
# 增加超时时间
proxy_read_timeout 300s;
proxy_connect_timeout 75s;

# 增加缓冲区
proxy_buffer_size 128k;
proxy_buffers 4 256k;
```

## 常见错误代码

| 错误码 | 含义 | 解决方案 |
|--------|------|---------|
| 10001 | 流程配置不存在 | 检查 formCode 是否正确 |
| 10002 | 流程未生效 | 调用 effectiveBpmn 接口 |
| 10003 | 任务不存在 | 检查任务ID或流程编号 |
| 10004 | 无操作权限 | 确认当前用户是否为任务处理人 |
| 10005 | 审批人未配置 | 检查节点审批人规则 |
| 10006 | 条件表达式错误 | 检查条件语法和变量名 |
| 20001 | 数据库连接失败 | 检查数据库服务状态 |
| 20002 | 数据库超时 | 优化查询或增加超时时间 |
| 30001 | 第三方系统调用失败 | 检查外部服务可用性 |
| 99999 | 系统内部错误 | 查看详细日志 |

## 紧急故障处理

### 服务不可用

1. **快速恢复**：
   ```bash
   # 重启服务
   sudo systemctl restart antflowcore
   
   # 或 Docker 方式
   docker restart antflowcore-api
   ```

2. **降级方案**：
   - 关闭非核心功能（如流程预览图）
   - 切换到备用数据库
   - 限流保护

3. **数据保护**：
   - 确保事务完整性
   - 检查是否有未提交的数据
   - 备份当前状态

### 数据损坏

1. **立即措施**：
   - 停止写入操作
   - 备份当前数据
   - 定位损坏范围

2. **修复流程**：
   ```sql
   -- 1. 检查表状态
   CHECK TABLE bpm_af_task;
   
   -- 2. 修复表
   REPAIR TABLE bpm_af_task;
   
   -- 3. 验证数据一致性
   SELECT COUNT(*) FROM bpm_af_task WHERE status IS NULL;
   ```

## 获取帮助

如果以上方案无法解决问题，请收集以下信息后联系技术支持：

- [ ] AntFlowCore 版本号
- [ ] .NET 运行时版本 (`dotnet --version`)
- [ ] 数据库类型和版本
- [ ] 完整的错误日志（包括堆栈跟踪）
- [ ] 问题复现步骤
- [ ] 相关配置（脱敏后）
- [ ] 系统资源使用情况（CPU、内存、磁盘）
