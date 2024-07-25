# GitCommitReport

通过提交描述生成周报或其他报告

## 1. 使用方法

[下载](https://gitee.com/shixixiyue/git-commit-report/releases)

[在线体验](https://gitreport.shizhuoran.top/)

### docker

可以docker拉取到本地

compose.yaml

```
version: "3.8"
services:
  gitreport:
    restart: unless-stopped
    image: 935732994/gitcommitreport
    ports:
      - 8004:9091
    environment:
      - MODEL=gpt-4o-mini
      - URL=https://api.chatanywhere.tech/v1/chat/completions
      - KEY=sk-XXXX
```

### 1.1 配置 `helpconfig.json`

可以自己接个kimi

[GPT_API_free](https://gitcode.com/chatanywhere/GPT_API_free/overview?tab=readme-ov-file&utm_source=csdn_github_accelerator&isLogin=1) 提供的免费API，目前支持`gpt-4o`、`gpt-3.5-turbo`、`gpt-3.5-turbo-16k`

```
{
  "model": "gpt-4o", --模型名称
  "url": "", --api地址
  "key": "" --api key
}
```

### 1.2 配置用户名和Token

Gitee:设置-安全设置-私人令牌

Gitea:设置-应用-生成令牌

GitHub:Settings-Developer settings-Personal access tokens-Tokens(classic)

![](https://blog.shizhuoran.top/static/img/4104c52311e4e45aeec6855e6a62f019.02.webp)

### 1.3 新建项目

新建后会自动拉取日期范围内的提交，需要等待一会

![](https://blog.shizhuoran.top/static/img/2fb057fd032efa2b5a3557fe77caec73.03.webp)

### 1.4 生成报告

报告不会自动生成，需要手动生成，可以通过配置中的 `AI角色` 进行调整

![](https://blog.shizhuoran.top/static/img/8ac1493a70f9f0779fdc605214228258.01.webp)

![](https://blog.shizhuoran.top/static/img/eff5387ae9c13b1f7083fd008cf75d48.05.webp)

## 2. 本地开发

项目使用 .Net8 + FineUICore 开发

发布命令
```
dotnet publish -c Release -r linux-x64 --self-contained true /p:PublishSingleFile=false
```

```
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=false
```


## 3. 相关链接

[FineUI11](https://fineui.com/fans/)

[GPT_API_free](https://gitcode.com/chatanywhere/GPT_API_free/overview?tab=readme-ov-file&utm_source=csdn_github_accelerator&isLogin=1)

## 4. 结善缘

![](https://blog.shizhuoran.top/static/img/18c9e1719a9419ba2b2abb07f5e286ae.weixin20.webp)