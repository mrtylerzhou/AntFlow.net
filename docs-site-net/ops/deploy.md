# 部署指南

## 环境要求

### 运行时
- **.NET SDK**: 10.0 或更高版本
- **数据库**: MySQL 5.7+ / PostgreSQL 12+ / SQL Server 2019+
- **操作系统**: Windows / Linux / macOS

### 最低硬件配置
| 环境 | CPU | 内存 | 磁盘 |
|------|-----|------|------|
| 开发环境 | 2核 | 4GB | 20GB |
| 测试环境 | 4核 | 8GB | 50GB |
| 生产环境 | 8核 | 16GB | 100GB+ |

## 开发环境部署

### 1. 克隆代码

```bash
git clone https://github.com/tylerzhou/AntFlowCore.git
cd AntFlowCore
```

### 2. 还原依赖

```bash
dotnet restore
```

### 3. 配置数据库连接

编辑 `src/AntFlowCore.Api/appsettings.Development.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=antflowcore;Uid=root;Pwd=your_password;Charset=utf8mb4;"
  }
}
```

### 4. 初始化数据库

AntFlowCore 使用 FreeSql 的 CodeFirst 模式，首次启动会自动创建表结构：

```bash
dotnet run --project src/AntFlowCore.Api
```

> 也可以先执行 `dotnet ef migrations add InitialCreate` 生成迁移脚本检查后再更新。

### 5. 启动项目

```bash
# 进入API项目目录
cd src/AntFlowCore.Api

# 启动（开发模式，支持热重载）
dotnet watch run
```

服务默认运行在 `http://localhost:5000`，Swagger 文档地址：`http://localhost:5000/swagger`

![Swagger API文档](/images/swagger-api.png)

## 生产环境部署

### 1. 发布应用

```bash
# 发布 Release 版本
dotnet publish src/AntFlowCore.Api -c Release -o ./publish

# 或发布为单文件（便于部署）
dotnet publish src/AntFlowCore.Api -c Release -o ./publish --self-contained true -r linux-x64 -p:PublishSingleFile=true
```

### 2. 生产环境配置

编辑 `appsettings.Production.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=prod-db-server;Port=3306;Database=antflowcore;Uid=app_user;Pwd=strong_password;Charset=utf8mb4;"
  },
  "Jwt": {
    "Issuer": "AntFlowCore",
    "Audience": "AntFlowClient",
    "SecretKey": "your-256-bit-secret-key-here",
    "ExpireMinutes": 1440
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    }
  },
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:8080"
      }
    }
  }
}
```

### 3. 使用 Nginx 反向代理（Linux）

```nginx
server {
    listen 80;
    server_name flow.yourcompany.com;
    
    # 强制 HTTPS
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name flow.yourcompany.com;

    ssl_certificate /etc/ssl/certs/your-cert.pem;
    ssl_certificate_key /etc/ssl/private/your-key.pem;

    # 安全头
    add_header X-Frame-Options "SAMEORIGIN" always;
    add_header X-Content-Type-Options "nosniff" always;
    add_header X-XSS-Protection "1; mode=block" always;

    # 请求体大小限制（支持大文件上传）
    client_max_body_size 50M;

    location / {
        proxy_pass http://127.0.0.1:8080;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_cache_bypass $http_upgrade;
        
        # 超时设置
        proxy_connect_timeout 60s;
        proxy_send_timeout 60s;
        proxy_read_timeout 60s;
    }

    # 静态文件缓存
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2)$ {
        proxy_pass http://127.0.0.1:8080;
        expires 30d;
        add_header Cache-Control "public, immutable";
    }
}
```

### 4. 使用 Systemd 管理进程（Linux）

创建服务文件 `/etc/systemd/system/antflowcore.service`：

```ini
[Unit]
Description=AntFlowCore Workflow Engine
After=network.target mysql.service
Wants=network.target

[Service]
Type=notify
WorkingDirectory=/var/www/antflowcore
ExecStart=/usr/bin/dotnet /var/www/antflowcore/AntFlowCore.Api.dll
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=antflowcore
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

# 资源限制
LimitNOFILE=100000
MemoryMax=2G

[Install]
WantedBy=multi-user.target
```

启动并设置开机自启：

```bash
sudo systemctl enable antflowcore
sudo systemctl start antflowcore
sudo systemctl status antflowcore
```

### 5. Windows 服务部署

```powershell
# 创建 Windows 服务
sc.exe create AntFlowCore binPath="C:\Services\AntFlowCore\AntFlowCore.Api.exe"
sc.exe config AntFlowCore start=auto
sc.exe description AntFlowCore "AntFlowCore .NET Workflow Engine"

# 启动服务
sc.exe start AntFlowCore

# 停止服务
sc.exe stop AntFlowCore
```

## Docker 部署

### 1. 创建 Dockerfile

```dockerfile
# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 复制项目文件
COPY ["src/AntFlowCore.Api/AntFlowCore.Api.csproj", "AntFlowCore.Api/"]
COPY ["src/AntFlowCore.Base/AntFlowCore.Base.csproj", "AntFlowCore.Base/"]
COPY ["src/AntFlowCore.Engine/AntFlowCore.Engine.csproj", "AntFlowCore.Engine/"]
COPY ["src/AntFlowCore.Persist/AntFlowCore.Persist.csproj", "AntFlowCore.Persist/"]
COPY ["src/AntFlowCore.VirtualNode/AntFlowCore.VirtualNode.csproj", "AntFlowCore.VirtualNode/"]
COPY ["src/AntFlowCore.Bpmn/AntFlowCore.Bpmn.csproj", "AntFlowCore.Bpmn/"]
COPY ["src/AntFlowCore.Abstraction/AntFlowCore.Abstraction.csproj", "AntFlowCore.Abstraction/"]

# 还原依赖
RUN dotnet restore "AntFlowCore.Api/AntFlowCore.Api.csproj"

# 复制全部源码
COPY src/ ./src/

# 发布
WORKDIR "/src/AntFlowCore.Api"
RUN dotnet publish "AntFlowCore.Api.csproj" -c Release -o /app/publish

# 运行阶段
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# 安装字体（用于PDF导出）
RUN apt-get update && apt-get install -y --no-install-recommends \
    fonts-wqy-zenhei \
    fonts-wqy-microhei \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# 健康检查
HEALTHCHECK --interval=30s --timeout=3s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

EXPOSE 8080
ENTRYPOINT ["dotnet", "AntFlowCore.Api.dll"]
```

### 2. 创建 docker-compose.yml

```yaml
version: '3.8'

services:
  antflowcore-api:
    build:
      context: .
      dockerfile: Dockerfile
    container_name: antflowcore-api
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=mysql;Port=3306;Database=antflowcore;Uid=root;Pwd=antflow123;Charset=utf8mb4;
    depends_on:
      mysql:
        condition: service_healthy
    restart: unless-stopped
    networks:
      - antflow-net

  mysql:
    image: mysql:8.0
    container_name: antflowcore-mysql
    ports:
      - "3306:3306"
    environment:
      MYSQL_ROOT_PASSWORD: antflow123
      MYSQL_DATABASE: antflowcore
      MYSQL_CHARSET: utf8mb4
      MYSQL_COLLATION: utf8mb4_unicode_ci
    volumes:
      - mysql-data:/var/lib/mysql
      - ./scripts/init.sql:/docker-entrypoint-initdb.d/init.sql:ro
    command:
      - --character-set-server=utf8mb4
      - --collation-server=utf8mb4_unicode_ci
      - --default-authentication-plugin=mysql_native_password
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      interval: 10s
      timeout: 5s
      retries: 5
    restart: unless-stopped
    networks:
      - antflow-net

  redis:
    image: redis:7-alpine
    container_name: antflowcore-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    command: redis-server --appendonly yes --requirepass redis123
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 3s
      retries: 5
    restart: unless-stopped
    networks:
      - antflow-net

volumes:
  mysql-data:
  redis-data:

networks:
  antflow-net:
    driver: bridge
```

### 3. 构建并启动

```bash
# 构建镜像
docker-compose build

# 启动所有服务
docker-compose up -d

# 查看日志
docker-compose logs -f antflowcore-api

# 停止服务
docker-compose down

# 停止并删除数据卷
docker-compose down -v
```

### 4. 使用已有 MySQL 的部署方式

如果已有 MySQL 实例，可以只部署 API 服务：

```bash
docker build -t antflowcore:latest .

docker run -d \
  --name antflowcore-api \
  -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e "ConnectionStrings__DefaultConnection=Server=your-mysql-host;Port=3306;Database=antflowcore;Uid=app_user;Pwd=your_password;Charset=utf8mb4;" \
  --restart unless-stopped \
  antflowcore:latest
```

## 部署后检查清单

- [ ] 数据库连接正常，表结构已创建
- [ ] Swagger 文档可正常访问
- [ ] 健康检查接口 `/health` 返回 200
- [ ] 日志输出正常，无 ERROR 级别错误
- [ ] 前端页面可以正常加载
- [ ] 登录认证功能正常
- [ ] 流程预览图可以正常渲染
- [ ] 文件上传/下载功能正常
- [ ] 定时任务（如有）正常运行
- [ ] 监控指标正常采集

## 常见部署问题

**Q: 启动时报数据库连接失败？**

A: 检查：
1. 数据库服务是否启动
2. 连接字符串中的端口、用户名、密码是否正确
3. 数据库是否已创建
4. 防火墙是否开放了数据库端口

**Q: 数据库表未自动创建？**

A: FreeSql CodeFirst 需要确保程序集被正确扫描。检查启动配置中是否注册了 FreeSql 服务，以及实体类是否在扫描的程序集内。

**Q: Docker 容器内时间不对？**

A: 在 docker-compose 中添加时区设置：
```yaml
environment:
  - TZ=Asia/Shanghai
volumes:
  - /etc/localtime:/etc/localtime:ro
```
